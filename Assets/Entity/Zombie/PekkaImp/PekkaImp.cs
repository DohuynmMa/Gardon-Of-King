using Assets.Scripts.Utils;
using DG.Tweening;
using UnityEngine;

public class PekkaImp : ArmorZombie
{
    public override void Update()
    {
        base.Update();
        armorUpdate();
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
    public override void onArmorLose()
    {
        transform.GetChild(0).Find("PekkaImp_left_angle").gameObject.SetActive(false);
        transform.GetChild(0).Find("PekkaImp_left_angle").GetComponent<SpriteRenderer>().sprite = null;
        fallingArmorEffectType = entityGroup == EntityGroup.friend ? AreaEffectType.FallingBluePekkaHead : AreaEffectType.FallingRedPekkaHead;
        base.onArmorLose();
    }
    public override void fallArm()
    {
        transform.GetChild(0).Find("PekkaImp_left_hand").gameObject.SetActive(false);
        transform.GetChild(0).Find("PekkaImp_left_hand").GetComponent<SpriteRenderer>().sprite = null;
        base.fallArm();
    }
    public override void entityDie()
    {
        pekkaBroken();
        base.entityDie();
    }
    public override bool attack(float damage, Bullet damageBullet = null, Entity damager = null, bool deltaTimeDamage = false)
    {
        var aimPos = aim.getEntityBoxColliderPos();
        for(int i = 0; i < Random.Range(3,5); i++)
        {
            DOVirtual.DelayedCall(0.15f * i, () =>
            {
                var attackEffect = Instantiate(Utils.findEffectByType(AreaEffectType.PekkaHitEffect), aimPos, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
                attackEffect.transform.localScale *= Random.Range(0.5f, 0.8f);
                CameraManager.Instance.shake(0.01f);
            });
        }
        return base.attack(damage, damageBullet, damager, deltaTimeDamage);
    }
    public override void addToCellEvent(Cell cell)
    {
        Sounds.·ÅÖÃÆ¤¿¨.play();
        base.addToCellEvent(cell);
    }
    private void summonImp()
    {
        var imp = Instantiate(HandManager.Instance.getEntityPrefeb(EntityType.ImpZombie), this.getEntityBoxColliderPos(), Quaternion.identity);
        imp.entityGroup = entityGroup;
        if (golden)
        {
            imp.golden = true;
            imp.damage *= 2;
            imp.maxHitpoint *= 10;
            imp.hitpoint = imp.maxHitpoint;
        }
        imp.updateHpBarImage();
        imp.updateHpBar();
        imp.transitionToEnable();
    }
    private void pekkaAttackByAnim()
    {
        if(aim == null) return;
        attack(damage, null, this);
        Sounds.Æ¤¿¨»÷ÖÐ.play();
    }
    /// <summary>
    /// Æ¤¿¨Ëð»µ,ÕÙ»½Ð¡¹í½©Ê¬
    /// </summary>
    private void pekkaBroken()
    {
        summonImp();
        Instantiate(Utils.findEffectByType(AreaEffectType.PekkaBroken), this.getEntityBoxColliderPos(), Quaternion.identity);
    }
}
