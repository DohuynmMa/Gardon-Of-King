using UnityEngine;
using Assets.Scripts.Utils;

public class LittleWolf : Plant
{
    private void addIceBuffToAllEnemyWhenDie()
    {
        Instantiate(Utils.findEffectByType(AreaEffectType.FrozenAll),transform.position,Quaternion.identity);
        foreach(var entity in Utils.findAllEntitiesByGroup(entityGroup == EntityGroup.enemy ? EntityGroup.friend : EntityGroup.enemy))
        {
            if (entity == null) continue;
            BuffManager.Instance.addBuff(entity, 5f, BuffType.IcePlus);
        }
    }
}
