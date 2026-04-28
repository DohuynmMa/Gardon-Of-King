using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.NetWork;
using Assets.Scripts.NetWork.Packet.Play.Client;
using Assets.Scripts.NetWork.Packet.Play.Server;
public class Shovel : MonoBehaviour
{
    public bool usingShovel;
    public GameObject lockObj;
    public GameObject deleteTag;
    public GameObject num1;
    public GameObject shovelButton;
    Vector2 oldButtonSize;
    public Entity willDeleteEntity;
    private void Awake()
    {
        usingShovel = false;
    }
    private void Start()
    {
        num1.SetActive(!Application.isMobilePlatform);
        oldButtonSize = shovelButton.GetComponent<RectTransform>().sizeDelta;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (DataManager.Instance.data.hasShovel && !DialogConsole.Instance.inTyping)
            {
                useOrCancelShovel();
            }
        }
        if (DataManager.Instance.data.hasShovel)
        {
            lockObj.SetActive(false);
            gameObject.SetActive(true);
            updatePos();
        }
        else
        {
            lockObj.SetActive(true);
            gameObject.SetActive(false);
        }
    }
    public void useOrCancelShovel()
    {
        if (DataManager.Instance.data.hasShovel)
        {
            bool usingWateringCan = false;
            if(HandManager.Instance.wateringCan != null)
            {
                usingWateringCan = HandManager.Instance.wateringCan.usingWateringCan;
            }
            if (!usingShovel && !HandManager.Instance.handingEntity && !usingWateringCan)
            {
                Sounds.使用铲子.play();
                usingShovel = true;
                shovelButton.GetComponent<RectTransform>().sizeDelta *= 1000;
            }
            else deleteEntity();
        }
    }
    public void updatePos()
    {
        if (usingShovel)
        {
            HandManager.Instance.shovelFollow();
        }
        else
        {
            transform.position = lockObj.transform.position;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Entity")
        {
            Entity entity = other.GetComponent<Entity>();
            if (entity.entityGroup == EntityGroup.enemy || entity.hasParent) return;
            if (!usingShovel) return;
            if (entity.moveSpeed == 0)//判断是不是植物
            {
                deleteTag.SetActive(true);
                willDeleteEntity = entity;
                Vector2 colliderPos2 = entity.boxCollider.bounds.center;
                Vector3 colliderPos3 = new Vector3(colliderPos2.x, colliderPos2.y, entity.transform.position.z);
                deleteTag.transform.position = colliderPos3;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Entity")
        {
            if (other.GetComponent<Entity>().moveSpeed == 0)//判断是不是植物
            {
                willDeleteEntity = null;
                deleteTag.SetActive(false);
            }
        }
    }
    public void deleteEntity()
    {
        if (willDeleteEntity != null)
        {
            //服务端玩家使用铲子
            if (GameManager.Instance.gameMode == GameMode.MultiPlayer && MultiGameManager.server != null)
            {
                NetworkServerService.getUserById(1).Send(new PlayServerUseShovel(willDeleteEntity.entityID));
                Sounds.种植物.play();
                if (willDeleteEntity.anim != null)
                {
                    willDeleteEntity.anim.SetBool("Die", true);
                }
                else
                {
                    willDeleteEntity.entityDie();
                }
                usingShovel = false;
                shovelButton.GetComponent<RectTransform>().sizeDelta = oldButtonSize;
                return;
            }
            //客户端玩家使用铲子
            if (GameManager.Instance.gameMode == GameMode.MultiPlayer && MultiGameManager.client != null)
            {
                MultiGameManager.client.Send(new PlayClientUseShovel(willDeleteEntity.entityID));
                usingShovel = false;
                shovelButton.GetComponent<RectTransform>().sizeDelta = oldButtonSize;
                return;
            }
            //单机使用铲子
            Sounds.种植物.play();
            if (willDeleteEntity.anim != null)
            {
                willDeleteEntity.anim.SetBool("Die", true);
            }
            else
            {
                willDeleteEntity.entityDie();
            }
        }
        usingShovel = false;
        shovelButton.GetComponent<RectTransform>().sizeDelta = oldButtonSize;
    }
}
