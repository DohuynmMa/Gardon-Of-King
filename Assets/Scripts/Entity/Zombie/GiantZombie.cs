using Assets.Scripts.Utils;
using UnityEngine;

public class GiantZombie : Zombie
{
    [Header("GiantZombie:Zombie")]
    public bool hasSon;
    public bool hasThrowSon;
    public override void Update()
    {
        base.Update();
        updateSelf();
    }
    public virtual void updateSelf()
    {
    }
    public virtual void throwSon()
    {
    }
    public virtual void attack()
    {
    }
    public override bool shoot(Bullet bullet = null)
    {
        hasSon = false;
        return base.shoot(bullet);
    }
    public override void processShotBullet(Bullet shotBullet)
    {
        var shootPos = transform.position;
        if (hasParent)
        {
            shootPos.x += bulletOffsetX * (parentEntity.transform.localScale.x >= 0 ? -1 : 1);
        }
        else
        {
            shootPos.x += (transform.localScale.x >= 0 ? bulletOffsetX : bulletOffsetX * -1f);
        }
        shootPos.y += bulletOffsetY;
        shotBullet.transform.position = shootPos;
        shotBullet.direction = (aim.getEntityBoxColliderPos() - shootPos) / (aim.getEntityBoxColliderPos() - shootPos).magnitude;
        shotBullet.flyRange = 20;
    }
}
