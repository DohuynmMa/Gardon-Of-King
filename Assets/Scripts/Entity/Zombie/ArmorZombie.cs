using Assets.Scripts.Utils;
using UnityEngine;

public class ArmorZombie : Zombie
{
    [Header("ArmorZombie:Zombie")]

    public GameObject armor;//·À¾ß²¿Î»
    public AreaEffectType fallingArmorEffectType;
    public bool hasArmor;

    public float maxArmorHp;
    public float armorHp;
    public override void Awake()
    {
        base.Awake();
        hasArmor = true;
    }
    public override void Update()
    {
        base.Update();
        armorUpdate();
    }
    public virtual void onArmorLose()
    {
        if (!hasArmor) return;
        hasArmor = false;
        if (armor == null) return;
        armor.gameObject.SetActive(false);
        armor.GetComponent<SpriteRenderer>().sprite = null;
        hpBarInner.fillAmount = 1;
        if (fallingArmorEffectType == AreaEffectType.None) return;
        Instantiate(Utils.findEffectByType(fallingArmorEffectType), armor.transform.position, Quaternion.identity);
    }
    public virtual void armorUpdate()
    {

        if (hasArmor)
        {
            if (entityGroup == EntityGroup.friend)
            {
                hpBarInner.sprite = ImageManager.Instance.hpBarImages[10];
            }
            else
            {
                hpBarInner.sprite = ImageManager.Instance.hpBarImages[11];
            }
        }
        else
        {
            if (entityGroup == EntityGroup.friend)
            {
                hpBarInner.sprite = ImageManager.Instance.hpBarImages[2];
            }
            else
            {
                hpBarInner.sprite = ImageManager.Instance.hpBarImages[3];
            }
        }
    }
    public override void updateHpBar()
    {
        if (hpBar != null && hpBarInner != null)
        {
            if (hasArmor)
            {
                hpBarInner.fillAmount = armorHp / maxArmorHp;
            }
            else hpBarInner.fillAmount = hitpoint / maxHitpoint;
        }
    }
    public override void addToCellEvent(Cell cell)
    {
        if (golden)
        {
            maxArmorHp *= 10;
        }
        if(hasArmor) armorHp = maxArmorHp;
    }
    public override bool changeHitpoint(float damage, Bullet damageBullet = null, Entity damager = null, bool deltaTimeDamage = false)
    {
        if(hasArmor && hasArmor)
        {
            armorHp -= damage;
            return base.changeHitpoint(0, damageBullet, damager, deltaTimeDamage);
        }
        else
        {
            return base.changeHitpoint(damage, damageBullet, damager, deltaTimeDamage);
        }
    }
}
