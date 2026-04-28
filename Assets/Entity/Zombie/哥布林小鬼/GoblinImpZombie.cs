using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Utils;
using UnityEngine.U2D.Animation;
public class GoblinImpZombie : Zombie
{
    [Header("GoblinImpZombie:Zombie")]
    private float healTimer = 2.9f;

    public GameObject beLostingHand;//会断的手臂,(要改变的贴图)
    public override void Start()
    {
        base.Start();
        agent.enabled = false;
        agent.enabled = true;
    }

    public override void Update()
    {
        base.Update();
        healTimer += Time.deltaTime;
        if (healTimer >= 3)
        {
            agent.enabled = false;
            agent.enabled = true;
            healTimer = 0;
        }
    }
    public override void hideHead()
    {
        transform.Find("Zombie_imp_head").gameObject.SetActive(false);
        transform.Find("Zombie_imp_jaw").gameObject.SetActive(false);
        base.hideHead();
    }
    public override void fallArm()
    {
        transform.Find("Zombie_imp_arm2").gameObject.SetActive(false);
        transform.Find("Zombie_imp_arm1").gameObject.SetActive(false);
        base.fallArm();
    }
    private void findCoin()//小鬼攻击概率爆金币
    {
        if (aim == null || aim.hitpoint <= 0 || entityGroup == EntityGroup.enemy) return;
        switch (Random.Range(1, 20))
        {
            case 3:
                Utils.summonCoin(this.getEntityBoxColliderPos(), 1);
                break;
        }
    }
}
