using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utils;
public enum CellState
{
    enable,
    disable
}
public class Cell : MonoBehaviour
{
    public int ID;
    public CellState cellState;
    public int cellArea;
    public Entity currentEntity;
    public bool towerCell = false;
    /// <summary>
    /// 判定是否可以放置
    /// </summary>
    public void OnUp()
    {
        if ((cellState == CellState.disable && !(HandManager.Instance.currentEntity is AllAreaZombie || HandManager.Instance.currentEntity.isAreaEffect())) || cellArea == 4)
        {
            print("cant place this to the cell!");
            return;
        }
        if ((HandManager.Instance.currentEntity is AllAreaZombie || HandManager.Instance.currentEntity.isAreaEffect()) && GameManager.Instance.specCellSet)
        {
            print("spec cell set");
            return;
        }
        HandManager.Instance.onCellMouseUp(this);
    }
    
    private void Start()
    {
        if (cellArea == 3) cellState = CellState.disable;
    }

    public bool addEntityByPacket(EntityType type, EntityGroup entityGroup,int entityID) // 收到网络包，放置实体
    {
        var entityPrefeb = HandManager.Instance.getEntityPrefeb(type);
        if (entityPrefeb == null)
        {
            throw new System.Exception("null entityprefeb");
        }
        //确定位置
        var pos = entityPosition(entityPrefeb);

        //实例化并且初始化实体
        var entity = Instantiate(entityPrefeb, pos, Quaternion.identity);
        entity.entityGroup = entityGroup;
        entity.entityID = entityID;
        entity.lifeTimer = 0;
        entity.hitpoint = entity.maxHitpoint;
        entity.addToCellEvent(this);
        //放置实体(packet)
        MultiGameManager.putEntityByPacket(entity);
        return true;
    }
    public bool addEntityByHand(Entity entity, EntityGroup entityGroup) // 玩家通过 HandManager 放置实体
    {
        if (currentEntity != null)
        {
            if (HandManager.Instance.currentEntity.isPlant())
            {
                print("has Entity");
                return false;
            }
        }
        addEntity(currentEntity = entity, entityGroup);
        for(var i = 0; i < HandManager.Instance.currentSummonEntityCount - 1; i ++)
        {
            var entityPrefeb = HandManager.Instance.getEntityPrefeb(entity.entityType);
            addEntityDirectly(entityPrefeb, entityGroup);
        }
        if (currentEntity.moveSpeed != 0) currentEntity = null;
        return true;
    }
    public bool addEntityDirectly(Entity entity, EntityGroup entityGroup, bool golden = false) // 直接放置实体(AI/COMMAND/OTHER)
    {
        if (currentEntity != null)
        {
            if (entity.isPlant())
            {
                print("has Entity(D)");
                return false;
            }
        }
        if (entity == null)
        {
            print("entity == null");
            return false;
        }
        entity.golden = golden;
        addEntity(currentEntity = entity, entityGroup);
        return true;
    }

    /// <summary>
    /// 实体的添加和初始化
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="entityGroup"></param>
    private void addEntity(Entity entity, EntityGroup entityGroup)
    {
        entity.entityGroup = entityGroup;
        if (!GameManager.Instance.zombiesComingSoundFlag && entity.isZombie())
        {
            GameManager.Instance.zombiesComingSoundFlag = true;
            Sounds.ZombiesAreComing.play();
        }
        entity.lifeTimer = 0;
        if (entity.golden)
        {
            entity.damage *= 2;
            entity.maxHitpoint *= 10;
        }

        entity.addToCellEvent(this);

        entity.hitpoint = entity.maxHitpoint;

        //根据模式设置属性,满足模式的需要
        entity.modeSet();

        //根据阵营翻转
        if (entityGroup == EntityGroup.enemy)
        {
            entity.transform.localScale = new Vector3(
                entity.transform.localScale.x * -1,
                entity.transform.localScale.y,
                entity.transform.localScale.z
            );
        }
        Sounds.种植物.playWithPitch(Random.Range(0.9f, 1.1f));
        entity.cell = this;
        entity.updateHpBarImage();
        var colliderPos3 = entityPosition(entity);
        if(!(entity is AllAreaZombie) && !(entity.isAreaEffect()))entity.transform.position = colliderPos3;
        entity.transitionToEnable();
        if (entity.moveSpeed == 0)
        {
            entity.lockPos = colliderPos3;
        }
    }

    public Vector3 entityPosition(Entity entity)
    {
        Vector2 colliderPos2 = GetComponent<BoxCollider2D>().bounds.center;
        Vector3 colliderPos3 = new Vector3(
     colliderPos2.x + entity.offsetX * (entity.GetComponent<BasicZombie>() == null ? 1 : 1.5f),
     colliderPos2.y + entity.offsetY * (entity.GetComponent<BasicZombie>() == null ? 1 : 0.5f),
     0);
        return colliderPos3;
    }

}
