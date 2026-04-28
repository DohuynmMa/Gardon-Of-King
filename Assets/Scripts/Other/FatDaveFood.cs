using Assets.Scripts.Utils;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
public enum FatDaveFoodType
{
    None,
    Pea,
    SnowPea,
    Melon,
    IceMelon,
    Cabbage,
    RottenFlesh
}

public class FatDaveFood : MonoBehaviour
{
    public FatDaveFoodType foodType = FatDaveFoodType.None;
    public AreaEffectType feedEffectType = AreaEffectType.None;
    public static FatDaveFood currentFood;
    //可以补充的饥饿值
    public float hunger = 0;
    //是否冰冻全场敌人
    public bool frozeAllEnemies = false;
    //是否在跟随鼠标
    public bool followingMouse = false;
    //投喂的戴夫
    public FatDave fatDave;
    private void Start()
    {
        //todo 弹簧声
        float flyRange = Random.Range(.1f, .4f);
        float flyHeight = Random.Range(.3f, .5f);
        transform.DOPath(new Vector3[] { new Vector3(transform.position.x - flyRange, transform.position.y + flyHeight, 0), new Vector3(transform.position.x - flyRange * 2, transform.position.y - flyHeight, 0) }, 1f, PathType.CatmullRom).SetEase(Ease.OutQuad);
        Invoke("DestroyFood", 20);
    }
    private void Update()
    {
        //跟随鼠标
        if (followingMouse)
        {
            var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Application.isMobilePlatform ? Input.GetTouch(0).position : Input.mousePosition);
            mouseWorldPosition.z = 2;
            transform.position = mouseWorldPosition;
        }
    }
    private void DestroyFood()
    {
        Destroy(gameObject);
    }
    private void feedDave()
    {
        if (fatDave == null) return;
        var accurateHunger = hunger;
        if (foodType == FatDaveFoodType.RottenFlesh) accurateHunger = hunger * (Random.Range(0, 2) == 1 ? 1f : -1f);
        fatDave?.beFed(accurateHunger);
        fatDave.openMouth = false;
        if(feedEffectType != AreaEffectType.None) Instantiate(Utils.findEffectByType(feedEffectType), transform.position, Quaternion.identity);
        if (frozeAllEnemies)
        {
            Instantiate(Utils.findEffectByType(AreaEffectType.FrozenAll), transform.position, Quaternion.identity);
            foreach (var e in Utils.findAllEntitiesByGroup(EntityGroup.enemy))
            {
                if(e == null) continue;
                if(e.hitpoint <= 0) continue;
                BuffManager.Instance.addBuff(e,2f,BuffType.IcePlus);
            }
        }
        Destroy(gameObject);
    }
    private void OnMouseDown()
    {
        followingMouse = !followingMouse;
        Sounds.tap.playWithPitch();
        if (currentFood != null)
        {
            currentFood.followingMouse = false;
        }
        currentFood = this;
    }
    private void OnTriggerStay(Collider collision)
    {
        if(collision.gameObject.GetComponent<FatDave>() != null)
        {
            fatDave = collision.gameObject.GetComponent<FatDave>();
            fatDave.openMouth = true;
            if (!followingMouse)
            {
                feedDave();
                fatDave?.mouthObj?.SetActive(false);
                //todo 音效
            }
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.GetComponent<FatDave>() != null)
        {
            fatDave = collision.gameObject.GetComponent<FatDave>();
            fatDave.openMouth = false;
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<FatDave>() != null)
        {
            fatDave = collision.gameObject.GetComponent<FatDave>();
            fatDave.openMouth = true;
        }
    }
}
