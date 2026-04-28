using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 国王的商店(SRCS) 设施升级
/// </summary>
public class DialogKingShopUpgrade : MonoBehaviour, UIHelper
{
    public static DialogKingShopUpgrade Instance { get; private set; }
    public DialogKingShop parent { get => DialogKingShop.Instance; }
    public UIManager ui { get => UIManager.Instance; }
    public string GetName()
    {
        return "KingShop.upgrade";
    }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UIManager.registerUI(this);
    }

    public GameObject upgradeUI;//父UI

    public GameObject maxText;//已达最高等级提示

    public GameObject nowTower;//当前等级tower外观
    public GameObject nextTower;//下一级tower外观

    public GameObject nowLevel;//当前等级图标
    public GameObject nextLevel;//下一等级图标

    public TextMeshProUGUI costCoin;//花费金币

    public TextMeshProUGUI towerHpNow;//当前tower最高血量
    public TextMeshProUGUI homeHpNow;//当前home最高血量

    public TextMeshProUGUI towerHpNext;//下一级tower最高血量
    public TextMeshProUGUI homeHpNext;//下一级home最高血量

    [Action("open")]
    public void openUpgradeUI()
    {
        upgradeUI.SetActive(true);
        UIManager.fadeOutAllChild(parent.kingShopMainUI);
        UIManager.fadeInAllChild(upgradeUI);
        parent.shopBG.GetComponent<Image>().sprite = ImageManager.Instance.kingShopBG[2];
        updateUpgradeUI();
    }
    public void updateUpgradeUI()
    {
        parent.coinCount.text = DataManager.Instance.data.coinCount.ToString();
        int nowLevel = DataManager.Instance.data.towerLevel;

        //设置相关贴图
        this.nowLevel.GetComponent<Image>().sprite = ImageManager.Instance.levelIcon[nowLevel - 1];
        nextLevel.GetComponent<Image>().sprite = ImageManager.Instance.levelIcon[nowLevel];
        nowTower.GetComponent<Image>().sprite = ImageManager.Instance.towerSkins[nowLevel - 1];
        nextTower.GetComponent<Image>().sprite = ImageManager.Instance.towerSkins[nowLevel];
        parent.doorLevel.GetComponent<Image>().sprite = ImageManager.Instance.levelIcon[nowLevel - 1];
        //设置升级花费金币和属性text
        switch (nowLevel)
        {
            case 1:
                homeHpNow.text = "150"; towerHpNow.text = "90";
                homeHpNext.text = "180"; towerHpNext.text = "105";
                costCoin.text = "1000";
                break;
            case 2:
                homeHpNow.text = "180"; towerHpNow.text = "105";
                homeHpNext.text = "210"; towerHpNext.text = "120";
                costCoin.text = "2000";
                break;
            case 3:
                homeHpNow.text = "210"; towerHpNow.text = "120";
                homeHpNext.text = "240"; towerHpNext.text = "135";
                costCoin.text = "5000";
                break;
            case 4:
                homeHpNow.text = "240"; towerHpNow.text = "135";
                homeHpNext.text = "300"; towerHpNext.text = "150";
                costCoin.text = "10000";
                break;
            case 5:
                homeHpNow.text = "300"; towerHpNow.text = "150";
                homeHpNext.text = "400"; towerHpNext.text = "200";
                costCoin.text = "20000";
                break;
            case 6:
                homeHpNow.text = "400"; towerHpNow.text = "200";
                homeHpNext.text = "???"; towerHpNext.text = "???";
                costCoin.text = "MAX";
                break;
        }

        if (nowLevel == 6)
        {
            maxText.SetActive(true);
        }//满级后提醒会出现
    }
    [Action("tower")]
    public void upgradeTower()
    {
        int nowLevel = DataManager.Instance.data.towerLevel;
        int coin = DataManager.Instance.data.coinCount;
        int costCoin = 99999;
        //设置升级花费金币
        switch (nowLevel)
        {
            case 1:
                costCoin = 1000;
                break;
            case 2:
                costCoin = 2000;
                break;
            case 3:
                costCoin = 5000;
                break;
            case 4:
                costCoin = 10000;
                break;
            case 5:
                costCoin = 20000;
                break;
            case 6:
                costCoin = 30000;
                break;
        }
        if (coin >= costCoin)
        {
            if (nowLevel == 6) return;//防止超过版本最高等级
            CoinManager.Instance.changeCoin(costCoin * -1);
            DataManager.Instance.data.towerLevel++;
            updateUpgradeUI();
            var gm = GameManager.Instance;
            gm.tower1.GetComponent<Tower>().updateTranslate();
            gm.tower2.GetComponent<Tower>().updateTranslate();
            gm.home1.GetComponent<Home>().updateHomeTranslate();
            if (gm.tower1 != null)
            {
                gm.tower1.GetComponent<Tower>().towerHp = gm.tower1.GetComponent<Tower>().towerMaxHp;
                gm.tower1.GetComponent<Tower>().attachEntity.hitpoint = gm.tower1.GetComponent<Tower>().towerHp;
                gm.tower1.GetComponent<Tower>().attachEntity.maxHitpoint = gm.tower1.GetComponent<Tower>().towerMaxHp;
            }
            if (gm.tower2 != null)
            {
                gm.tower2.GetComponent<Tower>().towerHp = gm.tower2.GetComponent<Tower>().towerMaxHp;
                gm.tower2.GetComponent<Tower>().attachEntity.hitpoint = gm.tower2.GetComponent<Tower>().towerHp;
                gm.tower2.GetComponent<Tower>().attachEntity.maxHitpoint = gm.tower2.GetComponent<Tower>().towerMaxHp;
            }
            gm.home1.GetComponent<Entity>().hitpoint = gm.home1.GetComponent<Entity>().maxHitpoint;
            DataManager.Instance.savePlayerData();
        }
        else parent.cantAfford();
    }
}
