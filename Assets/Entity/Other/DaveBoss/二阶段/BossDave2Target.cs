using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDave2Target : MonoBehaviour
{
    public bool aimed = false;
    public Entity entity;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Entity" || collision.tag == "Home")
        {
            if (collision.GetComponent<Entity>() == null) return;
            Entity entity = collision.GetComponent<Entity>();
            if(entity.entityGroup != this.entity.entityGroup && entity == this.entity.aim)
            {
                aimed = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Entity" || other.tag == "Home")
        {
            aimed = false;
        }
    }
}
