using System;
using UnityEngine;

/// <summary>
/// 国王的商店(SRCS) 道具商店
/// </summary>
public class DialogKingShopItem : MonoBehaviour, UIHelper
{
    public static DialogKingShopItem Instance { get; private set; }
    public DialogKingShop parent { get => DialogKingShop.Instance; }
    public UIManager ui { get => UIManager.Instance; }
    public string GetName()
    {
        return "KingShop.item";
    }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UIManager.registerUI(this);
    }

    public GameObject itemShopUI;

    [Action("open")]
    public void openItemShopUI()
    {
        itemShopUI.SetActive(true);
        UIManager.fadeOutAllChild(parent.kingShopMainUI);
        UIManager.fadeInAllChild(itemShopUI);
    }

    // TODO: 将 Assets/Scripts/Other/KingShopItem 移到这里
}
