using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;
using DG.Tweening;
public class GemBullet : Bullet
{
    CircleCollider2D boxCollider;
    Vector3 shooterCoPos;
    Vector3 aimCoPos;
    private void Awake()
    {
        boxCollider = GetComponent<CircleCollider2D>();
    }
    private void Update()
    {
        bulletFly();
    }
    private void Start()
    {
        //取消联机中的联机效果
        if (GameManager.Instance.gameMode == GameMode.MultiPlayer)
        {
            var aim2 = shooter.findingClosestAim();
            if (!shooter.randomAims.Contains(aim2) && aim2 != null) shooter.randomAims.Add(aim2);
            Entity randomEntity = null;
            if (shooter.randomAims.Count != 0) randomEntity = shooter.randomAims[UnityEngine.Random.Range(0, shooter.randomAims.Count)];
            if (randomEntity != null) shooter.aim = randomEntity;
        }
        var random = Random.Range(0.6f, 1.5f);
        var aim = shooter.aim;
        shooterCoPos = boxCollider.bounds.center;
        aimCoPos = aim.getEntityBoxColliderPos();
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * (5 + (aimCoPos.y - shooterCoPos.y + Random.Range(0, 3f))), ForceMode2D.Impulse);
        damage *= GameManager.Instance.gameMode == GameMode.MultiPlayer ? 1 : random;
        transform.localScale *= random;
        GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
    public override void bulletFly()
    {
        base.bulletFly();
        var shooterCoPos = boxCollider.bounds.center;
        aimCoPos = shooter.aim.getEntityBoxColliderPos();
        var directionTemp = aimCoPos - shooterCoPos;
        transform.position += (directionTemp.x >= 0 ? Vector3.right : Vector3.left) * Mathf.Abs(directionTemp.x) * 2.2f * Time.deltaTime;
    }
}
