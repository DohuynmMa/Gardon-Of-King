using UnityEngine.U2D.Animation;

public class BasicZombie : Zombie
{
    public override void shout()
    {
        AudioHelper.normalZombieShout();
    }
    public override void fallArm()
    {
        transform.GetChild(0).Find("Zombie_outerarm_hand").gameObject.SetActive(false);
        transform.GetChild(0).Find("Zombie_outerarm_lower").gameObject.SetActive(false);
        transform.GetChild(0).Find("Zombie_outerarm_upper").GetComponent<SpriteResolver>()
            .SetCategoryAndLabel("arm", "incomplete");
        base.fallArm();
    }
    public override void hideHead()
    {
        transform.GetChild(0).Find("Zombie_head").gameObject.SetActive(false);
        transform.GetChild(0).Find("Zombie_jaw").gameObject.SetActive(false);
        base.hideHead();
    }
    public override void enableUpdate()
    {
        base.enableUpdate();
    }
}
