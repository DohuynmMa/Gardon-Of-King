using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Utils;
using System;
public class DialogGamingMenu : MonoBehaviour, UIHelper
{
    [Header("常规")]
    public GameObject 关卡提示Canvas;
    public GameObject pauseButton;//暂停键
    public GameObject textbookButton;
    public GameObject constonleButton;
    public GameObject speedButton;
    public GameObject speedKeyboard;

    public GameObject jiesuanWhite;
    public GameObject cardAreaUI;//卡槽ui1
    public GameObject cardAreaSun;//卡槽阳光区域
    public GameObject loseUI;//游戏失败UI
    public GameObject pauseUI;//暂停UI

    public GameObject shovelBlankUI;
    public GameObject wateringcanBlankUI;


    public TextMeshProUGUI tishiText1;//关卡提示1
    public TextMeshProUGUI tishiText2;//关卡提示2
    public GameObject gaoshi;//关卡告示牌
    public GameObject zbhks;//准备好开始

    public Sprite sunSprite;
    public Sprite moonSprite;

    public TextMeshProUGUI sunPointText;
    public TextMeshProUGUI sunPointMaxText;

    [Header("尸如潮水")]
    public GameObject 扭曲程度UI;
    public TextMeshProUGUI 扭曲程度TXT;
    public static DialogGamingMenu Instance { get; private set; }
    public UIManager ui { get => UIManager.Instance; }
    public GameManager gm { get => GameManager.Instance; }
    public string GetName()
    {
        return "GamingMenu";
    }
    public void loadGamingUI()
    {
        speedButton.GetComponent<Image>().sprite = ImageManager.Instance.alarms[Array.IndexOf(GameManager.speeds, DataManager.Instance.data.gameSpeed)];
        speedKeyboard.SetActive(!Application.isMobilePlatform);
        cardAreaUI.SetActive(true);
        //特殊设置
        switch (gm.gameMode)
        {
            case GameMode.Normal:
                扭曲程度UI.SetActive(false);
                wateringcanBlankUI.SetActive(false);
                break;
            case GameMode.SRCS:
                扭曲程度UI.SetActive(true);
                wateringcanBlankUI.SetActive(true);
                break;
            case GameMode.MultiPlayer:
                speedButton.SetActive(false);
                pauseButton.SetActive(false);
                textbookButton.SetActive(false);
                wateringcanBlankUI.SetActive(false);
                break;
        }
        if (gm.gameMode.isMiniGame())
        {
            textbookButton.SetActive(false);
            wateringcanBlankUI.SetActive(false);
        }
    }
    public void unloadGamingUI()
    {
        speedKeyboard.SetActive(false);
        cardAreaUI.SetActive(false);
    }
    private void Start()
    {
        var 屏幕比例 = (float)((float)Screen.width / (float)Screen.height);
        if (Utils.scene == "Game" || gm.gameMode == GameMode.SRCS || gm.gameMode == GameMode.MultiPlayer)
        {
            关卡提示Canvas.GetComponent<CanvasScaler>().screenMatchMode = 屏幕比例 < 16f / 9f ? CanvasScaler.ScreenMatchMode.Shrink : CanvasScaler.ScreenMatchMode.Expand;
        }
        UIManager.registerUI(this);
    }
    private void Awake()
    {
        Instance = this;
    }
}
