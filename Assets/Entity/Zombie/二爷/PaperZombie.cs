using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Assets.Scripts.Utils;
public class PaperZombie : ShieldZombie
{
    [Header("PaperZombie:ShieldZombie:Zombie")]

    public List<Sprite> paperSprites;
    public bool hasAngry = false;
    private float healTimer;
    public override void Start()
    {
        base.Start();   
        DOVirtual.DelayedCall(0.1f, () =>
        {
            shieldBarIcon.GetComponent<Image>().sprite = ImageManager.Instance.hpBarIcon[entityGroup == EntityGroup.friend ? 6 : golden ? 9 : 7];
            shieldBarInner.GetComponent<Image>().sprite = ImageManager.Instance.hpBarImages[entityGroup == EntityGroup.friend ? 10 : 11];
        });
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
    public override void fallArm()
    {
        transform.GetChild(0).Find("Zombie_paper_leftarm_lower").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_paper_hands3").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_paper_hands").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_paper_leftarm_upper2").gameObject.SetActive(true);
        base.fallArm();
    }
    public override void hideHead()
    {
        transform.GetChild(0).Find("Zombie_paper_glasses").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_jaw").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_paper_hairpiece").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_pupils").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_paper_head_look").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_paper_madhead").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_head").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_hair").GetComponent<SpriteRenderer>().sprite = null;
        loseHeadEffectType = hasAngry ? AreaEffectType.PaperZombieAngryLoseHead : AreaEffectType.PaperZombieNormalLoseHead;
        base.hideHead();
    }
    public override void onShieldLose()
    {
        base.onShieldLose();
        anim.SetTrigger("ThrowPaper");
        moveSpeed *= 0.001f;
        agent.speed = moveSpeed;
    }
    public override void shieldUpdate()
    {
        if (shieldHitpoint <= 0 || hitpoint <= 0)
        {
            onShieldLose();
            shield.GetComponent<SpriteRenderer>().sprite = null;
            return;
        }
        else if (shieldHitpoint <= 3)
        {
            shield.GetComponent<SpriteRenderer>().sprite = paperSprites[2];
        }
        else if (shieldHitpoint <= 6)
        {
            shield.GetComponent<SpriteRenderer>().sprite = paperSprites[1];
        }
        else
        {
            shield.GetComponent<SpriteRenderer>().sprite = paperSprites[0];
        }
        base.shieldUpdate();
    }
    private void transitionToAngry()
    {
        if (hasAngry) return;
        AudioHelper.paperZombieAngry();
        moveSpeed *= 5000f;
        agent.speed = moveSpeed;
        hitTime *= 0.3f;
        hasAngry = true;
    }
    private void animSetMoveSpeed(float speed)
    {
        moveSpeed = speed;
        agent.speed = moveSpeed;
    }
}
