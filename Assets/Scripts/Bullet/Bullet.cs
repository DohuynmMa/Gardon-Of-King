using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Utils;
public class Bullet : MonoBehaviour
{
    public BulletType bulletType;
    public AreaEffectType breakEffectType;
    public float speed;
    public float damage;
    public float towerAndHomeDamageTime = 1;//对防御塔和家门的伤害倍率,为1则等于damage
    public bool ifDestory;
    public Vector3 direction;
    public Entity shooter;
    public float flyRange;
    public Sounds hitSounds = Sounds.子弹打中普通; //击中音效
    public List<Entity> damagedEntity;
    /// <summary>
    /// 1:直线 2:抛物线
    /// </summary>
    public int flyMode;
    public BuffType hitBuffType;
    public float hitBuffTime;
    public float destroyTime = 0;//等待时间后删除 不要再inspector里面改!
    public bool allAim = false;//是否碰到任意敌方目标就造成伤害?

    private float flyEffectTimer;

    [Header("特殊设置")]
    public FatDaveFoodType fatDaveFoodType;
    public virtual void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Entity" || other.tag == "Home")
        {
            if (destroyTime != 0) return;
            var entity = other.GetComponent<Entity>();
            if (entity == null) return;
            if (entity == shooter || ((!allAim || GameManager.Instance.gameMode == GameMode.MultiPlayer) && entity != shooter.aim) || shooter.entityGroup == entity.entityGroup || hasDamaged(entity)) return;
            if (!damagedEntity.Contains(entity)) damagedEntity.Add(entity);
            onBulletHit(entity);
        }
    }
    public virtual void breakBullet()
    {
        if(breakEffectType != AreaEffectType.None)
        {
            var eff = Instantiate(Utils.findEffectByType(breakEffectType), transform.position, Quaternion.identity);
            if (eff.GetComponent<AoeEffect>() != null)
            {
                eff.summonner = shooter;
            }
        }
        DOVirtual.DelayedCall(destroyTime / DataManager.Instance.data.gameSpeed, () =>
        {
            Destroy(gameObject);
        });
    }
    public virtual void breakEvent()//子弹破坏的事件
    {
    }
    public virtual void onBulletHit(Entity entity)
    {
        if(fatDaveFoodType != FatDaveFoodType.None && GameManager.Instance.gameMode == GameMode.MiniGame_BS)
        {
            Instantiate(Utils.findDaveFoodByType(fatDaveFoodType),transform.position,Quaternion.identity);
            Sounds.弹.playWithPitch();
            Destroy(gameObject);
            return;
        }
        if (hitBuffType != BuffType.none)
        {
            if (entity.GetComponent<PaperZombie>() != null)//判断是否有Shield
            {
                PaperZombie zombie = entity.GetComponent<PaperZombie>();
                if (zombie.hasShield)
                {
                    if (flyMode != 1) BuffManager.Instance.addBuff(entity, hitBuffTime, hitBuffType);
                }
                else BuffManager.Instance.addBuff(entity, hitBuffTime, hitBuffType);
            }
            else
            {
                BuffManager.Instance.addBuff(entity, hitBuffTime, hitBuffType);
            }
        }
        if (destroyTime == 0) entity.changeHitpoint(damage * (entity.hasParent || entity.tag == "Home" ? towerAndHomeDamageTime : 1), this, shooter);
        #region 播放音效
        if (entity.GetComponent<ConeZombie>() != null)
        {
            if (entity.GetComponent<ConeZombie>().hasArmor)
            {
                new[]
                {
                        Sounds.子弹打中塑料1, Sounds.子弹打中塑料2,
                    }
                .random().playWithPitch();
            }
            else
            {
                Sounds.子弹打中普通.playWithPitch();
            }
        }
        else if (entity.GetComponent<BucketZombie>() != null || entity.entityType == EntityType.PekkaImp || entity.entityType == EntityType.GiantPekka)
        {
            if (entity.GetComponent<ArmorZombie>().hasArmor || entity.entityType == EntityType.PekkaImp || entity.entityType == EntityType.GiantPekka)
            {
                new[]
                {
                        Sounds.子弹打铁1, Sounds.子弹打铁2,
                    }
                .random().playWithPitch();
            }
            else
            {
                Sounds.子弹打中普通.playWithPitch();
            }
        }
        else if (hitSounds == Sounds.子弹打中普通) hitSounds.playWithPitch();

        if (hitSounds != Sounds.子弹打中普通 && hitSounds != Sounds.水晶破碎 && hitSounds != Sounds.水晶破碎2) hitSounds.playWithPitch();
        if (hitSounds == Sounds.水晶破碎 || hitSounds == Sounds.水晶破碎2)
        {
            new[]
                {
                        Sounds.水晶破碎, Sounds.水晶破碎2,
                    }
                .random().playWithPitch(Random.Range(1.2f, 1.6f), Random.Range(0.35f, 0.45f));
        }
        #endregion
        if (ifDestory && destroyTime == 0)
        {
            breakEvent();
            breakBullet();
        }
    }
    public virtual void bulletFly()
    {
        var boxCollider = GetComponent<CircleCollider2D>();
        if (shooter.boxCollider == null || boxCollider == null || shooter == null)
        {
            breakBullet();
            return;
        }
        if (shooter.aim == null || shooter.boxCollider.dist(boxCollider) > flyRange)
        {
            breakBullet();
            return;
        }
        if (shooter.boxCollider.dist(boxCollider) > flyRange)
        {
            breakBullet();
            return;
        }
    }
    private bool hasDamaged(Entity entity)
    {
        foreach (Entity e in damagedEntity)
        {
            if (entity == e) return true;
        }
        return false;
    }
}
