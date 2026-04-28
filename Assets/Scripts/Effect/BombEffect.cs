using Assets.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;

public class BombEffect : AreaEffect
{
    public Entity entity;//parent entity
    public bool targetEnemy = true;
    public bool targetFriend;
    public float towerAndHomeDamagePercent = 1;
    public float damage;
    public List<Entity> damagedEntity;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Utils.scene == "GameMenu") return;
        if(other.tag == "Entity" || other.tag == "Home")
        {
            if (other.GetComponent<Entity>() == null) return;
            var entity = other.GetComponent<Entity>();
            if ((entity.entityGroup == this.entity.entityGroup && !targetFriend) || (entity.entityGroup != this.entity.entityGroup && !targetEnemy) || entity.hitpoint <= 0 || entity.entityState != EntityState.enable) return;
            bomb(entity);
        }
    }
    private void bomb(Entity entity)
    {
        if (hasDamaged(entity)) return;
        if (!damagedEntity.Contains(entity)) damagedEntity.Add(entity);
        if (entity.hasParent || entity.tag == "Home")
        {
            if (entity.hitpoint <= damage * towerAndHomeDamagePercent)
            {
                if (entity.hasParent) entity.parentEntity.GetComponent<Tower>().towerBreak();
                entity.hitpoint = 0;
                entity.entityDie();
            }
            else entity.changeHitpoint(damage * towerAndHomeDamagePercent);
        }
        else
        {
            if (entity.hitpoint <= damage)
            {
                //化敌为友模式,不会播放死亡动画,直接转换阵营
                if (GameManager.Instance.gameMode == GameMode.MiniGame_TETF)
                {
                    entity.changeEntityGroup();
                    return;
                }
                BuffManager.Instance.addBuff(entity, 5f, BuffType.Bomb);
            }
            else
            {
                entity.changeHitpoint(damage);
            }
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
