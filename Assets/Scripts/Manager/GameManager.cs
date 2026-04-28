using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Assets.Scripts.Utils;
using System;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.NetWork.Packet.Play.Server;
using Assets.Scripts.NetWork.Packet.Play.Client;
using Assets.Scripts.NetWork;
using Unity.VisualScripting;
public enum GameMode
{
    None,
    Normal,
    SRCS,
    MultiPlayer,
    MiniGame_TETF,
    MiniGame_WGWE,
    MiniGame_BS,
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public DialogGamingMenu gaming { get => DialogGamingMenu.Instance; }
    public BridgeManager bm { get =>  BridgeManager.Instance; }
    public GameMode gameMode;
    public float startSunPoint;
    public Tower tower;//prefab
    public Home home;//prefab
    public int enemyTowerCount;
    public int friendTowerCount;
    public Tower tower1, tower2, tower3, tower4;
    public Home home1, home2;

    public List<HomeNpc> homeNpcPrefabList;

    public GameObject cellListTop;
    public GameObject cellListBottom;
    public GameObject cantPlace;

    //卡槽内部obj
    Vector3 oldAPos;

    public int chapter;//当前章节
    public int level;//当前关数
    public BackGround BG;//当前背景
    public GameObject BossDave;//boss1
    public FatDave fatDavePrefab;

    //难度AI卡牌
    public List<Card> aiCardTotalBagSRCS;//尸如潮水AI总卡包,升级后就从这里拿新僵尸
    public List<Card> aiCardBagSRCS;//尸如潮水AI卡包

    public List<Card> aiCardListC1L1;//1-1
    public List<Card> aiCardListC1L2;//1-2
    public List<Card> aiCardListC1L3;//1-3
    public List<Card> aiCardListC1L4;//1-4
    public List<Card> aiCardListC1L5;//1-5
    public List<Card> aiCardListC1L6;//1-6
    public List<Card> aiCardListC1L7;//1-7
    public List<Card> aiCardListC1L8;//1-8
    public List<Card> aiCardListC2L2;//2-2
    public List<Card> aiCardListC2L3;//2-3
    public List<Card> aiCardListC2L4;//2-4

    public List<Card> aiCardListBS;//MINIGAME-BS
    public List<Card> playerCardListBS;
    private float autoSaveTimerSRCS;
    public bool inGame = false; // 当前是否在游戏中，特指“准备 好 开始”之后
    public bool specCellSet = false;// 是否根据关卡情况用新的cell设置,比如矿工无法全屏放置.
    internal bool zombiesComingSoundFlag = false;
    public int enemyCount;
    public static float[] speeds = { 0.5f, 1.0f, 1.5f, 2.0f };
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if(gaming.loseUI != null) gaming.loseUI.SetActive(false);
        oldAPos = gaming.cardAreaUI.GetComponent<RectTransform>().localPosition;
        gameMode = bm.gameMode;
    }

    private void Update()
    {
        if (gameMode == GameMode.SRCS && inGame)
        {
            autoSaveTimerSRCS += Time.deltaTime;
            if (autoSaveTimerSRCS >= 1)
            {
                saveSRCS();
                autoSaveTimerSRCS = 0;
            }
            扭曲程度Update();
        }
        if (Input.GetKeyDown(KeyCode.Space) && inGame) //暂停/继续
        {
            if (DialogConsole.Instance.inTyping) return;
            if (gaming.pauseUI.activeInHierarchy)
            {
                gameContinue();
            }
            else gamePause();
        }
        if (Input.GetKeyDown(KeyCode.Z) && inGame) //更改speed
        {
            if (DialogConsole.Instance.inTyping) return;
            changeGameSpeed();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && inGame && TestManager.Instance.按3时停) //更改speed
        {
            if (DialogConsole.Instance.inTyping) return;
            stopTheTime();
        }
        if (Input.GetKeyDown(KeyCode.I) && inGame) //显示/关闭血条
        {
            if (DialogConsole.Instance.inTyping) return;
            DataManager.Instance.data.showHpBar = !DataManager.Instance.data.showHpBar;
            DataManager.Instance.savePlayerData();
        }
        if (gameMode == GameMode.SRCS)
        {
            updateEnemyCount();
        }
        setCardAreaUIWithScreen();
    }
    /// <summary>
    /// 初始化防御塔
    /// </summary>
    /// <param name="chapter"></param>
    /// <param name="level"></param>
    public void setTower(int chapter, int level)
    {
        var entityType = DataManager.Instance.data.towerEntity;
        var homeNpcType = DataManager.Instance.data.homeNpc;
        if (gameMode == GameMode.Normal)
        {
            enemyTowerCount = 2;
            friendTowerCount = 2;

            var enemyHomeNpcType = HomeNpcType.none;

            var enemyTowerPlantType = EntityType.PeaShooter;

            if (chapter == 1 && level >= 5 && level != 8)//设置敌方守门NPC-第一章
            {
                enemyHomeNpcType = HomeNpcType.Dave;
            }

            if(chapter == 99 && level == 1)//设置测试关卡
            {
                enemyHomeNpcType = TestManager.Instance.测试关卡敌方守护者;
                enemyTowerPlantType = TestManager.Instance.测试关卡防御塔植物;
                if (TestManager.Instance.videoMode)
                {
                    enemyTowerCount = 0;
                    friendTowerCount = 0;
                    home1 = this.newHome(new(-7.9f, -2.27f, 0), EntityGroup.friend, HomeNpcType.none, max => max * 999999);
                    home2 = this.newHome(new(8.04f, -2.27f, 0), EntityGroup.enemy, HomeNpcType.none, max => max * 999999);
                    return;
                }
            }

            tower1 = this.newTower(1, new(-4.61f, 0.84f, 0), EntityGroup.friend, entityType, max => max, Utils.inNight());
            tower2 = this.newTower(2, new(-4.61f, -2.8f, 0), EntityGroup.friend, entityType, max => max, Utils.inNight());
            tower3 = this.newTower(3, new(4.60f, -2.80f, 0), EntityGroup.enemy, enemyTowerPlantType, max => max, Utils.inNight());
            tower4 = this.newTower(4, new(4.60f, 0.84f, 0), EntityGroup.enemy, enemyTowerPlantType, max => max, Utils.inNight());
            home1 = this.newHome(new(-7.9f, -2.27f, 0), EntityGroup.friend, homeNpcType);
            home2 = this.newHome(new(8.04f, -2.27f, 0), EntityGroup.enemy, enemyHomeNpcType);
        }
        else if (gameMode == GameMode.SRCS)
        {
            if (DataManager.Instance.data.havePlayedSCRS)
            {
                loadSRCSTowers();
            }
            else
            {
                tower1 = this.newTower(1, new(-4.61f, 0.84f, 0), EntityGroup.friend, entityType,max => max * 3, Utils.inNight());
                tower2 = this.newTower(2, new(-4.61f, -2.8f, 0), EntityGroup.friend, entityType,max => max * 3, Utils.inNight());

                enemyTowerCount = 2;
                friendTowerCount = 2;

                home1 = this.newHome(new(-7.9f, -2.27f, 0), EntityGroup.friend, homeNpcType,max => max * 3);
                home2 = this.newHome(new(8.04f, -2.27f, 0), EntityGroup.enemy, HomeNpcType.none, max => max * 999999);
            }
        }
        else if (gameMode == GameMode.MultiPlayer && MultiGameManager.server != null)
        {
            //服务器负责放塔

            tower1 = this.newTower(1, new(-4.61f, 0.84f, 0), EntityGroup.friend, entityType, max => max, Utils.inNight());
            tower2 = this.newTower(2, new(-4.61f, -2.8f, 0), EntityGroup.friend, entityType, max => max, Utils.inNight());
            //获取user id为1(对面)的防御塔植物
            var type = NetworkServerService.getPlayerInfoById(1).towerEntity;
            tower3 = this.newTower(3, new(4.60f, -2.80f, 0), EntityGroup.enemy, type, max => max, Utils.inNight());
            tower4 = this.newTower(4, new(4.60f, 0.84f, 0), EntityGroup.enemy, type, max => max, Utils.inNight());

            enemyTowerCount = 2;
            friendTowerCount = 2;

            var homeType = NetworkServerService.getPlayerInfoById(1).homeNpc;
            home1 = this.newHome(new(-7.9f, -2.27f, 0), EntityGroup.friend, homeNpcType);
            home2 = this.newHome(new(8.04f, -2.27f, 0), EntityGroup.enemy, homeType);
            NetworkServerService.getUserById(1).Send(new PlayServerSetTower(entityType, homeNpcType));
        }
        else if (gameMode.isMiniGame())
        {
            var t3Type = EntityType.PeaShooter;
            var t4Type = EntityType.PeaShooter;
            var h2Type = HomeNpcType.none;
            switch (gameMode)
            {
                case GameMode.MiniGame_TETF:
                    t3Type = EntityType.GatlingPeaShooter;
                    t4Type = EntityType.SnowPeaShooter;
                    h2Type = HomeNpcType.Dave;
                    break;
                case GameMode.MiniGame_WGWE:
                    t3Type = EntityType.Cornpult;
                    t4Type = EntityType.IceMelon;
                    h2Type = HomeNpcType.Dave;
                    break;
                case GameMode.MiniGame_BS:
                    Instantiate(fatDavePrefab);
                    break;
            }
            tower1 = this.newTower(1, new(-4.61f, 0.84f, 0), EntityGroup.friend, entityType, max => max, Utils.inNight());
            tower2 = this.newTower(2, new(-4.61f, -2.8f, 0), EntityGroup.friend, entityType, max => max, Utils.inNight());
            tower3 = this.newTower(3, new(4.60f, -2.80f, 0), EntityGroup.enemy, t3Type, max => max, Utils.inNight());
            tower4 = this.newTower(4, new(4.60f, 0.84f, 0), EntityGroup.enemy, t4Type, max => max, Utils.inNight());

            enemyTowerCount = 2;
            friendTowerCount = 2;

            home1 = this.newHome(new(-7.9f, -2.27f, 0), EntityGroup.friend, homeNpcType);
            home2 = this.newHome(new(8.04f, -2.27f, 0), EntityGroup.enemy, h2Type);
        }
    }
    /// <summary>
    /// 初始化阳光
    /// </summary>
    private void setSun()
    {
        if (gameMode == GameMode.SRCS && DataManager.Instance.data.havePlayedSCRS)
        {
            SunManager.Instance.sunPoint = DataManager.Instance.data.sunPointSRCS;
        }
        else SunManager.Instance.sunPoint = startSunPoint;
        SunManager.Instance.enemySunPoint = 0;
    }
    /// <summary>
    /// 初始化格子
    /// </summary>
    /// <param name="cellList"></param>
    /// <param name="set"></param>
    private void setCell(GameObject cellList, bool set)
    {
        for (int i = 0; i < cellList.transform.childCount; i++)
        {
            Cell cell = cellList.transform.GetChild(i).gameObject.GetComponent<Cell>();
            if (set) cell.cellState = CellState.enable;
            else cell.cellState = CellState.disable;
        }
    }
    /// <summary>
    /// 更新防御塔状态
    /// </summary>
    public void towerUpdate()
    {
        if (gameMode == GameMode.Normal || gameMode == GameMode.MultiPlayer || gameMode.isMiniGame())
        {
            if (enemyTowerCount < 2)
            {
                if (tower3 == null && tower4 == null)
                {
                    setCell(cellListTop, true);
                    setCell(cellListBottom, true);
                    cantPlace.GetComponent<SpriteRenderer>().sprite = ImageManager.Instance.cantPlace[3];
                }
                if (tower4 == null && tower3 != null)
                {
                    setCell(cellListTop, true);
                    cantPlace.GetComponent<SpriteRenderer>().sprite = ImageManager.Instance.cantPlace[1];
                }
                if (tower3 == null && tower4 != null)
                {
                    setCell(cellListBottom, true);
                    cantPlace.GetComponent<SpriteRenderer>().sprite = ImageManager.Instance.cantPlace[2];
                }
            }
            else cantPlace.GetComponent<SpriteRenderer>().sprite = ImageManager.Instance.cantPlace[0];
        }
    }
    /// <summary>
    /// 游戏结束
    /// </summary>
    /// <param name="winner"></param>
    /// <param name="reveivePacket"></param>
    public void gameOver(EntityGroup winner,bool reveivePacket = false)
    {
        inGame = false;
        CardManager.Instance.cardEnable.Clear();
        CardManager.Instance.cardWaiting.Clear();
        AI_Manager.Instance.aiCardEnable.Clear();
        AI_Manager.Instance.aiCardWaiting.Clear();
        enemyTowerCount = 0;
        friendTowerCount = 0;
        SunManager.Instance.enabled = false;
        AI_Manager.Instance.enabled = false;
        if (gameMode == GameMode.Normal || gameMode.isMiniGame()) // 闯关模式或迷你模式游戏结束
        {
            var allEntity = GameObject.FindGameObjectsWithTag("Entity");
            var allTower = GameObject.FindGameObjectsWithTag("Tower");
            var allHome = GameObject.FindGameObjectsWithTag("Home");
            var allObjects = allEntity.Concat(allTower).ToArray().Concat(allHome).ToArray();
            foreach (var o in allObjects)
            {
                Destroy(o);
            }
            gaming.cardAreaUI.SetActive(false);
            print("gameOver");

            // 发放奖励,游戏结束
            if (winner == EntityGroup.friend) win();
            else lose();
        }
        if (gameMode == GameMode.SRCS) //尸如潮水模式结束
        {
            var allEntity = GameObject.FindGameObjectsWithTag("Entity");
            var allTower = GameObject.FindGameObjectsWithTag("Tower");
            var allHome = GameObject.FindGameObjectsWithTag("Home");
            var allObjects = allEntity.Concat(allTower).ToArray().Concat(allHome).ToArray();
            foreach (var entity in allObjects)
            {
                Destroy(entity);
            }
            gaming.cardAreaUI.SetActive(false);
            var dat = DataManager.Instance.data;
            if (dat.scoreSRCS > dat.maxScoreSRCS) dat.maxScoreSRCS = dat.scoreSRCS;
            dat.scoreSRCS = 0;
            lose();
        }
        if (gameMode == GameMode.MultiPlayer) // 对战模式游戏结束
        {
            var allEntity = GameObject.FindGameObjectsWithTag("Entity");
            var allTower = GameObject.FindGameObjectsWithTag("Tower");
            var allHome = GameObject.FindGameObjectsWithTag("Home");
            var allObjects = allEntity.Concat(allTower).ToArray().Concat(allHome).ToArray();
            foreach (var entity in allObjects)
            {
                Destroy(entity);
            }
            print("对战结束");
            gaming.cardAreaUI.SetActive(false);
            MultiGameManager.myUserId = 0;
            //根据阵营来相对地判断出胜者
            var winnerName = winner == EntityGroup.friend ? DataManager.Instance.data.playerName : MultiGameManager.enemyName;
            Utils.showDialog("游戏结束\n胜者:" + winnerName, 18.1f, Color.white, "退 出", 20f, Color.white);
            (winner == EntityGroup.friend ? Musics.胜利音乐 : Musics.失败音乐).play(false);
            //懒得写服务端的包了 就让客户端决定吧:)
            if (MultiGameManager.server != null)
            {
                if(!reveivePacket) NetworkServerService.getUserById(1).Send(new PlayServerGameOver(winnerName));
                MultiGameManager.server.Stop();
                MultiGameManager.server = null;
            }
            if (MultiGameManager.client != null)
            {
                if (!reveivePacket) MultiGameManager.client.Send(new PlayClientGameOver(winnerName));
                MultiGameManager.client.CloseAsync();
                MultiGameManager.client = null;
            }
        }
        level = 0;
        chapter = 0;
    }
    /// <summary>
    /// 游戏开始
    /// </summary>
    /// <param name="chapter"></param>
    /// <param name="level"></param>
    public void gameStart(int chapter,int level)
    {
        Time.timeScale = gameMode == GameMode.MultiPlayer ? 1f : DataManager.Instance.data.gameSpeed;
        CardManager.Instance.checkCard();
        if (gameMode == GameMode.Normal)//闯关
        {
            this.chapter = chapter;
            this.level = level;

            //初始化格子
            setCell(cellListTop, false);
            setCell(cellListBottom, false);

            //初始化阳光
            setSun();

            gaming.loadGamingUI();
            DialogKingShop.Instance.unloadGamingUI();
            SunManager.Instance.enabled = true;
            AI_Manager.Instance.enabled = true;
            CardManager.Instance.gameStart();
            //难度设置
            if (chapter == 1)
            {
                switch (level)
                {
                    case 1:
                        AI_Manager.Instance.aiCardList = aiCardListC1L1;
                        AI_Manager.Instance.difficult = 99;
                        break;
                    case 2:
                        AI_Manager.Instance.aiCardList = aiCardListC1L2;
                        AI_Manager.Instance.difficult = 0;
                        break;
                    case 3:
                        AI_Manager.Instance.aiCardList = aiCardListC1L3;
                        AI_Manager.Instance.difficult = 1;
                        break;
                    case 4:
                        AI_Manager.Instance.aiCardList = aiCardListC1L4;
                        AI_Manager.Instance.difficult = 2;
                        break;
                    case 5:
                        AI_Manager.Instance.aiCardList = aiCardListC1L5;
                        AI_Manager.Instance.difficult = 2;
                        break;
                    case 6:
                        AI_Manager.Instance.aiCardList = aiCardListC1L6;
                        AI_Manager.Instance.difficult = 3;
                        break;
                    case 7:
                        AI_Manager.Instance.aiCardList = aiCardListC1L7;
                        AI_Manager.Instance.difficult = 3;
                        break;
                    case 8:
                        AI_Manager.Instance.aiCardList = aiCardListC1L8;
                        AI_Manager.Instance.difficult = 1;
                        home2.GetComponent<Entity>().maxHitpoint = 999999;
                        home2.GetComponent<Entity>().hitpoint = home2.GetComponent<Entity>().maxHitpoint;
                        SunManager.Instance.addTime = 2;
                        break;
                }
            }
            else if(chapter == 2)
            {
                switch (level)
                {
                    case 1:
                        AI_Manager.Instance.aiCardList = aiCardListC2L2;
                        AI_Manager.Instance.difficult = 99;
                        break;
                    case 2:
                        AI_Manager.Instance.aiCardList = aiCardListC2L2;
                        AI_Manager.Instance.difficult = 1;
                        break;
                    case 3:
                        AI_Manager.Instance.aiCardList = aiCardListC2L3;
                        AI_Manager.Instance.difficult = 2;
                        break;
                    case 4:
                        AI_Manager.Instance.aiCardList = aiCardListC2L4;
                        AI_Manager.Instance.difficult = 2;
                        break;
                }
            }
            else if (chapter == 99)//测试关卡
            {
                switch (level)
                {
                    case 1:
                        if (TestManager.Instance.videoMode)
                        {
                            inGame = true;
                            gaming.cardAreaUI.SetActive(false);
                            return;
                        }
                        AI_Manager.Instance.aiCardList = TestManager.Instance.测试关卡AI卡组;
                        AI_Manager.Instance.difficult = TestManager.Instance.测试关卡难度系数;
                        break;
                }
            }
            AI_Manager.Instance.setAI();
        }
        else if (gameMode == GameMode.SRCS)// 尸如潮水模式开始 (初次对话结束)
        {
            this.chapter = chapter;
            this.level = level;
            if (DataManager.Instance.data.havePlayedSCRS)
            {
                loadSRCSEntities();
                loadSRCSBuffs();
            }
            DialogKingShop.Instance.loadGamingUI();
            gaming.loadGamingUI();

            //初始化格子
            setCell(cellListTop, true);
            setCell(cellListBottom, true);

            //特殊化放置区域
            cantPlace.GetComponent<SpriteRenderer>().sprite = ImageManager.Instance.cantPlace[3];

            //初始化阳光
            setSun();

            //默认激活国王塔
            if(DataManager.Instance.data.homeNpc != HomeNpcType.none) 
                home1.transitionToActivate();

            SunManager.Instance.enabled = true;
            SunManager.Instance.addTime = 3;
            CardManager.Instance.gameStart();
            AI_Manager.Instance.enabled = true;
            AI_Manager.Instance.refreshCardList();

            AI_Manager.Instance.difficult = 10;
            AI_Manager.Instance.setAI();

            DataManager.Instance.data.havePlayedSCRS = true;

            saveSRCS();
        }
        else if(gameMode == GameMode.MultiPlayer)
        {
            AI_Manager.Instance.enabled = false;
            setCell(cellListTop, false);
            setCell(cellListBottom, false);
            gaming.loadGamingUI();
            DialogKingShop.Instance.unloadGamingUI();
            SunManager.Instance.enabled = true;
            setSun();

            CardManager.Instance.gameStart();
        }
        else if (gameMode.isMiniGame())
        {
            this.chapter = chapter;
            this.level = level;

            //初始化格子
            setCell(cellListTop, false);
            setCell(cellListBottom, false);

            //初始化阳光
            setSun();

            gaming.loadGamingUI();
            DialogKingShop.Instance.unloadGamingUI();
            SunManager.Instance.enabled = true;
            AI_Manager.Instance.enabled = true;
            List<Card> cards = null;
            int diff = 3;
            switch (gameMode)
            {
                case GameMode.MiniGame_TETF:
                    diff = 3;
                    AI_Manager.Instance.aiCardList = Utils.getRandomCardList();
                    break;
                case GameMode.MiniGame_WGWE:
                    diff = 8;
                    AI_Manager.Instance.aiCardList = Utils.getRandomCardList();
                    break;
                case GameMode.MiniGame_BS:
                    diff = 14;
                    AI_Manager.Instance.aiCardList = aiCardListBS;
                    cards = playerCardListBS;
                    break;
            }
            AI_Manager.Instance.setAI();
            AI_Manager.Instance.difficult = diff;
            CardManager.Instance.gameStart(cards);
        }
        inGame = true;
    }
    /// <summary>
    /// 游戏暂停
    /// </summary>
    public void gamePause()
    {
        bool inKingShopUI = DialogKingShop.Instance == null ? false : DialogKingShop.Instance.inKingShopUI;
        if (inKingShopUI || DialogTextbook.Instance.inTextbook || gameMode == GameMode.MultiPlayer) return;
        gaming.pauseUI.SetActive(true);
        SoundsManager.pauseMusic();
        Sounds.pause.play();
        Time.timeScale = 0f;
    }
    /// <summary>
    /// 游戏继续
    /// </summary>
    public void gameContinue()
    {
        if (DialogSettings.Instance.inSetting || gameMode == GameMode.MultiPlayer) return;
        gaming.pauseUI.SetActive(false);
        SoundsManager.resumeMusic();
        var speed = DataManager.Instance.data.gameSpeed;
        Time.timeScale = speed;
        if(gaming.speedButton != null)
        {
            var temp = Array.IndexOf(speeds, speed);
            gaming.speedButton.GetComponent<Image>().sprite = ImageManager.Instance.alarms[temp];
        }
    }
    /// <summary>
    /// 寻找守门NPC的prefab
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public HomeNpc getHomeNpcPrefab(HomeNpcType type)
    {
        foreach(var npc in homeNpcPrefabList)
        {
            if (npc.homeNpcType == type) return npc;
        }
        return null;
    }

    /// <summary>
    /// 设置游戏速度
    /// </summary>
    public void changeGameSpeed()
    {
        if (DialogSettings.Instance.inSetting || Time.timeScale == 0 || gameMode == GameMode.MultiPlayer) return;
        var speed = DataManager.Instance.data.gameSpeed;
        var temp = Array.IndexOf(speeds, speed);
        temp = (temp + 1) % speeds.Length;
        Time.timeScale = speeds[temp];
        gaming.speedButton.GetComponent<Image>().sprite = ImageManager.Instance.alarms[temp];
        DataManager.Instance.data.gameSpeed = speeds[temp];
        DataManager.Instance.savePlayerData();
    }
    /// <summary>
    /// 游戏胜利
    /// </summary>
    public void win()
    {
        var doorPos = new Vector3(7,-1,0);
        var coinBagPrefab = bm.itemsOfCoin[0];
        var data = DataManager.Instance.data;
        if (chapter == 1)
        {
            switch (level)
            {
                case 1:
                    if (data.timeC1L1 == 0)
                    {
                        Utils.gift(bm.itemsOfCard.findGiftCard(EntityType.SunFlower), doorPos, true);
                        data.level = 2;
                    }
                    else
                    {
                        Utils.giveCoinBag(coinBagPrefab, 10, doorPos, true);
                        data.timeC1L1++;
                    }
                    break;
                case 2:
                    if (data.timeC1L2 == 0)
                    {
                        Utils.gift(bm.itemsOfCard.findGiftCard(EntityType.WallNut), doorPos, true);
                        data.level = 3;
                    }
                    else
                    {
                        Utils.giveCoinBag(coinBagPrefab, 12, doorPos, true);
                        data.timeC1L2++;
                    }
                    break;
                case 3:
                    if (data.timeC1L3 == 0)
                    {
                        Utils.gift(bm.itemsOfCard.findGiftCard(EntityType.Cabbage), doorPos, true);
                        data.level = 4;
                    }
                    else
                    {
                        Utils.giveCoinBag(coinBagPrefab, 15, doorPos, true);
                        data.timeC1L3++;
                    }
                    break;
                case 4:
                    if (data.timeC1L4 == 0)
                    {
                        Utils.gift(bm.itemsOfOthers[0], doorPos, true);
                        data.level = 5;
                    }
                    else
                    {
                        Utils.giveCoinBag(coinBagPrefab, 20, doorPos, true);
                        data.timeC1L4++;
                    }
                    break;
                case 5:
                    if (data.timeC1L5 == 0)
                    {
                        Utils.gift(bm.itemsOfCard.findGiftCard(EntityType.Cornpult), doorPos, true);
                        data.level = 6;
                    }
                    else
                    {
                        Utils.giveCoinBag(coinBagPrefab, 25, doorPos, true);
                        data.timeC1L5++;
                    }
                    break;
                case 6:
                    if (data.timeC1L6 == 0)
                    {
                        Utils.gift(bm.itemsOfCard.findGiftCard(EntityType.SnowPeaShooter), doorPos, true);
                        data.level = 7;
                    }
                    else
                    {
                        Utils.giveCoinBag(coinBagPrefab, 30, doorPos, true);
                        data.timeC1L6++;
                    }
                    break;
                case 7:
                    if (data.timeC1L7 == 0)
                    {
                        Utils.gift(bm.itemsOfCard.findGiftCard(EntityType.Watermelon), doorPos, true);
                        data.level = 8;
                    }
                    else
                    {
                        Utils.giveCoinBag(coinBagPrefab, 40, doorPos, true);
                        data.timeC1L7++;
                    }
                    break;
                case 8:
                    if (data.timeC1L8 == 0)
                    {
                        Utils.gift(bm.itemsOfOthers[1], doorPos, true);
                        data.level = 1;
                        data.chapter = 2;
                    }
                    else
                    {
                        Utils.giveCoinBag(coinBagPrefab, 100, doorPos, true);
                        data.timeC1L8++;
                    }
                    break;
            }
        }
        else if (chapter == 2)
        {
            switch (level)
            {
                case 1:
                    if (data.timeC2L1 == 0)
                    {
                        Utils.gift(bm.itemsOfCard.findGiftCard(EntityType.RageBottleSpawner), doorPos, true);
                        data.level = 2;
                    }
                    else
                    {
                        Utils.giveCoinBag(coinBagPrefab, 120, doorPos, true);
                        data.timeC2L1++;
                    }
                    break;
                case 2:
                    if (data.timeC2L2 == 0)
                    {
                        Utils.gift(bm.itemsOfCard.findGiftCard(EntityType.FireballSpawner), doorPos, true);
                        data.level = 3;
                    }
                    else
                    {
                        Utils.giveCoinBag(coinBagPrefab, 150, doorPos, true);
                        data.timeC2L2++;
                    }
                    break;
                case 3:
                    if (data.timeC2L3 == 0)
                    {
                        Utils.gift(bm.itemsOfCard.findGiftCard(EntityType.GiantPekka), doorPos, true);
                        data.level = 4;
                    }
                    else
                    {
                        Utils.giveCoinBag(coinBagPrefab, 200, doorPos, true);
                        data.timeC2L3++;
                    }
                    break;
                case 4:
                    if (data.timeC2L4 == 0)
                    {
                        Utils.gift(bm.itemsOfCard.findGiftCard(EntityType.WitchZombie), doorPos, true);
                        data.level = 5;
                    }
                    else
                    {
                        Utils.giveCoinBag(coinBagPrefab, 250, doorPos, true);
                        data.timeC2L4++;
                    }
                    break;
                //TODO LEVEL5
            }
        }
        else if (chapter == 99)
        {
            switch (level)
            {
                case 1:
                    Utils.giveCoinBag(coinBagPrefab, 114514, doorPos, true);
                    break;
            }
        }
        else if (gameMode.isMiniGame())
        {
            switch (gameMode)
            {
                case GameMode.MiniGame_TETF:
                    data.timeTETF++;
                    break;
                case GameMode.MiniGame_WGWE:
                    data.timeWGWE++;
                    break;
            }
            if (data.miniGameLevel > level)
            {
                Utils.giveCoinBag(coinBagPrefab, 500, doorPos, true);
            }
            else
            {
                Utils.gift(bm.itemsOfOthers[2], doorPos, true);
            }
        }
        //todo chapter 3

    }
    /// <summary>
    /// 游戏失败
    /// </summary>
    public void lose()
    {
        SoundsManager.stopMusic();
        Musics.失败音乐.play(false);
        gaming.loseUI.SetActive(true);
    }
    
    /// <summary>
    /// 加载场景的空白动画
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="delay"></param>
    /// <param name="sth"></param>
    /// <param name="whiteBG"></param>
    private void changeSceneAni(string sceneName, float delay = 5f, Action sth = null,bool whiteBG = false)
    {
        UIManager.Instance.PFReRegisterUI();
        if (whiteBG && Time.timeScale != 0)
        {
            gaming.jiesuanWhite.SetActive(true);
            gaming.jiesuanWhite.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
        }
        DOVirtual.DelayedCall(delay, () =>
        {
            if (sth != null) sth.Invoke();
            SceneManager.LoadScene(sceneName);
        });
    }
    /// <summary>
    /// 切换场景 常用于返回菜单
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="delay"></param>
    /// <param name="gameMode"></param>
    public void changeScene(string sceneName,float delay = 0.5f,GameMode gameMode = GameMode.Normal,bool whiteBG = false)
    {
        inGame = false;
        gaming.loseUI.SetActive(false);
        switch (gameMode)
        {
            case GameMode.SRCS:
                saveSRCS();
                break;
            case GameMode.MultiPlayer:
                if (MultiGameManager.server != null)
                {
                    MultiGameManager.server.Stop();
                    MultiGameManager.server = null;
                }
                if (MultiGameManager.client != null)
                {
                    MultiGameManager.client.CloseAsync();
                    MultiGameManager.client = null;
                }
                break;
        }
        changeSceneAni(sceneName, delay, () => Time.timeScale = 1f, whiteBG);
    }
    /// <summary>
    /// 通过UI切换场景
    /// </summary>
    /// <param name="sceneName"></param>
    public void changeSceneByUI(string sceneName)
    {
        changeScene(sceneName, 0.5f, GameMode.None);
    }
    /// <summary>
    /// 加载无尽模式防御塔和家
    /// </summary>
    public void loadSRCSTowers() 
    {
        zombiesComingSoundFlag = true;
        var dat = DataManager.Instance.data;
        foreach (var tower in dat.allTowersSRCS)
        {
            tower.load();
        }
        foreach (var entity in dat.allEntitiesSRCS)
        {
            if (entity.tag == "Home")
            {
                entity.load();
            }
        }
        enemyTowerCount = 2;
        friendTowerCount = dat.allTowersSRCS.Count;
    }
    /// <summary>
    /// 加载无尽模式实体 (不包括家)
    /// </summary>
    public void loadSRCSEntities()
    {
        var dat = DataManager.Instance.data;
        foreach (var entity in dat.allEntitiesSRCS)
        {
            if (entity.tag != "Home")
            {
                entity.load();
            }
        }
    }
    /// <summary>
    /// 加载实体的BUFF
    /// </summary>
    public void loadSRCSBuffs()
    {
        var dat = DataManager.Instance.data;
        foreach (var buff in dat.allBuffsSRCS)
        {
            buff.load();
        }
    }
    /// <summary>
    /// 保存无尽模式进度
    /// </summary>
    public void saveSRCS()
    {
        var dat = DataManager.Instance.data;
        if (!dat.havePlayedSCRS) return;

        List<SavedEntity> savedEntities = new List<SavedEntity>();
        List<SavedTower> savedTowers = new List<SavedTower>();
        List<SavedBuff> savedBuffs = new List<SavedBuff>();
        GameObject[] homes = GameObject.FindGameObjectsWithTag("Home"); // 家当作普通实体保存
        IEnumerable<GameObject> entities = homes.Concat(GameObject.FindGameObjectsWithTag("Entity"));
        foreach (GameObject gameObject in entities)
        {
            if (gameObject == null) return;
            Entity entity = gameObject.GetComponent<Entity>();
            if (entity == null) continue;
            if (entity.hasParent) continue;
            var saved = entity.save();

            if (saved == null || savedEntities.Contains(saved)) continue;

            savedEntities.Add(saved);
            if(entity.buffList.Count != 0)
            {
                foreach (Buff buff in entity.buffList)
                {
                    var savedBuff = buff.save();

                    if (savedBuff == null || savedBuffs.Contains(savedBuff)) continue;
                    savedBuffs.Add(savedBuff);
                }
            }
        }
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        foreach (GameObject gameObject in towers)
        {
            Tower tower = gameObject.GetComponent<Tower>();
            var saved = tower.save();

            if (saved == null || savedTowers.Contains(saved)) continue;
            savedTowers.Add(saved);
        }
        // 游戏开始之前自动保存就在运行了，啥都没有的时候不保存
        if (savedEntities.Count == 0 && savedTowers.Count == 0) return;
        dat.sunPointSRCS = SunManager.Instance.sunPoint;
        dat.allEntitiesSRCS = savedEntities;
        dat.allTowersSRCS = savedTowers;
        dat.allBuffsSRCS = savedBuffs;
        DataManager.Instance.savePlayerData();
    }
    /// <summary>
    /// 更新敌人数量SRCS
    /// </summary>
    private void updateEnemyCount()
    {
        int currentCount = 0;
        GameObject[] gms = GameObject.FindGameObjectsWithTag("Entity");
        foreach(GameObject obj in gms)
        {
            if (obj.GetComponent<Entity>() == null) continue;
            Entity entity = obj.GetComponent<Entity>();
            if (entity.hitpoint <= 0) continue;
            if(entity.entityGroup == EntityGroup.enemy)
            {
                currentCount++;
            }
        }
        enemyCount = currentCount;
    }
    /// <summary>
    /// 重新开始尸如潮水模式
    /// </summary>
    public void resetSRCS()
    {
        var dat = DataManager.Instance.data;
        dat.havePlayedSCRS = false;
        dat.allBuffsSRCS.Clear();
        dat.allEntitiesSRCS.Clear();
        dat.allTowersSRCS.Clear();
        if(dat.scoreSRCS > dat.maxScoreSRCS) dat.maxScoreSRCS = dat.scoreSRCS;
        dat.scoreSRCS = 0;
        DataManager.Instance.savePlayerData();
    }
    /// <summary>
    /// 游戏重开
    /// </summary>
    public void restartGame()
    {
        Time.timeScale = 1f;
        UIManager.Instance.PFReRegisterUI();
        if (gameMode == GameMode.SRCS)
        {
            resetSRCS();
        }
        SceneManager.LoadScene(Utils.scene);
    }
    /// <summary>
    /// 生成疯狂戴夫BOSS
    /// </summary>
    public void summonDaveBoss()
    {
        Instantiate(BossDave);
    }
    /// <summary>
    /// 更新无尽分数
    /// </summary>
    public void 扭曲程度Update()
    {
        if (gaming.扭曲程度TXT == null) return;
        DataManager.Instance.data.scoreSRCS += Time.deltaTime * 10 * (1 + DataManager.Instance.data.towerLevel * 0.1f) + DataManager.Instance.data.scoreSRCS * 0.00001f * Time.timeScale;
        gaming.扭曲程度TXT.text = "园区空间-扭曲程度：" + Mathf.Floor(DataManager.Instance.data.scoreSRCS);
    }
    /// <summary>
    /// 屏幕适配
    /// </summary>
    public void setCardAreaUIWithScreen()
    {
        float 屏幕比例 = (float)((float)Screen.width / (float)Screen.height);
        if(屏幕比例 < 16f / 9f)
        {
            Vector3 r1 = oldAPos;
            r1.y += ((16f / 9f - 屏幕比例) / 0.27f) * 90f;
            gaming.cardAreaUI.GetComponent<RectTransform>().localPosition = r1;
        }
        else
        {
            gaming.cardAreaUI.GetComponent<RectTransform>().localPosition = oldAPos;
        }
    }
    /// <summary>
    /// 时停游戏
    /// </summary>
    public void stopTheTime()
    {
        var t = TestManager.Instance;
        if(Time.timeScale <= t.时停倍数)
        {
            Time.timeScale = DataManager.Instance.data.gameSpeed;
        }
        else
        {
            Time.timeScale = t.时停倍数;
        }
    }
}
