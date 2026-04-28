using Assets.Scripts.Utils;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Zombie
{
    private float healTimer = 2.9f;
    public override void Start()
    {
        base.Start();
        agent.enabled = false;
        agent.enabled = true;
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
    private void startLeaveDirt()
    {
        agent.speed = 0;
    }
    private void endLeaveDirt()
    {
        agent.radius = 0.5f;
        agent.enabled = true;
        agent.speed = moveSpeed;
        transitionToEnable();
    }
    public override void transitionToEnable()
    {
        base.transitionToEnable();
        Sounds.·ÅÖÃ÷¼÷Ã.playWithPitch(1,0.15f);
    }
    private void skeletonAttackByAnim()
    {
        if (aim == null) return;
        attack(damage, null, this);
        Sounds.÷¼÷Ã¹¥»÷.playWithPitch();
    }
}
