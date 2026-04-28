using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AoeEffect : AreaEffect
{
    public float damage;
    public List<Entity> damagedEntity;

    public Sounds hitSound = Sounds.子弹打中普通;

    public BuffType addBuff;
    public float addBuffTime;
    public override void onSpawn()
    {
        base.onSpawn();
        hitSound.playWithPitch();
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Entity" || other.tag == "Home")
        {
            if (other.GetComponent<Entity>() == null) return;
            var entity = other.GetComponent<Entity>();
            if(entity == null) return;
            if (entity.entityGroup == summonner.entityGroup || entity.hitpoint <= 0 || entity.entityState != EntityState.enable || hasDamaged(entity)) return;
            if (entity is AllAreaZombie)
            {
                if (entity.GetComponent<MinerZombie>().inMining) return;
            }
            if (addBuff != BuffType.none) BuffManager.Instance.addBuff(entity, addBuffTime, addBuff);
            if (!damagedEntity.Contains(entity)) damagedEntity.Add(entity);
            entity.changeHitpoint(damage, summonner.bullet,summonner);
            hitEvent();
        }
    }
    public virtual void hitEvent()
    {

    }
    private bool hasDamaged(Entity entity)
    {
        foreach(Entity e in damagedEntity)
        {
            if (entity == e) return true;
        }
        return false;
    }
}
