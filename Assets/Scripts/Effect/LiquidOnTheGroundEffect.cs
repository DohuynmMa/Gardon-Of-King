using UnityEngine;

public class LiquidOnTheGroundEffect : AreaEffect
{
    public EntityGroup parentEntityGroup;
    public BuffType givingBuffType = BuffType.none;
    public float givingBuffTime = 0;
    public bool friendTarget = false;
    public bool enemyTarget = true;
    public bool towerTarget = false;
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Entity")
        {
            if (other.GetComponent<Entity>() == null) return;
            var entity = other.GetComponent<Entity>();
            if (entity.entityState != EntityState.enable) return;
            parentEntityGroup = summonner.entityGroup;
            if ((friendTarget && entity.entityGroup == parentEntityGroup) || (enemyTarget && entity.entityGroup != parentEntityGroup))
            {
                if (BuffManager.Instance.hasBuff(entity, givingBuffType) || (!towerTarget && entity.hasParent)) return;
                BuffManager.Instance.addBuff(entity, givingBuffTime, givingBuffType);
            }
        }
    }
}
