using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Utils;

/// <summary>
/// 这是boss一形态
/// </summary>
public class BossDave : Entity
{
    [Header("BossDave:Entity")]
    public List<GameObject> shootPos;
    public List<GameObject> backPos;
    public List<BossDaveAWM> awms;
    public DaveBossState daveBossState = DaveBossState.idle;
    public GameObject mofazhen;
    public Bullet bomb;//手雷prefab

    public BossDave2 act2Prefab;//二阶段prefab

    private float changeTimer;//切换形态timer
    public enum DaveBossState
    {
        idle,
        shoot,
        rage,
        bomb,
    }
    public override void Start()
    {
        base.Start();
        entityGroup = EntityGroup.enemy;
        entityState = EntityState.enable;
        DOVirtual.DelayedCall(2.5f, () =>
        {
            hpBar.GetComponent<Animator>().enabled = false;
        });
    }
    public override void Update()
    {
        base.Update();
        if(maxHitpoint < 10000)
        {
            changeTimer += Time.deltaTime;
            if (changeTimer >= 12)
            {
                changeState();
                changeTimer = 0;
            }
            hitpointUpdate();
        }
    }
    public override void hpBarUpdate()
    {
        if (!DataManager.Instance.data.showHpBar || hitpoint <= 0)
        {
            hpBar.gameObject.SetActive(false);
            return;
        }
        hpBar.gameObject.SetActive(true);
        var offsetPos = transform.position;
        offsetPos.x += 0.4f;
        offsetPos.y += hpBaroffsetY; offsetPos.x += hpBaroffsetX;
        Utils.uiMove(hpBar.gameObject, offsetPos);
    }
    private void hitpointUpdate()
    {
        if (hitpoint <= 1000)
        {
            transitionToACT2();
            maxHitpoint = 999999999;
            hitpoint = maxHitpoint;
        }
    }
    private void changeState()
    {
        transitionToIdle();
        DOVirtual.DelayedCall(2f / DataManager.Instance.data.gameSpeed, () =>
        {
            switch (Random.Range(0,4))
            {
                case 0:
                    transitionToIdle();
                    break;
                case 1:
                    transitionToShoot();
                    break;
                case 2:
                    transitionToRage();
                    break;
                case 3:
                    transitionToThrowBomb();
                    break;
            }
        });
    }

    /// <summary>
    /// 定期给全部敌人上狂暴buff
    /// </summary>
    private void rage()
    {
        SoundsManager.playSounds(47, 0.3f);
        foreach(var entity in Utils.findAllEntities())
        {
            if(entity != null)
            {
                if (entity.hasParent || entity.hitpoint <= 0 || entity.entityState != EntityState.enable) continue;

                if (entity.entityGroup != entityGroup) continue;

                BuffManager.Instance.addBuff(entity, 5f, BuffType.Rage);
            }
        }
    }//anim
    private void transitionToIdle()
    {
        foreach (var awm in awms)
        {
            awm.transform.DOPath(new Vector3[] { backPos[awm.id].transform.position }, 0.5f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(()=> {
                if (awm.id == 2) {
                    daveBossState = DaveBossState.idle;
                }
            });
        }
        anim.SetBool("丢手雷", false);
        anim.SetBool("rage", false);
        anim.SetBool("shoot", false);
        mofazhen.GetComponent<SpriteRenderer>().color = Color.white;
    }
    private void transitionToShoot()
    {
        daveBossState = DaveBossState.shoot;
        foreach(var awm in awms)
        {
            awm.transform.DOPath(new Vector3[] { shootPos[awm.id].transform.position }, 0.5f, PathType.CatmullRom).SetEase(Ease.OutQuad);
        }
        anim.SetBool("shoot",true);
        mofazhen.GetComponent<SpriteRenderer>().color = Color.green;
    }
    private void transitionToRage()
    {
        daveBossState = DaveBossState.rage;
        anim.SetBool("rage", true);
        mofazhen.GetComponent<SpriteRenderer>().color = new Color32(233,0,255,255);
    }
    private void transitionToThrowBomb()
    {
        daveBossState = DaveBossState.bomb;
        DOVirtual.DelayedCall(2.5f, () =>
        {
            aim = null;
        });
        anim.SetBool("丢手雷", true);
        mofazhen.GetComponent<SpriteRenderer>().color = Color.red;
    }
    private void transitionToACT2()
    {
        var gm = GameManager.Instance;
        AI_Manager.Instance.enabled = false;
        bool playVideo = !DataManager.Instance.data.skipVideo;
        if(playVideo) VideoManager.Instance.play(1);
        gm.specCellSet = true;
        HandManager.Instance.stopAddEntity();
        foreach (var entity in Utils.findAllEntities())
        {
            if (entity == null) continue;
            if (entity.hasParent) continue;
            entity.entityDie();
        }
        if(gm.tower3 != null) gm.tower3.towerBreak();
        if (gm.tower4 != null) gm.tower4.towerBreak();
        if (gm.home2 != null) gm.home2.entityDie();
        gm.tower3 = null;
        gm.tower4 = null;
        gm.home2 = null;
        DOVirtual.DelayedCall(playVideo ? VideoManager.Instance.getTime(1) : 0.5f, () =>
        {
            Instantiate(act2Prefab);
            Destroy(gameObject);
        });
    }

    private void playSound(int ID)//动画
    {
        SoundsManager.playSounds(ID);
    }
    private void 落地()//anim
    {
        CameraManager.Instance.shake(0.5f);
        Sounds.砸瘪.playWithPitch();
    }
    public override void updateHpBarImage()
    {

    }
}
