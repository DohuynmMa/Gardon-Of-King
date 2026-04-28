using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public int entityID;
    public BuffType type;
    public float buffDuration;
    public Buff(BuffType type, float duration, int entityID)
    {
        this.entityID = entityID;
        this.type = type;
        this.buffDuration = duration;
    }
}
