using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Utils;
public class ShieldZombie : Zombie
{
    [Header("ShieldZombie:Zombie")]
    public bool hasShield = true;
    public float shieldHitpoint;
    public float shieldMaxHitpoint;
    public Image shieldBar;
    public Image shieldBarInner;
    public GameObject shieldBarIcon;
    public GameObject shield;
    public AreaEffectType shieldFallingEffectType;

    public override void Update()
    {
        base.Update();
        if(hasShield) shieldUpdate();
    }
    public virtual void onShieldLose()
    {
        if (!hasShield || shieldBar == null) return;
        hasShield = false;
        shieldBar.gameObject.SetActive(false);
        if (shield == null) return;
        shield.SetActive(false);
        if (shieldFallingEffectType == AreaEffectType.None) return;
        Instantiate(Utils.findEffectByType(shieldFallingEffectType), shield.transform.position, Quaternion.identity);
    }
    public virtual void shieldUpdate()
    {
        if (!DataManager.Instance.data.showHpBar || BuffManager.Instance.hasBuff(this, BuffType.Bomb) || hitpoint <= 0 || (!onSpriteMask && Utils.inNight()))
        {
            shieldBar.gameObject.SetActive(false);
            return;
        }
        shieldBar.gameObject.SetActive(entityState == EntityState.enable);
        shieldBarInner.fillAmount = shieldHitpoint / shieldMaxHitpoint;

        Vector3 offsetPos = transform.position;
        offsetPos.y += hpBaroffsetY + 0.2f; offsetPos.x += hpBaroffsetX + 0.3f;
        Utils.uiMove(shieldBar.gameObject, offsetPos);
    }
    public override void addToCellEvent(Cell cell)
    {
        if (golden)
        {
            shieldMaxHitpoint *= 10;
        }
        if(hasShield) shieldHitpoint = shieldMaxHitpoint;
    }
    public override bool changeHitpoint(float damage, Bullet damageBullet = null, Entity damager = null, bool deltaTimeDamage = false)
    {
        if (hasShield && shieldHitpoint > 0 && (damageBullet != null ? damageBullet.flyMode != 2 : true))
        {
            shieldHitpoint -= damage;
            return base.changeHitpoint(0, damageBullet, damager, deltaTimeDamage);
        }
        else
        {
            return base.changeHitpoint(damage, damageBullet, damager, deltaTimeDamage);
        }
    }
}
