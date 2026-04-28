using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallNut : Plant
{
    [Header("WallNut:Plant")]
    public GameObject body;
    private bool hurt1;
    private bool hurt2;
    public override void Awake()
    {
        base.Awake();
        hurt1 = false;
        hurt2 = false;
    }
    public override void Update()
    {
        base.Update();  
        if (hitpoint <= maxHitpoint * 0.3f && !hurt2)
        {
            anim.SetTrigger("hurt2");
            hurt2 = true;
        }
        else if (hitpoint <= maxHitpoint * 0.6f && !hurt1)
        {
            anim.SetTrigger("hurt1");
            hurt1 = true;
        }
        if (entityGroup == EntityGroup.enemy)
        {
            body.transform.localScale = new Vector3(-0.18f,0.18f, 0.18f);
        }
    }
    public override bool changeHitpoint(float damage, Bullet damageBullet = null, Entity damager = null, bool deltaTimeDamage = false)
    {
        return base.changeHitpoint(damage * 0.5f, damageBullet, damager, deltaTimeDamage);
    }
}
