using System.Collections.Generic;

[System.Serializable]
public class MyData
{
    public int version_DO_NOT_MODIFY; // 配置文件版本号，用于自动迁移数据
    [IntRange(min = 0)]
    public int chapter; //章节
    [IntRange(min = 0)]
    public int level; //关卡
    public string playerName; //名字
    [IntRange(min = 0)]
    public int coinCount; //硬币数量
    public bool hasShovel = false; //是否有铲子
    public bool hasWateringCan = false; //是否有水壶
    public bool hasDave = false;//是否有DAVE NPC
    [FloatRange(0f, 1f)]
    public float musicVolume = 1; //音乐音量
    [FloatRange(0f, 1f)]
    public float soundVolume = 1; //音效音量
    [FloatRange(0f, 1f)]
    public float voiceVolume = 1; //语音音量
    [FloatRange(0.5f, 2f)]
    public float gameSpeed = 1;//游戏速度
    public bool showHpBar = true; //显示血条
    public bool fullScreen = true; //是否全屏
    public bool skipVideo = false; //跳过动画

    public List<Card> cardBag; //卡牌背包
    public List<SavedCard> savedCardBag;

    public List<Card> cardList; //出站卡牌
    public List<SavedCard> savedCardList;

    public EntityType towerEntity = EntityType.PeaShooter;
    public HomeNpcType homeNpc = HomeNpcType.none;
    
    [IntRange(min = 0)]
    public int cardCount; //卡牌数量,不包括试用卡

    //通关次数
    [IntRange(min = 0)]
    public int timeC1L1;
    [IntRange(min = 0)]
    public int timeC1L2;
    [IntRange(min = 0)]
    public int timeC1L3;
    [IntRange(min = 0)]
    public int timeC1L4;
    [IntRange(min = 0)]
    public int timeC1L5;
    [IntRange(min = 0)]
    public int timeC1L6;
    [IntRange(min = 0)]
    public int timeC1L7;
    [IntRange(min = 0)]
    public int timeC1L8;
    [IntRange(min = 0)]
    public int timeC2L1;
    [IntRange(min = 0)]
    public int timeC2L2;
    [IntRange(min = 0)]
    public int timeC2L3;
    [IntRange(min = 0)]
    public int timeC2L4;

    //迷你游戏次数
    [IntRange(min = 1)]
    public int miniGameLevel = 1;
    [IntRange(min = 0)]
    public int timeTETF;
    [IntRange(min = 0)]
    public int timeWGWE;
    [IntRange(min = 0)]
    public int cupCount;//奖杯个数
    //尸如潮水存档设置
    public bool havePlayedSCRS = false;
    [IntRange(min = 0)]
    public float scoreSRCS = 0;
    [IntRange(min = 0)]
    public float maxScoreSRCS = 0;
    public List<SavedEntity> allEntitiesSRCS;
    public List<SavedTower> allTowersSRCS;
    public List<SavedBuff> allBuffsSRCS;
    [IntRange(min = 0)]
    public float sunPointSRCS;
    [IntRange(min = 1)]
    public int towerLevel = 1;
    //成就相关

    internal void afterLoad()
    {
        cardBag.Clear();
        foreach (var saved in savedCardBag)
        {
            var card = saved.load();
            if (card != null) cardBag.Add(card);
        }
        cardList.Clear();
        foreach (var saved in savedCardList)
        {
            var card = saved.load();
            if (card != null) cardList.Add(card);
        }
    }

    internal void beforeSave()
    {
        savedCardBag.Clear();
        foreach (var card in cardBag)
        {
            if (card == null) continue;
            savedCardBag.Add(card.save());
        }
        savedCardList.Clear();
        foreach (var card in cardList)
        {
            if (card == null) continue;
            savedCardList.Add(card.save());
        }
    }
}
