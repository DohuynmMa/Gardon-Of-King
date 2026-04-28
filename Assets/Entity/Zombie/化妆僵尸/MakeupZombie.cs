using Assets.Scripts.Utils;
using UnityEngine;

public class MakeupZombie : ArmorZombie
{
    [Header("MakeupZombie:ArmorZombie:Zombie")]
    public bool hasAngry = false;
    private float healTimer;
    public GameObject allObj;
    public override void Update()
    {
        base.Update();
        healTimer += Time.deltaTime;
        if (healTimer >= 1.5f)
        {
            agent.enabled = false;
            agent.enabled = true;
            healTimer = 0;
        }
    }

    public override void fallArm()
    {
        transform.GetChild(0).Find("Zombie_outerarm_lower").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_outerarm_hand").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_outerarm_upper").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("¿ñ±©Ð¡±Û").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("¿ñ±©ÉÏ±Û").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("¶Ï±Û").gameObject.SetActive(true);
        loseArmEffectType = hasAngry ? AreaEffectType.MakeupZombieAngryLoseArm : AreaEffectType.MakeupZombieNormalLoseArm;
        base.fallArm();
    }
    public override void hideHead()
    {
        transform.GetChild(0).Find("Zombie_head").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_jaw").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_head2").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("¶ú×¹").GetComponent<SpriteRenderer>().sprite = null;
        loseHeadEffectType = hasAngry ? AreaEffectType.MakeupZombieAngryLoseHead : AreaEffectType.MakeupZombieNormalLoseHead;
        base.hideHead();
    }
    public override void onArmorLose()
    {
        base.onArmorLose();
        anim.SetTrigger("ThrowPaper");
        moveSpeed *= 0.001f;
        agent.speed = moveSpeed;
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
    private void transitionToAngry()
    {
        if (hasAngry) return;
        moveSpeed *= 5000f;
        allObj.transform.localScale *= 1.3f;
        agent.speed = moveSpeed;
        damage *= 1.5f;
        hitTime *= 0.6f;
        hasAngry = true;
        var bombEffect = Instantiate(Utils.findEffectByType(AreaEffectType.MakeupZombieBomb), transform.position, Quaternion.identity).GetComponent<BombEffect>();
        bombEffect.entity = this;
        bombEffect.transform.localScale *= 0.7f;
    }
    private void cai()
    {
        if (aim == null) return;
        CameraManager.Instance.shake(0.1f);
        Sounds.ÔÒ±ñ.playWithPitch();
        float damage = 8;
        if (aim.hasParent || aim.tag == "Home") damage = 4;
        var aimCoPos = aim.getEntityBoxColliderPos();
        if (Vector3.Distance(GetComponent<BoxCollider2D>().bounds.center, aimCoPos) >= range * 1.5f) return;
        if (aim.changeHitpoint(damage,null,this))
        {
            if (aim.hasParent || aim.tag == "Home" || GameManager.Instance.gameMode == GameMode.MiniGame_TETF) return;

            var zabiePrefab = Utils.findEffectByType(AreaEffectType.RottenPlant).GetComponent<RottenPlantEffect>();
            zabiePrefab.entityType = aim.entityType;
            var zabieEffect = Instantiate(zabiePrefab.gameObject, aimCoPos, Quaternion.identity);
            Sounds.½©Ê¬³Ô.playWithPitch(Random.Range(0.75f, 0.9f));
        }
    }
}
