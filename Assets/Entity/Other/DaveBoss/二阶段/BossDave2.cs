using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这是boss二形态
/// </summary>
public class BossDave2 : Entity
{
    [Header("BossDaveAct2:Entity")]
    public LineRenderer line;//激光
    public GameObject 激光起点;
    public GameObject 激光终点;
    public BossDave2Target 准星;
    public DaveBossState daveBossState = DaveBossState.idle;
    public float damageTime;//激光伤害倍率
    public float damageMagn;//激光伤害倍率

    private float damageTimer;//激光伤害
    private float changeTimer;//改变形态
    public enum DaveBossState
    {
        idle,
        shoot
    }
    public override void Awake()
    {
        base.Awake();
        line.positionCount = 2;
        entityGroup = EntityGroup.enemy;
        entityState = EntityState.enable;
        准星.entity = this;
    }
    public override void Start()
    {
        base.Start();
        hpBarInner.fillAmount = hitpoint / maxHitpoint;
        GameManager.Instance.BG.gameObject.SetActive(false);
        Utils.changeBackground(BackgroundType.DayBoss);
        Musics.戴夫BOSS二阶段.play(true);
    }
    public override void Update()
    {
        base.Update();
        hpUpdate();
        foreach (var entity in Utils.findAllEntities())
        {
            if (entity == null) continue;
            if (entity.hasParent || entity.GetComponent<BossDave2>() != null || entity.entityState == EntityState.disable) continue;
            if (entity.boxCollider.bounds.center.x > 0.1f) entity.entityDie();
        }
        if (daveBossState == DaveBossState.idle) changeTimer += Time.deltaTime;
        if (changeTimer >= 12)
        {
            transitionToShoot();
            changeTimer = 0;
        }
        damageTimer += Time.deltaTime;
        if (daveBossState == DaveBossState.shoot)
        {
            if (aim == null)
            {
                激光终点.SetActive(false);
                return;
            }
            line.SetPosition(1, aim.boxCollider.bounds.center);
            Vector3 aimPos = line.GetPosition(1);
            准星.transform.position = aimPos;
            CameraManager.Instance.shake(0.02f);
            if (准星.aimed && hitpoint > 0)
            {
                激光终点.SetActive(true);
                激光终点.transform.position = aim.boxCollider.bounds.center;
                if(damageTimer >= 0.1f)
                {
                    Entity oldAim = aim;
                    aim.changeHitpoint(damageMagn * damageTime,null,this);
                    damageTimer = 0;
                    damageMagn += 0.01f;
                }
            }
            else 激光终点.SetActive(false);
        }
    }
    private void hpUpdate()
    {
        if(hitpoint <= 0)
        {
            anim.SetBool("Die", true);
            SoundsManager.stopMusic();
        }
    }
    private void bossDie()
    {
        entityDie();
        GameManager.Instance.gameOver(EntityGroup.friend);
    }
    private void transitionToShoot()
    {
        if (hitpoint <= 0) return;
        anim.SetTrigger("shoot");
        SoundsManager.playSounds(51, 1f, Time.timeScale);
    }
    private void daveShoot()//发射激光
    {
        daveBossState = DaveBossState.shoot;
        damageMagn = 0.1f;
    }
    private void stopShooting()//发射激光
    {
        daveBossState = DaveBossState.idle;
        激光终点.SetActive(false);
        line.SetPosition(1, line.GetPosition(0));
        准星.transform.position = line.GetPosition(0);
        aim = null;
        damageMagn = 0.1f;
    }
    private void shake(float value)//动画
    {
        CameraManager.Instance.shake(value);
        Sounds.explosion.playWithPitch();
    }
    private void playVoice(int ID)//动画
    {
        SoundsManager.playVoice(ID);
    }
    public override void updateHpBarImage()
    {

    }
}
