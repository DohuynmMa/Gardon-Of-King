using UnityEngine;
using Assets.Scripts.Utils;
using System.Collections.Generic;
public class GiantPekka : ArmorZombie
{
    [Header("GiantPekka:ArmorZombie")]
    public GameObject mainObj;
    public List<Sprite> pekkaHelmetSprites;
    Vector3 BDOffset;
    Vector3 BDSize;
    Entity singleEntity;
    public override void Start()
    {
        base.Start();
        BDOffset = boxCollider.offset;
        BDSize = boxCollider.size;
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
                transitionToAc3();
            }
            else if (armorHp <= maxArmorHp * 0.6f)
            {
                transitionToAc2();
            }
            else transitionToAc1();
        }
        base.armorUpdate();
    }
    public override void transitionToEnable()
    {
        base.transitionToEnable();
        Sounds.Æ¤¿¨¾ÞÈË·ÅÖÃ.playWithPitch();
        CameraManager.Instance.shake(0.2f);
    }
    private void showErrorEffecteByAnim()
    {
        Instantiate(Utils.findEffectByType(AreaEffectType.GiantPekkaError),this.getEntityBoxColliderPos(),Quaternion.identity);
    }
    private void afterSpecialAttackingStopHidingByAnim()
    {
        mainObj.SetActive(true);
        boxCollider.enabled = true;
    }
    private void showSpecialAttackEffectAndHideByAnim()
    {
        mainObj.SetActive(false);
        boxCollider.enabled = false;
        Instantiate(Utils.findEffectByType(AreaEffectType.GiantPekkaSpecialAttack), aim.getEntityBoxColliderPos(), Quaternion.identity);
        singleEntity = aim == null ? this : aim;
    }
    private void spawnSpecialAttackAoeAreaByAnim()
    {
        if (aim == null) return;
        Entity accurateEntity = singleEntity;
        if (accurateEntity == null) accurateEntity = this;
        if (accurateEntity.hitpoint <= 0 || accurateEntity.entityState == EntityState.disable) accurateEntity = this;
        var aoe = Instantiate(Utils.findEffectByType(AreaEffectType.GiantPekkaSpecialAoe), accurateEntity.getEntityBoxColliderPos(), Quaternion.identity).GetComponent<AoeEffect>();
        if (aoe != null) aoe.summonner = this;
    }
    private void attackByAnim()
    {
        if (aim == null) return;
        attack(damage, null, this);
        Sounds.Æ¤¿¨¾ÞÈË¹¥»÷.play();
    }
    private void pekkaBombAndSpawnAGargantuar()
    {
        var b = Instantiate(Utils.findEffectByType(AreaEffectType.PekkaBroken), this.getEntityBoxColliderPos(), Quaternion.identity);
        b.transform.localScale *= 4;
        var gar = Instantiate(HandManager.Instance.getEntityPrefeb(EntityType.Gargantuar), this.getEntityBoxColliderPos(), Quaternion.identity);
        gar.entityGroup = entityGroup;
        if (golden)
        {
            gar.golden = true;
            gar.damage *= 2;
            gar.maxHitpoint *= 10;
            gar.hitpoint = gar.maxHitpoint;
        }
        gar.updateHpBarImage();
        gar.updateHpBar();
        gar.transitionToEnable();
    }
    private void aboutBombByAnim()
    {
        Instantiate(Utils.findEffectByType(AreaEffectType.PekkaBroken), zombieHeadTransform.transform.position, Quaternion.identity);
        transform.GetChild(0).Find("Í¼²ã 1").gameObject.SetActive(false);
        transform.GetChild(0).Find("Í¼²ã 1").GetComponent<SpriteRenderer>().sprite = null;
    }
    public override void entityDie()
    {
        pekkaBombAndSpawnAGargantuar();
        base.entityDie();
    }
    public override void onArmorLose()
    {
        Instantiate(Utils.findEffectByType(AreaEffectType.PekkaBroken), zombieHeadTransform.transform.position, Quaternion.identity);
        transform.GetChild(0).Find("Í¼²ã 1").gameObject.SetActive(true);
        transform.GetChild(0).Find("Í¼²ã 1").GetComponent<SpriteRenderer>().sprite = pekkaHelmetSprites[3];
        base.onArmorLose();
    }
    public override void changeSkin()
    {
        var temp = 0;
        temp = anim.GetInteger("Act") - 1;
        transform.GetChild(0).Find("Åû·ç1½×¶Î").GetComponent<SpriteRenderer>().sprite = (entityGroup == EntityGroup.friend ? blueSkin : redSkin)[temp];
    }
    public void transitionToAc1()
    {
        if (!hasArmor) return;
        if (anim.GetInteger("Act") == 1) return;
        transform.GetChild(0).Find("Í¼²ã 1").gameObject.SetActive(true);
        transform.GetChild(0).Find("Í¼²ã 1").GetComponent<SpriteRenderer>().sprite = pekkaHelmetSprites[0];
        anim.SetInteger("Act", 1);
    }
    public void transitionToAc2()
    {
        if (!hasArmor) return;
        if (anim.GetInteger("Act") == 2) return;
        Instantiate(Utils.findEffectByType(AreaEffectType.PekkaBroken), zombieHeadTransform.transform.position, Quaternion.identity);
        transform.GetChild(0).Find("Í¼²ã 1").gameObject.SetActive(true);
        transform.GetChild(0).Find("Í¼²ã 1").GetComponent<SpriteRenderer>().sprite = pekkaHelmetSprites[1];
        anim.SetInteger("Act", 2);
        anim.SetTrigger("TransitionToAc2");
        moveSpeed = 0.000001f;
        agent.speed = moveSpeed;
        canBePushback = false;
        Sounds.Æ¤¿¨¾ÞÈËÇÐ»»ÐÎÌ¬.playWithPitch();
    }
    private void afterTransitionToAc2ByAnim()
    {
        moveSpeed = 0.2f;
        agent.speed = moveSpeed;
        transform.GetChild(0).Find("ÓÒµ¶£¬¾õÐÑÌ¬ °Îµ¶ºó").gameObject.SetActive(true);
        transform.GetChild(0).Find("ÓÒµ¶£¬¾õÐÑÌ¬ °Îµ¶Ç°").gameObject.SetActive(false);
    }
    public void transitionToAc3()
    {
        if (!hasArmor) return;
        if (anim.GetInteger("Act") == 3) return;
        Instantiate(Utils.findEffectByType(AreaEffectType.PekkaBroken), zombieHeadTransform.transform.position, Quaternion.identity);
        transform.GetChild(0).Find("Í¼²ã 1").gameObject.SetActive(true);
        transform.GetChild(0).Find("Í¼²ã 1").GetComponent<SpriteRenderer>().sprite = pekkaHelmetSprites[2];
        anim.SetInteger("Act", 3);
        moveSpeed = 0.4f;
        anim.speed *= 1.5f;
        agent.speed = moveSpeed;
        Sounds.Æ¤¿¨¾ÞÈËÇÐ»»ÐÎÌ¬.playWithPitch();
    }
}
