using Assets.Scripts.Utils;
using UnityEngine;

public class NPea : Bullet
{
    Entity aim;
    private void Update()
    {
        bulletFly();
    }
    private void Start()
    {
        aim = shooter.aim;
    }
    public override void bulletFly()
    {
        base.bulletFly();
        transform.Translate(direction * speed * Time.deltaTime);
        var boxCollider = GetComponent<CircleCollider2D>();
        if (shooter.boxCollider.dist(boxCollider) > flyRange)
        {
            Destroy(gameObject);
        }
    }
    public override void breakEvent()
    {
        var nuomizhi = Instantiate(Utils.findEffectByType(AreaEffectType.NurlMeWater), transform.position, Quaternion.identity);
        nuomizhi.GetComponent<LiquidOnTheGroundEffect>().parentEntityGroup = shooter.entityGroup;
    }
}
