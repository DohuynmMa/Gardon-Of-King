using Assets.Scripts.Utils;
using UnityEngine;
using System;

public class Butter : Bullet
{
    Entity aim;
    Vector3 shooterCoPos;
    Vector3 aimCoPos;
    CircleCollider2D boxCollider;
    public Sprite butterBreak;
    Vector3 lockPos;
    private void Awake()
    {
        boxCollider = GetComponent<CircleCollider2D>();
    }
    private void Start()
    {
        aim = shooter.aim;
        shooterCoPos = boxCollider.bounds.center;
        aimCoPos = aim.getEntityBoxColliderPos();
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * (5 + (aimCoPos.y - shooterCoPos.y)) , ForceMode2D.Impulse);
    }
    private void Update()
    {
        if (destroyTime != 0)
        {
            transform.position = lockPos;
            if (aim == null)
            {
                print("aim = null : butter");
                Destroy(gameObject);
                return;
            }
            if (aim.hitpoint <= 0)
            {
                print("aim dead");
                aim.anim.speed = 1f;
                Destroy(gameObject);
            }
            return;
        }
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
        destroyTime = 4;
        Sounds.ª∆”Õ.playWithPitch();
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().mass = 0;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Animator>().enabled = false;
        transform.rotation = Quaternion.identity;
        GetComponent<SpriteRenderer>().sprite = butterBreak;
        lockPos = aim.GetComponent<BoxCollider2D>().bounds.center;
        lockPos.y += 0.55f;
    }
}
