using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Utils;

public static class SaveBuffHelper
{
    public static SavedBuff save(this Buff buff)
    {

        return new()
        {
            entityID = buff.entityID,
            buffType = buff.type,
            buffDuration = buff.buffDuration
        };
    }
}
[System.Serializable]
public class SavedBuff
{
    public int entityID;
    public BuffType buffType;
    public float buffDuration;
    public void load()
    {
        foreach (Entity entity in Utils.findAllEntities())
        {
            if (entity.oldInstanceID == entityID)
            {
                BuffManager.Instance.removeBuff(entity, buffType);
                BuffManager.Instance.addBuff(entity, buffDuration, buffType);
                //Debug.Log("added! " + buffType);
                return;
            }
            Debug.Log("null entity !");
        }
    }
    public void apply(Buff buff)
    {
        buff.entityID = entityID;
        buff.type = buffType;
        buff.buffDuration = buffDuration;
    }
}
