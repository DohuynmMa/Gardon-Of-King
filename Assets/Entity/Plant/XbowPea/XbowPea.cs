using Assets.Scripts.Utils;
using UnityEngine;
public class XbowPea : Plant
{
    public override void processShotBullet(Bullet shotBullet)
    {
        base.processShotBullet(shotBullet);
        Vector2 direction = (aim.getEntityBoxColliderPos() - shotBullet.transform.position).normalized;
        var angle = Vector2.SignedAngle(direction,Vector2.right * transform.position.normalized.x) * -1f;
        shotBullet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
