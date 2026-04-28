using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewYearZombie : ArmorZombie
{
    public override void shout()
    {
        AudioHelper.normalZombieShout();
    }
    private void makeDumplings()
    {
        if (aim.isPlant() && !aim.hasParent) aim.entityDie();
    }
    public override void hideHead()
    {
        transform.GetChild(0).Find("Zombie_head").gameObject.SetActive(false);
        transform.GetChild(0).Find("Zombie_jaw").gameObject.SetActive(false);
        base.hideHead();
    }
    public override void armorUpdate()
    {
        if (hasArmor)
        {
            if (armorHp <= 0)
            {
                onArmorLose();
            }
        }
        base.armorUpdate();
    }
    private void setMoveSpeed(float speed)
    {
        moveSpeed = speed;
        agent.speed = speed;
    }
}
