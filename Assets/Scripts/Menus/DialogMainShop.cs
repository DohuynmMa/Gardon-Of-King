using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogMainShop : MonoBehaviour, UIHelper
{
    public static DialogMainShop Instance { get; private set; }
    public UIManager ui { get => UIManager.Instance; }

    public GameObject shopCar;//天上飞下来的车
    public GameObject shopCarShadow;//天上飞下来的车影子
    public GameObject shopItemInfoUI;//商品信息UI
    public GameObject shopItemInfoUIBG;//商品信息BG
    public GameObject shopUI;//商店UI
    private int currentItemID;
    public string GetName()
    {
        return "MainShop";
    }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UIManager.registerUI(this);
    }
    public void showShopItem()
    {
        foreach (var item in shopUI.GetComponent<Shop>().shopList)
        {
            item.gameObject.SetActive(true);
            item.onShow();
        }
    }
    public void hideShopItem()
    {
        foreach (var item in shopUI.GetComponent<Shop>().shopList)
        {
            item.gameObject.SetActive(false);
            item.onHide();
        }
    }
    public void openShopItemInfo(int id)
    {
        currentItemID = id;
        shopItemInfoUI.SetActive(true);
        DialogWallet.Instance.showWallet();
        shopItemInfoUIBG.GetComponent<Image>().sprite = ImageManager.Instance.shopInfo[id];
    }
    [Action("close item info")]
    public void closeShopItemInfo()
    {
        shopItemInfoUI.SetActive(false);
        DialogWallet.Instance.hideWallet();
    }
    [Action("buy item")]
    public void buyItem()
    {
        ShopItem item = findShopItem();
        item.buy();
    }
    private ShopItem findShopItem()
    {
        foreach (ShopItem item in shopUI.GetComponent<Shop>().shopList)
        {
            if (item.id == currentItemID) return item;
        }
        return null;
    }
    public void cantAfford()
    {
        Utils.showDialog("金币不足", 18.1f, Color.white, "确 定", 20f, Color.white);
    }
}
