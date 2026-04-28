using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DialogPFW : MonoBehaviour, UIHelper
{
    public static DialogPFW Instance { get; private set; }
    public UIManager ui { get => UIManager.Instance; }
    public DialogPFWTower setTower { get => DialogPFWTower.Instance; }
    public DialogPFWHome setHome { get => DialogPFWHome.Instance; }

    [HideInInspector] public int setCardUiTemp;//索引 0:编辑卡组 1:Tower Entity 2:编辑Home守护者 3:Emoji
    public GameObject setCardUI;
    public GameObject setTowerEntityUI;
    public GameObject setHomeNPCUI;
    public GameObject setCardMenuUI;
    public GameObject setEmojiUI;// * coming soon ! 
    public GameObject cardBagUI;
    public GameObject setCardBGUI;
    public string GetName()
    {
        return "PFW";
    }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UIManager.registerUI(this);
    }
    [Action("home")]
    /// <summary>
    /// 转到设置Home守护者
    /// </summary>
    public void transitionToSetHomeNPC()
    {
        CardManager.Instance.stopSetCard();
        setCardUiTemp = 2;

        setHome.loadHomeNpcInfo();
        setHome.updateHomeNpc();
        setCardMenuUI.SetActive(false);
        cardBagUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(1800, -1.4f), 0.2f).OnComplete(() =>
        {
            setHomeNPCUI.SetActive(true);
            setHome.homeNpc.GetComponent<RectTransform>().DOAnchorPos(new Vector2(405.16f, 0), 0.2f);
            setHome.homeNpcInfoUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-420, 10f), 0.2f);
        });
        setHome.选择你的守护者.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-440, 350f), 0.2f);
        setCardBGUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-1800, 266.6f), 0.2f).OnComplete(() =>
        {
            setCardUI.SetActive(false);
        });
    }
    [Action("tower")]
    /// <summary>
    /// 转到设置Tower上方Entity的界面
    /// </summary>
    public void transitionToSetTowerAttach()
    {
        CardManager.Instance.stopSetCard();
        setCardUiTemp = 1;

        setTower.towerEntityInfoUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-500, -174.76f), 0.2f);
        setCardBGUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-1800, 266.6f), 0.2f).OnComplete(() =>
        {
            setCardUI.SetActive(false);
        });
        setCardMenuUI.SetActive(false);
        setTowerEntityUI.SetActive(true);

        List<Card> totalCard = new List<Card>(DataManager.Instance.data.cardList);
        totalCard.AddRange(DataManager.Instance.data.cardBag);
        for (int i = totalCard.Count - 1; i >= 0; i--)
        {
            var card = totalCard[i];
            if (card.entityType.isZombie() || card.entityType.isAreaEffect())
            {
                totalCard.RemoveAt(i);
            }
        }
        setTower.updateTowerEntity();
        CardManager.Instance.setTowerCard(totalCard);
    }
    [Action("emoji")]
    /// <summary>
    /// TODO 转到设置战斗表情
    /// </summary>
    public void transitionToSetEmoji()
    {
        //todo 
        CardManager.Instance.stopSetCard();
        setCardUiTemp = 3;

        setCardMenuUI.SetActive(false);
        setEmojiUI.SetActive(true);
    }
    [Action("back to set card")]
    /// <summary>
    /// 返回初始设置卡牌页面
    /// </summary>
    public void backToSetCard()
    {
        DataManager.Instance.savePlayerData();
        CardManager.Instance.stopSetCard();
        GameObject obj = null;
        float delay = 0;
        switch (setCardUiTemp)
        {
            case 1:
                obj = setTowerEntityUI;
                delay = 0.2f;
                break;
            case 2:
                obj = setHomeNPCUI;
                delay = 0.2f;
                break;
            case 3:
                obj = setEmojiUI;
                break;
        }
        setCardUiTemp = 0;
        if (obj == null) return;
        cardBagUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(421.83f, -3.4f), delay).OnComplete(() =>
        {
            CardManager.Instance.setCard();
            obj.SetActive(false);
            setCardMenuUI.SetActive(true);
        });
        setTower.towerEntityInfoUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-1800, -174.76f), 0.2f);
        setHome.homeNpcInfoUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-1800, 10f), 0.2f);
        setHome.选择你的守护者.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-440, 800f), 0.2f);
        setHome.homeNpc.GetComponent<RectTransform>().DOAnchorPos(new Vector2(1800, 0), 0.2f);
        setCardBGUI.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-500, 266.6f), 0.2f).OnComplete(() =>
        {
            setCardUI.SetActive(true);
        });
    }
    [Action("close")]
    /// <summary>
    /// 退出备战界面
    /// </summary>
    public void stopSetCard()
    {
        if (DataManager.Instance.data.cardList.Count != 8)
        {
            return;
        }
        DataManager.Instance.savePlayerData();
        DialogMainMenu.Instance.transitionToChoose();
    }
}
