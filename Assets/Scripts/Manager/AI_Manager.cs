using System.Collections.Generic;
using UnityEngine;
using System.Linq; 
using UnityEngine.AI;
using Assets.Scripts.Utils;
public class AI_Manager : MonoBehaviour
{
    public static AI_Manager Instance { get; private set; }
    public int difficult;
    public List<Card> aiCardList;
    public List<Card> aiCardEnable;
    public List<Card> aiCardWaiting;

    public List<Cell> cellListRight;
    public List<Cell> cellListLeft;
    public Cell placeCell;

    private float refreshCardTimer;
    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (GameManager.Instance.gameMode == GameMode.SRCS)
        {
            refreshCardTimer += Time.deltaTime;
            if (refreshCardTimer >= 30)
            {
                refreshCardTimer = 0;
                refreshCardList();
            }
        }
        think();
    }
    /// <summary>
    /// 初始化AI
    /// </summary>
    public void setAI()
    {
        cellListRight.Clear();
        cellListRight = Utils.findAllCells(EntityGroup.enemy);
        cellListLeft = Utils.findAllCells(EntityGroup.friend);
        aiCardEnable.Clear();
        aiCardWaiting.Clear();
        int cardOrder = 5;
        for (int i = 0; i < 4; i++)
        {
            int temp = UnityEngine.Random.Range(0, aiCardList.Count);
            if (aiCardEnable.Contains(aiCardList[temp]))
            {
                i--;
            }
            else
            {
                aiCardList[temp].cardOrder = cardOrder;
                cardOrder++;
                aiCardEnable.Add(aiCardList[temp]);
            }
        }
        List<Card> cardWaitingTemp = aiCardList.Except(aiCardEnable).ToList();
        cardOrder = 1;
        aiCardWaiting = cardWaitingTemp.ToList();
        for (int i = 0; i < cardWaitingTemp.Count; i++)
        {
            Card card = cardWaitingTemp[i];
            card.cardOrder = cardOrder;
            aiCardWaiting[4 - cardOrder] = card;
            cardOrder++;
        }
    }
    /// <summary>
    /// AI思考
    /// </summary>
    private void think()
    {
        putCard();
    }
    /// <summary>
    /// AI放置卡牌(攻击或防御或支援)
    /// </summary>
    private void putCard()
    {
        //这里先找阳光最高的卡牌
        //todo:根据场上情况选择可放置卡牌
        var card = findMaxSunCard();

        if (card == null) return;
        if (card.needSunPoint > SunManager.Instance.enemySunPoint) return;

        //根据Type获取实体信息,不实例化
        var cardEntity = HandManager.Instance.getEntityPrefeb(card.entityType);

        //CellArea信息 : 4(非半场)/3(右半场靠右)/2(右半场靠左)/1(左半场)

        //先寻找随机格子,之后再根据卡牌类型重新寻找合适的格子,如果找不到则保持使用这个格子
        if (placeCell == null)
        {
            placeCell = cellListRight[Random.Range(0, cellListRight.Count)];
            return;
        }

        //向日葵不靠前
        if (card.entityType == EntityType.SunFlower && placeCell.cellArea != 3)
        {
            placeCell = cellListRight[Random.Range(0, cellListRight.Count)];
            return;
        }
        //坚果靠前
        else if (card.entityType == EntityType.WallNut && placeCell.cellArea != 2)
        {
            placeCell = cellListRight[Random.Range(0, cellListRight.Count)];
            return;
        }
        //AllArea僵尸放对面(矿工等)
        else if (cardEntity is AllAreaZombie)
        {
            placeCell = cellListLeft[Random.Range(0, cellListLeft.Count)];
        }
        //非AllArea僵尸靠前
        else if (card.entityType.isZombie())
        {
            if (placeCell.cellArea != 2 && !isAllCellsFull(2))
            {
                placeCell = cellListRight[Random.Range(0, cellListRight.Count)];
                return;
            }
        }
        //根据法术种类重新选择最佳格子(没有的话就保留上面的随机格子乱放(过牌))
        else if (card.entityType.isAreaEffect())
        {
            var allEntitiesEnemy = Utils.findAllEntitiesByGroup(EntityGroup.enemy);
            var allEntitiesFriend = Utils.findAllEntitiesByGroup(EntityGroup.friend);
            var allTowersAndHomes = Utils.findAllTowerAndHome();
            var effEntity = cardEntity.GetComponent<AreaEffectEntity>();
            Cell placingCell = null;

            var isHarmfulAreaEffect = Utils.isHarmfulAreaEffectSpawner(card.entityType);

            var canAffectTowersAndHomes = 
                   effEntity.spawningEffectType == AreaEffectType.Fireball
                || effEntity.spawningEffectType == AreaEffectType.RageLiquid
                || effEntity.spawningEffectType == AreaEffectType.LeavingSnow;

            //优先攻击实体
            if (((isHarmfulAreaEffect && allEntitiesFriend.Count == 0) || (!isHarmfulAreaEffect && allEntitiesEnemy.Count == 0)) && canAffectTowersAndHomes)
            {
                foreach (var o in allTowersAndHomes)
                {
                    if (o.tag == "Tower")
                    {
                        var tower = o.GetComponent<Tower>();
                        if (tower.entityGroup == (isHarmfulAreaEffect ? EntityGroup.friend : EntityGroup.enemy))
                        {
                            placingCell = tower.attachEntity.cell;
                            break;
                        }
                    }
                    else if (o.tag == "Home")
                    {
                        var home = o.GetComponent<Home>();
                        if (home.entityGroup == (isHarmfulAreaEffect ? EntityGroup.friend : EntityGroup.enemy))
                        {
                            placingCell = Utils.findCellById(isHarmfulAreaEffect ? 10 : 44);
                            break;
                        }
                    }
                }
            }
            else
            {
                List<Entity> entities = null;
                entities = isHarmfulAreaEffect ? allEntitiesFriend : allEntitiesEnemy;
                if(entities.Count != 0)
                {
                    foreach (var e in entities)
                    {
                        if (e == null || e.hitpoint <= 0 || e.isAreaEffect() || e.entityState == EntityState.disable) continue;
                        placingCell = e.cell;
                        break;
                    }
                }
            }
            if (placingCell != null) placeCell = placingCell;
        }
        //判定所选卡牌是否可以放在当前格子
        if (placeCell.currentEntity != null && cardEntity.isPlant())
        {
            placeCell = null;
            return;
        }

        //使用卡牌
        useCard(card);
    }
    /// <summary>
    /// AI放置卡牌
    /// </summary>
    /// <param name="card"></param>
    private void useCard(Card card)
    {
        if (card == null || placeCell == null) return;

        var entityPrefeb = HandManager.Instance.getEntityPrefeb(card.entityType);
        if (entityPrefeb == null) return;

        NavMeshHit hit;
        Entity currentEntity;

        //实例化实体
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            currentEntity = Instantiate(entityPrefeb, hit.position, Quaternion.identity);
        }
        else
        {
            currentEntity = null;
            print("null Nav");
            return;
        }

        bool isGolden = false;

        //无尽模式概率GOLDEN
        if (Random.Range(0, 31 - (difficult - 10) * 5) == 1 && GameManager.Instance.gameMode == GameMode.SRCS)
        {
            print("golden!!!");
            isGolden = true;
        }

        var isSuccess = placeCell.addEntityDirectly(currentEntity, EntityGroup.enemy, isGolden);

        //放置完毕,扣除AI的阳光数量,并且将卡牌放到AI牌组等待区域
        if (isSuccess)
        {
            placeCell = null;
            SunManager.Instance.changeEnemySun(card.needSunPoint * -1);
            allCardOrderChange();
            transitionToWaiting(card);
        }
        //不可放置,销毁实体预制体
        else currentEntity.entityDie();
    }
    /// <summary>
    /// AI洗牌
    /// </summary>
    private void allCardOrderChange()
    {
        transitionToEnable(aiCardWaiting[0]);
    }
    /// <summary>
    /// 将卡牌放在等待区
    /// </summary>
    /// <param name="card"></param>
    public void transitionToWaiting(Card card)
    {
        if (card == null) return;
        aiCardWaiting.Add(card);
        aiCardEnable.Remove(card);
    }
    /// <summary>
    /// 将卡牌放在非等待区
    /// </summary>
    /// <param name="card"></param>
    public void transitionToEnable(Card card)
    {
        if (card == null) return;
        aiCardWaiting.Remove(card);
        aiCardEnable.Add(card);
    }
    /// <summary>
    /// 寻找阳光最高的卡牌
    /// </summary>
    /// <returns></returns>
    private Card findMaxSunCard()
    {
        float maxSun = 0;
        Card maxCard = null;
        foreach(var card in aiCardEnable)
        {
            //如果可放置卡槽有僵尸卡,优先放僵尸
            if (!enableAllBuilding())
            {
                if (card.entityType.isPlant()) continue;
            }
            //判断阳光是否充足
            if (card.needSunPoint > maxSun)
            {
                maxSun = card.needSunPoint;
                maxCard = card;
            }
            return maxCard;
        }
        return null;
    }
    /// <summary>
    /// 可放置卡槽的卡牌是否全是Plant
    /// </summary>
    /// <returns></returns>
    private bool enableAllBuilding()
    {
        foreach (var card in aiCardEnable)
        {
            if (HandManager.Instance.getEntityPrefeb(card.entityType).moveSpeed != 0)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// AI增加可放置卡牌到自己卡牌背包中(升级难度)(无尽限定)
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    public Card aiFindCard(EntityType entityType)
    {
        var allCards = GameManager.Instance.aiCardTotalBagSRCS;
        foreach (var card in allCards)
        {
            if (card.entityType == entityType) return card;
        }
        return null;
    }
    /// <summary>
    /// 该区域所有格子是否都被植物占据
    /// </summary>
    /// <param name="cellArea"></param>
    /// <returns></returns>
    private bool isAllCellsFull(int cellArea)
    {
        foreach (var cell in Utils.findAllCellsByArea(cellArea))
        {
            if (cell == null) continue;
            if (cell.cellArea != cellArea) continue;
            if (cell.currentEntity == null) return false;
        }
        return true;
    }
    #region 无尽
    /// <summary>
    /// AI随着等级更新可放置卡牌 无尽限定
    /// </summary>
    public void updateCardBagWithLevel()
    {
        int nowLevel = DataManager.Instance.data.towerLevel;

        if (nowLevel > 1)//lv2
        {
            aiAddZombieCardSRCS(EntityType.ConeZombie);
            aiAddZombieCardSRCS(EntityType.BucketZombie);
            aiAddZombieCardSRCS(EntityType.NewYearZombie);
        }
        if (nowLevel > 2)//lv3
        {
            aiAddZombieCardSRCS(EntityType.Gargantuar);
        }
        if (nowLevel > 3)//lv4
        {
            aiAddZombieCardSRCS(EntityType.GoblinGargantuar);
            aiAddZombieCardSRCS(EntityType.GoblinImp);
        }
        if (nowLevel > 4)//lv5
        {
            aiAddZombieCardSRCS(EntityType.MinerZombie);
            aiAddZombieCardSRCS(EntityType.PaperZombie);
            aiAddZombieCardSRCS(EntityType.PekkaImp);
        }
        if (nowLevel > 5)//lv6
        {
            aiAddZombieCardSRCS(EntityType.MinerZombie2);
            aiAddZombieCardSRCS(EntityType.MakeupZombie);
            aiAddZombieCardSRCS(EntityType.SkeletonTomb);
        }
    }
    /// <summary>
    /// AI随着等级更新难度(放置速度) 无尽限定
    /// </summary>
    public void updateDifficult()
    {
        float score = DataManager.Instance.data.scoreSRCS;

        if (score >= 1000)
        {
            difficult = 11;
        }
        if (score >= 10000)
        {
            difficult = 12;
        }
        if (score >= 100000)
        {
            difficult = 13;
        }
        if (score >= 10000000)
        {
            difficult = 14;
        }
    }
    /// <summary>
    /// 根据TYPE在AI背包加入卡牌
    /// </summary>
    /// <param name="type"></param>
    private void aiAddZombieCardSRCS(EntityType type)
    {
        var gm = GameManager.Instance;
        Card card = aiFindCard(type);
        if (card != null)
        {
            if (!gm.aiCardBagSRCS.Contains(card)) gm.aiCardBagSRCS.Add(card);
        }
    }
    /// <summary>
    /// AI刷新自己的卡组 无尽限定
    /// </summary>
    public void refreshCardList()
    {
        updateCardBagWithLevel();
        updateDifficult();
        aiCardEnable.Clear();
        aiCardWaiting.Clear();
        aiCardList.Clear();
        for (int i = 0; i < 8; i++)
        {
            int temp = UnityEngine.Random.Range(0, GameManager.Instance.aiCardBagSRCS.Count);
            if (aiCardList.Contains(GameManager.Instance.aiCardBagSRCS[temp]))
            {
                i--;
            }
            else
            {
                aiCardList.Add(GameManager.Instance.aiCardBagSRCS[temp]);
            }
        }
        int cardOrder = 5;
        for (int i = 0; i < 4; i++)
        {
            int temp = UnityEngine.Random.Range(0, aiCardList.Count);
            if (aiCardEnable.Contains(aiCardList[temp]))
            {
                i--;
            }
            else
            {
                aiCardList[temp].cardOrder = cardOrder;
                cardOrder++;
                aiCardEnable.Add(aiCardList[temp]);
            }
        }
        var cardWaitingTemp = aiCardList.Except(aiCardEnable).ToList();
        cardOrder = 1;
        aiCardWaiting = cardWaitingTemp.ToList();
        for (int i = 0; i < cardWaitingTemp.Count; i++)
        {
            var card = cardWaitingTemp[i];
            card.cardOrder = cardOrder;
            aiCardWaiting[4 - cardOrder] = card;
            cardOrder++;
        }
    }
    #endregion
}
