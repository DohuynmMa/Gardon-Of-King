using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 国王的商店(SRCS)
/// </summary>
public class DialogKingShop : MonoBehaviour, UIHelper
{
    public static DialogKingShop Instance { get; private set; }
    public UIManager ui { get => UIManager.Instance; }
    public string GetName()
    {
        return "KingShop";
    }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UIManager.registerUI(this);
    }
    public GameObject kingShopUI;//总的父UI
    public GameObject kingShopButton;//进入按钮
    public GameObject doorLevel;//门前的等级图标
    public GameObject kingShopMainUI;//商店主菜单
    public GameObject shopBG;//商店背景
    public TextMeshProUGUI coinCount;

    [HideInInspector] public bool inKingShopUI;
    public void loadGamingUI()
    {
        doorLevel.SetActive(true);
        kingShopButton.SetActive(true);
        doorLevel.GetComponent<Image>().sprite = ImageManager.Instance.levelIcon[DataManager.Instance.data.towerLevel - 1];
    }
    public void unloadGamingUI()
    {
        doorLevel.SetActive(false);
        kingShopButton.SetActive(false);
    }

    [Action("open")]
    public void openShop()
    {
        inKingShopUI = true;
        kingShopUI.SetActive(true);
        coinCount.text = DataManager.Instance.data.coinCount.ToString();
        UIManager.fadeInAllChild(kingShopUI);
        DialogKingShopUpgrade.Instance.maxText.SetActive(false);
    }
    [Action("close")]
    public void closeShop()
    {
        inKingShopUI = false;
        UIManager.fadeOutAllChild(kingShopUI);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            kingShopUI.SetActive(false);
        });
    }
    [Action("return")]
    public void returnToShop()
    {
        UIManager.fadeOutAllChild(DialogKingShopFix.Instance.fixUI);
        UIManager.fadeOutAllChild(DialogKingShopUpgrade.Instance.upgradeUI);
        UIManager.fadeOutAllChild(DialogKingShopItem.Instance.itemShopUI);
        UIManager.fadeInAllChild(kingShopMainUI);
        shopBG.GetComponent<Image>().sprite = ImageManager.Instance.kingShopBG[0];
    }

    public void cantAfford()
    {
        Utils.showDialog("金币不足", 18.1f, Color.white, "确 定", 20f, Color.white);
    }

}
