using Assets.Scripts.Utils;
using System;
using UnityEngine;

public class DialogAbout : MonoBehaviour, UIHelper
{
    public static DialogAbout Instance { get; private set; }

    public string GetName()
    {
        return "About";
    }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UIManager.registerUI(this);
    }

    public GameObject OFUI;//关于UI

    public void clickAuthor(string name)
    {
        EntityType type = EntityType.PeaShooter;
        switch (name)
        {
            case "NurlMe":
                type = EntityType.Npeashooter;
                break;
            case "人间工作":
                onClick_MrXiaoM();
                return;
            case "prq":
                //todo
                break;
            case "猫猫":
                //type = EntityType.Catty; todo
                break;
            case "寰宇":
                //todo
                break;
        }
        if (CardManager.Instance.hasCard(type) || AchievementManager.Instance.hasDropItem) return;
        var gm = BridgeManager.Instance;
        Utils.gift(gm.itemsOfCard.findGiftCard(type), new Vector3(-3, 0, 0));
        AchievementManager.Instance.hasDropItem = true;
    }

    [Action("open")]
    public void OF()
    {
        OFUI.SetActive(true);
    }
    [Action("close")]
    public void closeOF()
    {
        OFUI.SetActive(false);
    }

    #region 人间工作
    private int flagM = 0;
    private void onClick_MrXiaoM()
    {
        if (++flagM >= 13)
        {
            flagM = 0;
            closeOF();
            // TODO: 小游戏彩蛋
        }
    }

    #endregion
}
