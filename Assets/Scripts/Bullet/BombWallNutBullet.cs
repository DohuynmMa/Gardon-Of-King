using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;
using System;

public class BombWallNutBullet : Bullet
{
    Entity aim;
    Vector3 shooterCoPos;
    Vector3 aimCoPos;
    CircleCollider2D boxCollider;
    public BombEffect bombPrefab;
    private void Awake()
    {
        boxCollider = GetComponent<CircleCollider2D>();
    }
    private void Start()
    {
        aim = shooter.aim;
        shooterCoPos = boxCollider.bounds.center;
        aimCoPos = aim.getEntityBoxColliderPos();
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * (5 + (aimCoPos.y - shooterCoPos.y)), ForceMode2D.Impulse);
    }
    private void Update()
    {
        bulletFly();
    }
    public override void bulletFly()
    {
        base.bulletFly();
        Vector2 colliderPos1 = boxCollider.bounds.center;
        shooterCoPos = new Vector3(colliderPos1.x, colliderPos1.y, 0);
        aimCoPos = aim.getEntityBoxColliderPos();
        Vector3 directionTemp = aimCoPos - shooterCoPos;
        transform.position += (directionTemp.x >= 0 ? Vector3.right : Vector3.left) * Math.Abs(directionTemp.x) * 2.2f * Time.deltaTime;
    }
    public override void breakEvent()
    {
        var bombEffect = Instantiate(bombPrefab, transform.position, Quaternion.identity);
        bombEffect.entity = shooter;
    }
}
