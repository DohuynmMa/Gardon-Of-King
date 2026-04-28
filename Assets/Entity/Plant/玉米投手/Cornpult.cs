using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.NetWork.Packet.Play.Server;
public class Cornpult : Plant
{
    [Header("Cornpult:Plant")]
    public GameObject kernal;//玉米粒
    public GameObject butter;//黄油
    public List<Sprite> bulletState;//子弹图

    public Bullet nextBullet;
    public Bullet butterBullet;
    public Bullet kernalBullet;

    public int bulletTemp = 0;
    public override void Start()
    {
        base.Start();
        bulletUpdate();
    }
    private void bulletUpdate()//更新贴图
    {
        bulletTemp = MultiGameManager.server == null && GameManager.Instance.gameMode == GameMode.MultiPlayer ? -1 : Random.Range(0, 4);
        //服务端传递玉米子弹TEMP
        if (MultiGameManager.server != null && GameManager.Instance.gameMode == GameMode.MultiPlayer)
        {
            NetworkServerService.getUserById(1).Send(new PlayServerCornPultBulletUpdate(entityID, bulletTemp));
        }
        if (bulletTemp == -1) return;
        if (bulletTemp == 3)
        {
            kernal.GetComponent<SpriteRenderer>().sprite = null;
            butter.GetComponent<SpriteRenderer>().sprite = bulletState[1];
            nextBullet = butterBullet;
        }
        else
        {
            butter.GetComponent<SpriteRenderer>().sprite = null;
            kernal.GetComponent<SpriteRenderer>().sprite = bulletState[0];
            nextBullet = kernalBullet;
        }
    }
    private void cornShoot()//anim
    {
        shoot(nextBullet);
        bulletUpdate();
    }
    public void syncBulletByPacket(int syncTemp)
    {
        if (syncTemp == 3)
        {
            kernal.GetComponent<SpriteRenderer>().sprite = null;
            butter.GetComponent<SpriteRenderer>().sprite = bulletState[1];
            nextBullet = butterBullet;
        }
        else
        {
            butter.GetComponent<SpriteRenderer>().sprite = null;
            kernal.GetComponent<SpriteRenderer>().sprite = bulletState[0];
            nextBullet = kernalBullet;
        }
    }
}
