using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Utils;
public class MinerZombie : ArmorZombie,AllAreaZombie
{
    public bool inMining = true;//是否挖土
    public List<Sprite> armorSprites;//防具状态图

    public Vector3 dest;
    public override void Update()
    {
        base.Update();
        if (inMining)
        {
            agent.enabled = true;
        }
    }
    public override void armorUpdate()
    {
        base.armorUpdate();
        if (hasArmor)
        {
            if (armorHp <= 0)
            {
                onArmorLose();
            }
            else if (armorHp <= 5)
            {
                armor.GetComponent<SpriteRenderer>().sprite = armorSprites[2];
            }
            else if (armorHp <= 8)
            {
                armor.GetComponent<SpriteRenderer>().sprite = armorSprites[1];
            }
            else armor.GetComponent<SpriteRenderer>().sprite = armorSprites[0];
        }
    }
    public void mine()
    {
        agent.enabled = true;
        if (agent != null && agent.enabled && inMining)
        {
            agent.speed = 3f;
            if (!agent.isOnNavMesh)
            {
                agent.enabled = false;
            }
            agent.SetDestination(dest);
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude == 0f)
            {
                agent.speed = 0;
                anim.SetTrigger("出土");
                if(entityType == EntityType.MinerZombie2)
                {
                    range = 4f;
                    agent.stoppingDistance = range;
                }
                DOVirtual.DelayedCall((entityType == EntityType.MinerZombie2 ? 3f : 5f) / DataManager.Instance.data.gameSpeed, () =>
                {
                    inMining = false;
                    transitionToEnable();
                    for (int i = 0; i < spriteRenderers.Length; i++)
                    {
                        if (spriteRenderers[i].gameObject.tag == "Shadow") continue;
                        spriteRenderers[i].sortingLayerName = "Entity";
                    }
                    agent.speed = moveSpeed;
                });
            }
        }
    }
    public override void transitionToDisable()
    {
        base.transitionToDisable();
        agent.enabled = true;
        foreach (var spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer.gameObject.tag == "Shadow") continue;
            spriteRenderer.sortingLayerName = "Entity";
        }
    }
    public override void useCardEvent()
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer.gameObject.tag == "Shadow") continue;
            spriteRenderer.sortingLayerName = "Other";
        }
    }
    public override void enableUpdate()
    {
        if (inMining)
        {
            mine();
            return;
        }
        base.enableUpdate();
    }
    public override void fallArm()
    {
        if(entityType == EntityType.MinerZombie2)
        {
            transform.GetChild(0).Find("左上臂").GetComponent<SpriteRenderer>().sprite = entityGroup == EntityGroup.enemy ? redSkin[2] : blueSkin[2];
        }
        transform.GetChild(0).Find("Zombie_digger_outerarm_hand").GetComponent<SpriteRenderer>().sprite = null;
        transform.GetChild(0).Find("Zombie_digger_outerarm_lower").GetComponent<SpriteRenderer>().sprite = null;

        base.fallArm();
    }
    public override void hideHead()
    {
        transform.GetChild(0).Find("Zombie_digger_head").GetComponent<SpriteRenderer>().sprite = null;
        base.hideHead();
    }
    public override void onArmorLose()
    {
        if (entityType == EntityType.MinerZombie2)
        {
            transform.GetChild(0).Find("图层 2").GetComponent<SpriteRenderer>().sprite = null;
        }
        else
        {
            transform.GetChild(0).Find("Zombie_digger_hardhat").GetComponent<SpriteRenderer>().sprite = null;
        }
        base.onArmorLose();
    }
    public override void addToCellEvent(Cell cell)
    {
        print(entityGroup);
        transform.position = (entityGroup == EntityGroup.friend ? GameManager.Instance.home1 : GameManager.Instance.home2).transform.position;
        dest = cell.entityPosition(this);
    }
    private void mineGem()//anim : 发射随机宝石
    {
        if (aim == null) return;
        var temp = Random.Range(5, 30);
        if (temp >= 20) temp = Random.Range(10, 30);
        temp *= 2;
        if(GameManager.Instance.gameMode == GameMode.MultiPlayer)
        {
            for (int i = 0; i < 20; i++)
            {
                shoot();
            }
        }
        else
        {
            for (int i = 0; i < temp; i++)
            {
                shoot();
            }
            for (int i = 0; i < randomAims.Count; i++)
            {
                if (randomAims[i] == null) randomAims.Remove(randomAims[i]);
            }
            aim = findingClosestAim();
        }
    }
}
