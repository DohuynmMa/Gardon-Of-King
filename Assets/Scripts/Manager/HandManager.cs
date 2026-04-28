using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Utils;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.NetWork;
using Assets.Scripts.NetWork.Packet.Play.Client;
using Assets.Scripts.NetWork.Packet.Play.Server;
public class HandManager : MonoBehaviour
{
    public static HandManager Instance { get; private set; }

    public Entity currentEntity;
    public Card currentCard;

    public Shovel shovel;
    public WateringCan wateringCan;

    public bool handingEntity;
    public int currentSummonEntityCount = 1;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (Utils.scene == "GameMenu")
        {
            return;
        }
        clickUpdate();
        followCursor();
    }
    /// <summary>
    /// 根据Card的Type实例化实体
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    public bool addEntity(EntityType entityType)
    {
        if (currentEntity != null) return false;
        var entityPrefeb = getEntityPrefeb(entityType);
        if (entityPrefeb == null)
        {
            print("null entityprefeb");
            return false;
        }
        currentEntity = Instantiate(entityPrefeb, transform.position, Quaternion.identity);
        currentEntity.transitionToDisable();
        return true;
    }
    protected internal void stopAddEntity()
    {
        if (currentEntity == null) return;
        Destroy(this.currentEntity.gameObject);
        handingEntity = false;
        currentEntity = null;
        if (currentCard == null) return;
        currentCard.updateColor();
        currentCard = null;
    }
    public Entity getEntityPrefeb(EntityType entityType)
    {
        foreach (var entity in BridgeManager.Instance.allEntityPrefab)
        {
            if (entity.entityType == entityType) return entity;
        }
        return null;
    }
    private void followCursor()
    {
        if (currentEntity == null)
        {
            GameManager.Instance.cantPlace.SetActive(false);
            return;
        }
        else GameManager.Instance.cantPlace.SetActive(!(currentEntity is AllAreaZombie || currentEntity.isAreaEffect()));

        currentEntity.useCardEvent();

        if(currentEntity.deployShadow != null)
        {
            if (currentEntity.cell != null && (currentEntity.cell.currentEntity == null || !currentEntity.isPlant()) && (currentEntity.cell.cellState == CellState.enable || ((currentEntity is AllAreaZombie || currentEntity.isAreaEffect()) && currentEntity.cell.cellArea != 4)))
            {
                var colliderPos2 = currentEntity.cell.GetComponent<BoxCollider2D>().bounds.center;
                var colliderPos3 = new Vector3(colliderPos2.x, colliderPos2.y, 0);
                if (currentEntity.fixDeployShadowPos)
                {
                    colliderPos3.x += currentEntity.offsetX;
                    colliderPos3.y += currentEntity.offsetY;
                }
                currentEntity.deployShadow.transform.position = colliderPos3;
                currentEntity.deployShadow.transform.SetParent(null);
                currentEntity.deployShadow.SetActive(true);
            }
            else
            {
                currentEntity.deployShadow.transform.SetParent(currentEntity.deployShadow.transform);
                currentEntity.deployShadow.SetActive(false);
            }
        }
        else
        {
            print("null deployShadow");
        }

        moveToMouse(currentEntity);
    }
    public void shovelFollow()
    {
        if (shovel == null) return;
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        shovel.transform.position = mouseWorldPosition;
    }
    public void wateringCanFollow()
    {
        if (wateringCan == null) return;
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        wateringCan.transform.position = mouseWorldPosition;
    }
    /// <summary>
    /// 将实体放置到指定Cell上
    /// </summary>
    /// <param name="cell"></param>
    public void onCellMouseUp(Cell cell)
    {
        if (currentEntity == null || currentCard == null)
        {
            print("null entity or card");
            return;
        }
        //客户端放卡
        if (MultiGameManager.client != null && GameManager.Instance.gameMode == GameMode.MultiPlayer)
        {
            SunManager.Instance.changeSun(currentCard.needSunPoint * -1);
            currentCard.TransitionsToWaitingSun();
            if (CardManager.Instance.cardEnable.Contains(currentCard)) CardManager.Instance.transitionToWaiting(currentCard);
            currentEntity.deployShadow.SetActive(false);
            MultiGameManager.client.Send(new PlayClientUseCard(Utils.getRelativeCellId(cell.ID), currentCard.entityType,currentSummonEntityCount));
        }
        //服务端放卡
        else if (MultiGameManager.server != null && GameManager.Instance.gameMode == GameMode.MultiPlayer)
        {
            SunManager.Instance.changeSun(currentCard.needSunPoint * -1);
            currentCard.TransitionsToWaitingSun();
            if (CardManager.Instance.cardEnable.Contains(currentCard)) CardManager.Instance.transitionToWaiting(currentCard);
            currentEntity.deployShadow.SetActive(false);
            //因为这里是1v1 所以发送的对象id默认为1
            var entityId = Utils.createEntityId();
            var group = EntityGroup.friend;
            NetworkServerService.getUserById(1).Send(new PlayServerAddEntity(Utils.getRelativeCellId(cell.ID), entityId, currentCard.entityType, EntityGroup.enemy, currentSummonEntityCount));

            if (cell.currentEntity != null && currentCard.entityType.isPlant())
            {
                Debug.Log("有实体");
                return;
            }
            cell.addEntityByPacket(currentCard.entityType, group, entityId);
        }
        //单机游戏放卡
        else
        {
            bool isSuccess = cell.addEntityByHand(currentEntity, EntityGroup.friend);
            if (isSuccess)
            {
                currentEntity = null;
            }
            else
            {
                print("add entity failed");
            }
        }
    }
    /// <summary>
    /// Android/Windows 识别点击
    /// </summary>
    /// <returns></returns>
    public static bool onClick()
    {
        return Application.isMobilePlatform ? (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) : Input.GetMouseButtonDown(0);
    }
    public static bool onUp()
    {
        return Input.GetMouseButtonUp(0);
    }
    private void clickUpdate()
    {
        if (Utils.scene == "GameMenu") return;
        if (onClick() || onUp())
        {
            addOrStopAddingEntity();
        }
    }
    public void addOrStopAddingEntity()
    {
        //阳光不足
        if (currentCard != null)
        {
            if (currentCard.needSunPoint > SunManager.Instance.sunPoint)
            {
                stopAddEntity();
                print("not enough sun point");
                return;
            }
        }
        if (handingEntity)
        {
            if (currentEntity != null)
            {
                moveToMouse(currentEntity);
                DOVirtual.DelayedCall(0.005f, () =>
                {
                    currentEntity.cell.OnUp();
                    bool isSuccess = addEntity(currentCard.entityType);
                    if (isSuccess)
                    {
                        SunManager.Instance.changeSun(currentCard.needSunPoint * -1);
                        currentCard.TransitionsToWaitingSun();
                        if (CardManager.Instance.cardEnable.Contains(currentCard)) CardManager.Instance.transitionToWaiting(currentCard);
                    }
                    else
                    {
                        print("添加实体失败");
                    }
                    stopAddEntity();
                });
                return;
            }
        }
        stopAddEntity();
    }
    /// <summary>
    /// 把实体移动到鼠标
    /// </summary>
    /// <param name="entity"></param>
    public void moveToMouse(Entity entity)
    {
        if (entity == null) return;
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Application.isMobilePlatform ? Input.GetTouch(0).position : Input.mousePosition);
        mouseWorldPosition.z = 0; mouseWorldPosition.y -= entity.isAreaEffect() ? 0 : 0.3f;
        entity.transform.position = mouseWorldPosition;
    }
}
