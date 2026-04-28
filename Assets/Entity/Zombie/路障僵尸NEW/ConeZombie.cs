using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class ConeZombie : ArmorZombie
{
    [Header("ConeZombie:ArmorZombie:Zombie")]
    public List<Sprite> armorSprites;//·À¾ß×´Ì¬Í¼
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
    public override void armorUpdate()
    {
        if (hasArmor)
        {
            if (armorHp <= 0)
            {
                onArmorLose();
            }
            else if (armorHp <= maxArmorHp * 0.3f)
            {
                armor.GetComponent<SpriteRenderer>().sprite = armorSprites[2];
            }
            else if (armorHp <= maxArmorHp * 0.6f)
            {
                armor.GetComponent<SpriteRenderer>().sprite = armorSprites[1];
            }
            else armor.GetComponent<SpriteRenderer>().sprite = armorSprites[0];
        }
        base.armorUpdate();
    }
}
