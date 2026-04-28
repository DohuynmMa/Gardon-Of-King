using Assets.Scripts.Utils;
using UnityEngine;
using System;
public class MelonBullet : Bullet
{
    Entity aim;
    Vector3 shooterCoPos;
    Vector3 aimCoPos;
    CircleCollider2D boxCollider;
    public AoeEffect aoePrefab;
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
        shooterCoPos = boxCollider.bounds.center;
        aimCoPos = aim.getEntityBoxColliderPos();
        var directionTemp = aimCoPos - shooterCoPos;
        transform.position += (directionTemp.x >= 0 ? Vector3.right : Vector3.left) * Math.Abs(directionTemp.x) * 2.2f * Time.deltaTime;
    }
    public override void breakEvent()
    {
        base.breakEvent();
        if (destroyTime != 0) return;
        Sounds.Î÷¹ÏÆÆÀÃ.playWithPitch();
        var aoe = Instantiate(aoePrefab, boxCollider.bounds.center, Quaternion.identity);
        aoe.summonner = shooter;
    }
}
