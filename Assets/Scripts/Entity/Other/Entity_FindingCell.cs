using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_FindingCell : MonoBehaviour
{
    private Entity parentEntity;
    private void Start()
    {
        checkParent();
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Cell" && parentEntity.entityState == EntityState.disable)
        {
            parentEntity.cell = other.GetComponent<Cell>();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Cell" && parentEntity.entityState == EntityState.disable)
        {
            parentEntity.cell = null;
        }
    }
    private void checkParent()
    {
        var pe = transform.parent;
        while (true)
        {
            if (pe.GetComponent<Entity>() == null)
            {
                pe = pe.transform.parent;
            }
            else break;
        }
        parentEntity = pe.GetComponent<Entity>();
    }
}
