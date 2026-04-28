using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;
public class FireballSpawnerEntity : AreaEffectEntity
{
    public override void transitionToEnable()
    {
        agent.enabled = true;
        anim.enabled = true;
        entityState = EntityState.enable;
        GetComponent<SpriteRenderer>().sprite = null;
        if (spawningEffectType == AreaEffectType.None)
        {
            print("no effect");
            return;
        }
        var bomb = Instantiate(Utils.findEffectByType(spawningEffectType), transform.position, Quaternion.identity).GetComponent<FireballEffect>();
        var gm = GameManager.Instance;
        bomb.parentEntity = entityGroup == EntityGroup.friend ? gm.home1 : gm.home2;
        spawnEffectSounds.play();
        deployShadow.GetComponent<SpriteRenderer>().sprite = null;  
        entityDie();
    }
    public override void addToCellEvent(Cell cell)
    {
        transform.position = cell.entityPosition(this);
    }
}
