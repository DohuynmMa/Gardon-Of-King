using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.Utils;

public class Son : Bullet
{
    Entity aim;
    public Entity impPrefeb;
    Vector3 shooterCoPos;
    Vector3 aimCoPos;
    CircleCollider2D boxCollider;
    public List<GameObject> changeSkinParts;
    public List<Sprite> groupSkinsRed;
    public List<Sprite> groupSkinsBlue;
    private void Awake()
    {
        boxCollider = GetComponent<CircleCollider2D>();
    }
    private void Start()
    {
        aim = shooter.aim;
        transform.localScale = new Vector3(shooter.transform.localScale.x >= 0 ? 1.4f : -1.4f, 1.4f, 1.4f);
        var i = 0;
        shooterCoPos = boxCollider.bounds.center;
        aimCoPos = aim.getEntityBoxColliderPos();
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * (5 + (aimCoPos.y - shooterCoPos.y)), ForceMode2D.Impulse);
        foreach (var obj in changeSkinParts)
        {
            if (shooter.entityGroup == EntityGroup.enemy)
            {
                obj.GetComponent<SpriteRenderer>().sprite = groupSkinsRed[i];
            }
            else obj.GetComponent<SpriteRenderer>().sprite = groupSkinsBlue[i];
            i++;
        }
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
        transform.position += (directionTemp.x >= 0 ? Vector3.right : Vector3.left) * Math.Abs(directionTemp.x) * 3f * Time.deltaTime;
    }
    private void summonImp()
    {
        impPrefeb = HandManager.Instance.getEntityPrefeb(EntityType.ImpZombie);
        impPrefeb.entityGroup = shooter.entityGroup;

        if (aim == null) aimCoPos = transform.position;
        var imp = Instantiate(impPrefeb, aimCoPos, Quaternion.identity);
        if (shooter.golden)
        {
            imp.golden = true;
            imp.damage *= 2;
            imp.maxHitpoint *= 10;
            imp.hitpoint = imp.maxHitpoint;
        }
        imp.updateHpBarImage();
        imp.updateHpBar();
        imp.transitionToEnable();
        Destroy(gameObject);
    }
    public override void breakBullet()
    {
    }
    public override void breakEvent()
    {
        summonImp();
    }
}
