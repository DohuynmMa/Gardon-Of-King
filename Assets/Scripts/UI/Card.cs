using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Utils;
public enum EntityType
{
    PeaShooter,
    NormalZombie,
    ImpZombie,
    SunFlower,
    ConeZombie,
    BucketZombie,
    WallNut,
    Cabbage,
    Gargantuar,
    GoblinImp,
    GoblinGargantuar,
    Npeashooter,
    GatlingPeaShooter,
    Watermelon,
    MinerZombie,
    SnowPeaShooter,
    Cornpult,
    PaperZombie,
    MakeupZombie,
    MinerZombie2,
    IceMelon,
    LittleWolf,
    PekkaImp,
    XbowPea,
    Skeleton,
    SkeletonTomb,
    LIUDEHUA,
    NewYearZombie,
    RageBottleSpawner,
    SnowBottleSpawner,
    FireballSpawner,
    HealBottleSpawner,
    PoisonBottleSpawner,
    GiantPekka,
    WitchZombie,
    FakeZombie,
}
public enum BulletType
{
    Pea,
    Cabbage,
    Son,
    NPea,
    BombWallNut,
    SnowPea,
    Kernal,
    Butter,
    Melon,
    IceMelon,
    Gem,
    WitchZombieLight
}
enum CardState
{
    WaitingForSun,
    Ready
}
public static class EntityTypeHelper
{
    /// <summary>
    /// 该实体是否属于植物 (从 entityType 上判断)
    /// </summary>
    public static bool isPlant(this EntityType type)
    {
        return type == EntityType.PeaShooter
            || type == EntityType.SunFlower
            || type == EntityType.WallNut
            || type == EntityType.Cabbage
            || type == EntityType.Npeashooter
            || type == EntityType.Watermelon
            || type == EntityType.SnowPeaShooter
            || type == EntityType.Cornpult
            || type == EntityType.IceMelon
            || type == EntityType.LittleWolf
            || type == EntityType.LIUDEHUA;
    }
    /// <summary>
    /// 该实体是否属于僵尸 (从 entityType 上判断)
    /// </summary>
    public static bool isZombie(this EntityType type)
    {
        return type == EntityType.NormalZombie
            || type == EntityType.ImpZombie
            || type == EntityType.ConeZombie
            || type == EntityType.BucketZombie
            || type == EntityType.Gargantuar
            || type == EntityType.GoblinImp
            || type == EntityType.GoblinGargantuar
            || type == EntityType.PaperZombie
            || type == EntityType.MakeupZombie
            || type == EntityType.MinerZombie2
            || type == EntityType.NewYearZombie
            || type == EntityType.Skeleton
            || type == EntityType.SkeletonTomb
            || type == EntityType.MinerZombie
            || type == EntityType.GiantPekka
            || type == EntityType.WitchZombie;
    }
    /// <summary>
    /// 该实体是否属于僵尸 (从 Component 上判断)
    /// </summary>
    public static bool isZombie(this Entity e)
    {
        if (e == null) return false;
        return e.GetComponent<Zombie>() != null;
    }
    /// <summary>
    /// 该实体是否属于植物 (从 Component 上判断)
    /// </summary>
    public static bool isPlant(this Entity e)
    {
        if (e == null) return false;
        return e.GetComponent<Plant>() != null;
    }
    /// <summary>
    /// 该实体是否属于实体发生器 (从 Component 上判断)
    /// </summary>
    public static bool isAreaEffect(this Entity e)
    {
        if(e == null) return false;
        return e.GetComponent<AreaEffectEntity>() != null;
    }
    /// <summary>
    /// 该实体是否属于实体发生器 (从 Type 上判断)
    /// </summary>
    public static bool isAreaEffect(this EntityType type)
    {
        return type == EntityType.RageBottleSpawner
            || type == EntityType.SnowBottleSpawner
            || type == EntityType.FireballSpawner
            || type == EntityType.HealBottleSpawner
            || type == EntityType.PoisonBottleSpawner;
    }
}

public class Card : MonoBehaviour
{
    public string cardTag;
    public int summonAmount = 1;
    private CardState cardState = CardState.WaitingForSun;
    public EntityType entityType;
    public GameObject cardLight;
    public GameObject cardGray;
    public Image cardMask;
    public int cardOrder = 0;
    public bool cantBeUsedInRandomCardList = false;//是否在随机卡组禁用?

    [Header("CARD'S INFORMATION")]
    public float needSunPoint;

    public void Update()
    {
        if (Utils.scene == "GameMenu") return;
        switch (cardState)
        {
            case CardState.WaitingForSun:
                WaitingForSunUpdate();
                break;
            case CardState.Ready:
                ReadyUpdate();
                break;
            default:
                break;
        }
    }
    void WaitingForSunUpdate()
    {
        cardMask.fillAmount = 1f - ((float)SunManager.Instance.sunPoint / (float)needSunPoint);
        if (needSunPoint <= SunManager.Instance.sunPoint) TransitionsToReady();
    }
    void ReadyUpdate()
    {
        if (needSunPoint > SunManager.Instance.sunPoint) TransitionsToWaitingSun();
    }
    public void OnClick()
    {
        var hm = HandManager.Instance;
        if (Utils.scene == "GameMenu") return;
        if (!CardManager.Instance.cardEnable.Contains(this)) return;
        if (needSunPoint > SunManager.Instance.sunPoint
            || hm.handingEntity
            || hm.currentEntity != null
            || hm.currentCard != null
            || hm.shovel.usingShovel)
        {
            return;
        }
        if(GameManager.Instance.gameMode == GameMode.SRCS)
        {
            if (hm.wateringCan.usingWateringCan) return;
        }
        hm.addEntity(entityType);
        hm.currentCard = this;
        Sounds.SeedLift.play();
        hm.handingEntity = true;
        hm.currentSummonEntityCount = summonAmount;
        updateColor();
    }
    public void OnUp()
    {
        HandManager.Instance.addOrStopAddingEntity();
    }



    public void TransitionsToWaitingSun()
    {
        cardState = CardState.WaitingForSun;
        cardLight.SetActive(false);
        cardGray.SetActive(true);
        cardMask.gameObject.SetActive(true);
    }
    public void TransitionsToReady()
    {
        cardState = CardState.Ready;
        cardLight.SetActive(true);
        cardGray.SetActive(false);
        cardMask.gameObject.SetActive(false);
    }
    public void updateColor()
    {
        var colorTemp = (byte)(HandManager.Instance.handingEntity ? 70 : 255);
        cardLight.GetComponent<Image>().color = new Color32(colorTemp, colorTemp, colorTemp, 255);
    }
}
