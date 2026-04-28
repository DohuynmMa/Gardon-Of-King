using Assets.Scripts.Utils;
using UnityEngine;

public class Pea : Bullet
{
    Entity aim;
    CircleCollider2D boxCollider;
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
        aim = shooter.aim;
    }
    public override void bulletFly()
    {
        base.bulletFly();
        transform.transform.position += direction * speed * Time.deltaTime;
        if (gameObject == null || shooter == null) return;
        if (shooter.boxCollider.dist(boxCollider) > flyRange)
        {
            Destroy(gameObject);
        }
    }
}
