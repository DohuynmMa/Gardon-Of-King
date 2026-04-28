using Assets.Scripts.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 国王的商店(SRCS) 修补建筑
/// </summary>
public class DialogKingShopFix : MonoBehaviour, UIHelper
{
    public static DialogKingShopFix Instance { get; private set; }
    public DialogKingShop parent { get => DialogKingShop.Instance; }
    public UIManager ui { get => UIManager.Instance; }
    public string GetName()
    {
        return "KingShop.fix";
    }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UIManager.registerUI(this);
    }

    public GameObject fixUI;

    public TextMeshProUGUI tower1Hp;
    public TextMeshProUGUI tower2Hp;
    public TextMeshProUGUI homeHp;//这些都是百分比
    public TextMeshProUGUI fixCostCoin;

    [Action("open")]
    public void openFixUI()
    {
        fixUI.SetActive(true);
        updateFixUI();
        UIManager.fadeOutAllChild(parent.kingShopMainUI);
        UIManager.fadeInAllChild(fixUI);
        parent.shopBG.GetComponent<Image>().sprite = ImageManager.Instance.kingShopBG[1];
    }
    
    public void updateFixUI()
    {
        var gm = GameManager.Instance;
        int nowLevel = DataManager.Instance.data.towerLevel;
        int rebuildCost = 999999;
        homeHp.text = Mathf.RoundToInt((gm.home1.GetComponent<Entity>().hitpoint / gm.home1.GetComponent<Entity>().maxHitpoint) * 100) + "%";
        tower1Hp.text = gm.tower1 != null ? Mathf.RoundToInt((gm.tower1.GetComponent<Tower>().towerHp / gm.tower1.GetComponent<Tower>().towerMaxHp) * 100) + "%" : "待重建";
        tower2Hp.text = gm.tower2 != null ? Mathf.RoundToInt((gm.tower2.GetComponent<Tower>().towerHp / gm.tower2.GetComponent<Tower>().towerMaxHp) * 100) + "%" : "待重建";
        switch (nowLevel)
        {
            case 1:
                rebuildCost = 1000;
                break;
            case 2:
                rebuildCost = 1150;
                break;
            case 3:
                rebuildCost = 1300;
                break;
            case 4:
                rebuildCost = 1450;
                break;
        }
        int homeFixCoin = (int)(gm.home1.GetComponent<Entity>().maxHitpoint - gm.home1.GetComponent<Entity>().hitpoint) * 10;
        int tower1FixCoin = gm.tower1 != null ? (int)(gm.tower1.GetComponent<Tower>().towerMaxHp - gm.tower1.GetComponent<Tower>().towerHp) * 10 : rebuildCost;
        int tower2FixCoin = gm.tower2 != null ? (int)(gm.tower2.GetComponent<Tower>().towerMaxHp - gm.tower2.GetComponent<Tower>().towerHp) * 10 : rebuildCost;
        fixCostCoin.text = (homeFixCoin + tower1FixCoin + tower2FixCoin).ToString();
    }

    [Action("tower")]
    public void fixTower()
    {
        var gm = GameManager.Instance;
        int coin = DataManager.Instance.data.coinCount;
        int nowLevel = DataManager.Instance.data.towerLevel;
        int rebuildCost = 999999;
        int homeFixCoin = (int)(gm.home1.GetComponent<Entity>().maxHitpoint - gm.home1.GetComponent<Entity>().hitpoint) * 10;
        switch (nowLevel)
        {
            case 1:
                rebuildCost = 1000;
                break;
            case 2:
                rebuildCost = 1150;
                break;
            case 3:
                rebuildCost = 1300;
                break;
            case 4:
                rebuildCost = 1450;
                break;
            case 5:
                rebuildCost = 1600;
                break;
            case 6:
                rebuildCost = 2100;
                break;
        }
        int tower1FixCoin = gm.tower1 != null ? (int)(gm.tower1.GetComponent<Tower>().towerMaxHp - gm.tower1.GetComponent<Tower>().towerHp) * 10 : rebuildCost;
        int tower2FixCoin = gm.tower2 != null ? (int)(gm.tower2.GetComponent<Tower>().towerMaxHp - gm.tower2.GetComponent<Tower>().towerHp) * 10 : rebuildCost;
        int totalFixCost = homeFixCoin + tower1FixCoin + tower2FixCoin;
        if (coin >= totalFixCost)
        {
            if (totalFixCost == 0) return;
            if (gm.tower1 == null || gm.tower2 == null) rebuildTower();
            gm.tower1.GetComponent<Tower>().towerHp = gm.tower1.GetComponent<Tower>().towerMaxHp;
            gm.tower1.GetComponent<Tower>().attachEntity.hitpoint = gm.tower1.GetComponent<Tower>().towerHp;
            gm.tower1.GetComponent<Tower>().attachEntity.maxHitpoint = gm.tower1.GetComponent<Tower>().towerMaxHp;
            gm.tower2.GetComponent<Tower>().towerHp = gm.tower2.GetComponent<Tower>().towerMaxHp;
            gm.tower2.GetComponent<Tower>().attachEntity.hitpoint = gm.tower2.GetComponent<Tower>().towerHp;
            gm.tower2.GetComponent<Tower>().attachEntity.maxHitpoint = gm.tower2.GetComponent<Tower>().towerMaxHp;
            gm.home1.GetComponent<Entity>().hitpoint = gm.home1.GetComponent<Entity>().maxHitpoint;
            updateFixUI();
            CoinManager.Instance.changeCoin(totalFixCost * -1);
        }
        else parent.cantAfford();
    }

    public void rebuildTower()
    {
        var gm = GameManager.Instance;
        EntityType entityType = DataManager.Instance.data.towerEntity;
        if (gm.tower1 == null) gm.tower1 = gm.newTower(1, new(-4.61f, 0.84f, 0), EntityGroup.friend, entityType, max => max * 3);
        if (gm.tower2 == null) gm.tower2 = gm.newTower(2, new(-4.61f, -2.8f, 0), EntityGroup.friend, entityType, max => max * 3);
    }
}

