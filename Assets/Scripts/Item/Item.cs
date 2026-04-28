using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool isGameWin;//是否导致游戏胜利?
    private void Update()
    {
        checkPos();
    }
    private void checkPos()
    {
        Vector3 pos = transform.position; 
        if(pos.y < -4.5f)
        {
            transform.position = new Vector3(pos.x,-4.5f,pos.z);
        }
        if(pos.y > 4f)
        {
            transform.position = new Vector3(pos.x,4f,pos.z);
        }
        if(pos.x < -7.5f)
        {
            transform.position = new Vector3(-7.5f, pos.y, pos.z);
        }
        if (pos.x > 8)
        {
            transform.position = new Vector3(8f, pos.y, pos.z);
        }
    }
    public virtual void clickEvent()
    {
        if (isGameWin)
        {
            Musics.胜利音乐.play(false);
            GameManager.Instance.changeScene("GameMenu", 5, GameMode.None, true);
        }
        destroyItem();
    }
    public virtual void destroyItem()
    {
        Destroy(gameObject);
    }
    private void OnMouseDown()
    {
        clickEvent();
    }
}
