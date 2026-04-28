using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home_Target : MonoBehaviour
{
    private Entity target;
    public List<Sprite> aimSprite; //0: blue 1: red
    public bool isAimed = false;
    public void hide()
    {
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
        isAimed = false;
    }
    public void appear()
    {
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 200);
    }
    public void moveTo(Entity targetObject)
    {
        Vector2 colliderPos2 = targetObject.GetComponent<BoxCollider2D>().bounds.center;
        Vector3 aimPos = new Vector3(colliderPos2.x, colliderPos2.y, 0);
        target = targetObject;
        if(Time.timeScale != 0) transform.position = Vector3.MoveTowards(transform.position, aimPos, 0.5f * Vector3.Distance(aimPos, transform.position) * Time.deltaTime);
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (target == null || collision.tag != "Entity") return;

        if (collision.GetComponent<Entity>() == target && !isAimed) isAimed = true;
    }
}
