using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Entity_Hp_Icon : MonoBehaviour
{
    public Entity parentEntity;
    public RectTransform rectTransform;
    private void Start()
    {
        checkParent();
        if (parentEntity == null || ImageManager.Instance.hpBarIcon.Count == 0 || GetComponent<BossDave2>() != null || GetComponent<BossDave>() != null) return;
        if (parentEntity.transform.parent != null)//ÔÝ¶¨:·ÀÓùËþ
        {
            if(parentEntity.entityGroup == EntityGroup.friend)GetComponent<Image>().sprite = GetComponent<Image>().sprite = ImageManager.Instance.hpBarIcon[2];
            else GetComponent<Image>().sprite = GetComponent<Image>().sprite = ImageManager.Instance.hpBarIcon[3];
        }
        else if (parentEntity.tag == "Home")//·¿×Ó
        {
            if (parentEntity.entityGroup == EntityGroup.friend) GetComponent<Image>().sprite = GetComponent<Image>().sprite = ImageManager.Instance.hpBarIcon[4];
            else GetComponent<Image>().sprite = GetComponent<Image>().sprite = ImageManager.Instance.hpBarIcon[5];
        }
    }
    private void Update()
    {
        if (parentEntity == null || ImageManager.Instance.hpBarIcon.Count == 0 || GetComponent<BossDave2>() != null || GetComponent<BossDave>() != null) return;
        if (hasArmor())
        {
            if (parentEntity.entityGroup == EntityGroup.friend) GetComponent<Image>().sprite = ImageManager.Instance.hpBarIcon[6];
            else
            {
                GetComponent<Image>().sprite = ImageManager.Instance.hpBarIcon[parentEntity.golden ? 9 : 7];
            }
        }
        else
        {
            if (parentEntity.tag == "Home" || parentEntity.transform.parent != null) return;
            if (parentEntity.entityGroup == EntityGroup.friend) GetComponent<Image>().sprite = ImageManager.Instance.hpBarIcon[0];
            else GetComponent<Image>().sprite = ImageManager.Instance.hpBarIcon[parentEntity.golden ? 8 : 1];
        }
    }
    private bool hasArmor()
    {
        if (parentEntity == null) return false;
        return (parentEntity.GetComponent<ArmorZombie>() != null && parentEntity.GetComponent<ArmorZombie>().hasArmor);
    }
    private void checkParent()
    {
        GameObject pe = transform.parent.gameObject;
        while (true)
        {
            if (pe.GetComponent<Entity>() == null)
            {
                if (pe.transform.parent == null) return;
                pe = pe.transform.parent.gameObject;
            }
            else break;
        }
        parentEntity = pe.GetComponent<Entity>();
    }
}
