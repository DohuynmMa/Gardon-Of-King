using UnityEngine;
public class ImpZombie : Zombie
{
    [Header("ImpZombie:Zombie")]
    private float healTimer = 2.9f;//定期治疗小鬼原地踏步的毛病

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
        if(healTimer >= 3)
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
    public void setArmColor()//修改断手后贴图
    {
        if (entityGroup == EntityGroup.friend)
        {
            beLostingHand.GetComponent<SpriteRenderer>().sprite = blueSkin[3];
        }
        else beLostingHand.GetComponent<SpriteRenderer>().sprite = redSkin[3];
    }
}
