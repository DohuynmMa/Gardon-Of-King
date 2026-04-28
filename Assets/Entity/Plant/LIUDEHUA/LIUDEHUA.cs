using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LIUDEHUA : Plant
{
    public List<Sprite> iceImages;
    public GameObject ice;
    private bool hasIce = true;
    private float happyTimer;
    public override void Update()
    {
        base.Update();
        happyTimer += Time.deltaTime;
        if(happyTimer >= 2)
        {
            giveHappy();
            happyTimer = 0;
        }
        if (!hasIce) return;
        if(hitpoint > 23)
        {
            ice.GetComponent<SpriteRenderer>().sprite = iceImages[0];
        }
        else if (hitpoint > 16)
        {
            ice.GetComponent<SpriteRenderer>().sprite = iceImages[1];
        }
        else if(hitpoint > 10)
        {
            ice.GetComponent<SpriteRenderer>().sprite = iceImages[2];
        }
        else
        {
            lostIce();
        }
    }
    private void lostIce()
    {
        ice.GetComponent<SpriteRenderer>().sprite = null;
        hasIce = false;
        Musics.¹§Ï²·¢²Æ.play(true);
    }
    private void giveHappy()
    {
        if (hasIce) return;
        foreach(var e in Utils.findAllEntitiesByGroup(entityGroup))
        {
            BuffManager.Instance.addBuff(e, 2, BuffType.Rage);
        }
        Utils.summonCoin(this.getEntityBoxColliderPos(), Random.Range(1,3));
    }
}
