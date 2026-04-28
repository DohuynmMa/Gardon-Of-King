using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Assets.Scripts.Utils;
using System.Collections.Generic;

public class DialogMainMenu : MonoBehaviour, UIHelper
{
    public static DialogMainMenu Instance { get; private set; }
    public UIManager ui { get => UIManager.Instance; }
    public GameObject menuNewCanvas;
    public GameObject menu;
    public GameObject menu2;
    public GameObject logo;
    public GameObject welcomeText;//玩家名字txt外壳
    public TextMeshProUGUI nameTxt;//玩家名字txt
    public GameObject whiteBg;//选关背景

    public GameObject chapt1;//第一章全部关卡
    public GameObject chapt2;//第二章全部关卡
    public GameObject chapter2EnterButton;//第二章进入按钮
    public List<Sprite> chapterButtonSprites;

    public GameObject xuanzhangjie;//章节选择
    public GameObject pvpUI;//pvp ui
    public GameObject miniGameUI;//益智时刻 UI
    public TextMeshProUGUI miniGameCups;
    public GameObject setCard;//卡牌设置

    public GameObject sb;//需要隐藏的按钮

    public string GetName()
    {
        return "MainMenu";
    }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UIManager.registerUI(this);
        float 屏幕比例 = (float)((float)Screen.width / (float)Screen.height);
        //更新背景
        if ((DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 6) || DateTime.Now.Hour >= 18)
        {
            Utils.changeBackground(BackgroundType.Night);
        }
        else
        {
            Utils.changeBackground(BackgroundType.Day);
        }

        if (Utils.scene == "GameMenu")
        {
            Time.timeScale = 1f;
            menuNewCanvas.GetComponent<CanvasScaler>().screenMatchMode = 屏幕比例 < 16f / 9f ? CanvasScaler.ScreenMatchMode.Shrink : CanvasScaler.ScreenMatchMode.Expand;
        }
        bool success = DialogSetName.Instance.checkName();
        if (success)
        {
            if (sb != null) sb.SetActive(false);
            transitionToChoose();
        }
    }
    [Action("choose chapter")]
    public void transitionToEnter() // 打开 冒险风暴(闯关模式) 菜单
    {
        if (Utils.scene != "GameMenu") return;
        UIManager.fadeOutAllChild(setCard);
        UIManager.fadeOutAllChild(chapt1);
        UIManager.fadeOutAllChild(chapt2);
        UIManager.scaleOut(menu2);
        menu.GetComponent<RectTransform>().DOAnchorPos(new Vector2(404, 833), 0.2f).OnComplete(() =>
        {
            whiteBg.GetComponent<SpriteRenderer>().sprite = ImageManager.Instance.menuBg[0];
            whiteBg.transform.DOPath(new Vector3[] { new Vector3(0, 0, 0) }, 0.2f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                UIManager.fadeInAllChild(xuanzhangjie);
            });
        });
        logo.transform.DOPath(new Vector3[] { new Vector3(6.6f, 0, 29) }, 0.2f, PathType.CatmullRom).SetEase(Ease.OutQuad);
        welcomeText.SetActive(false);
        chapter2EnterButton.GetComponent<Image>().sprite = chapterButtonSprites[DataManager.Instance.data.chapter >= 2 ? 1 : 2];
        DOVirtual.DelayedCall(0.2f, () =>
        {
            sb.SetActive(false);
        });
    }
    [Action("chapter1")]
    public void transitionToChapter1() // 打开 闯关模式 -> 第一章 菜单
    {
        if (Utils.scene != "GameMenu") return;
        sb.SetActive(true);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            UIManager.fadeInAllChild(chapt1);
        });
        UIManager.fadeOutAllChild(xuanzhangjie);
        UIManager.fadeOutAllChild(setCard);
    }
    [Action("chapter2")]
    public void transitionToChapter2() // 打开 闯关模式 -> 第二章 菜单
    {
        if (Utils.scene != "GameMenu" || DataManager.Instance.data.chapter < 2) return;
        sb.SetActive(true);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            UIManager.fadeInAllChild(chapt2);
        });
        UIManager.fadeOutAllChild(xuanzhangjie);
        UIManager.fadeOutAllChild(setCard);
    }

    [Action("shop")]
    public void transitionToShop() // 打开商店
    {
        if (Utils.scene != "GameMenu") return;
        menu.GetComponent<RectTransform>().DOAnchorPos(new Vector2(404, 833), 0.2f);
        logo.transform.DOPath(new Vector3[] { new Vector3(6.6f, 0, 29) }, 0.2f, PathType.CatmullRom).SetEase(Ease.OutQuad);
        welcomeText.SetActive(false);
        sb.SetActive(false);
        DialogSetName.Instance.setName.SetActive(false);
        UIManager.scaleOut(menu2);
        Sounds.zamboni.playWithPitch();
        var shadow = Instantiate(DialogMainShop.Instance.shopCarShadow);
        DOVirtual.DelayedCall(2f, () =>
        {
            Instantiate(DialogMainShop.Instance.shopCar);
            DOVirtual.DelayedCall(0.9f, () =>
            {
                Destroy(shadow);
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    DialogMainShop.Instance.shopUI.SetActive(true);
                    DialogMainShop.Instance.showShopItem();
                });
            });
        });
        UIManager.fadeOutAllChild(xuanzhangjie);
        UIManager.fadeOutAllChild(setCard);
    }
    [Action("mainmenu")]
    public void transitionToChoose() // 返回主菜单
    {
        if (Utils.scene != "GameMenu") return;
        
        DOVirtual.DelayedCall(0.5f, () =>
        {
            Sounds.rollin.play();
        });
        DialogMainShop.Instance.hideShopItem();
        DialogMainShop.Instance.shopUI.SetActive(false);
        CardManager.Instance.stopSetCard();
        DialogSetName.Instance.setName.SetActive(false);
        UIManager.fadeOutAllChild(xuanzhangjie);
        UIManager.fadeOutAllChild(chapt1);
        UIManager.fadeOutAllChild(chapt2);
        UIManager.fadeOutAllChild(miniGameUI);
        UIManager.fadeOutAllChild(setCard);
        UIManager.fadeOutAllChild(pvpUI);
        welcomeText.SetActive(false);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            whiteBg.transform.DOPath(new Vector3[] { new Vector3(30, 0, 0) }, 0.2f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                UIManager.scaleIn(menu2);
                menu.GetComponent<RectTransform>().DOAnchorPos(new Vector2(404, -40), 0.2f).OnComplete(() =>
                {
                    welcomeText.SetActive(true);
                });
                logo.transform.DOPath(new Vector3[] { new Vector3(6.6f, -5.8f, 29) }, 0.2f, PathType.CatmullRom).SetEase(Ease.OutQuad);
            });
        });
        if (DataManager.Instance.data.playerName != null) nameTxt.text = DataManager.Instance.data.playerName;
    }
    [Action("PFW")]
    public void transitionToSetCard() // 打开 备战 菜单
    {
        if (Utils.scene != "GameMenu") return;
        if (DataManager.Instance.data.cardCount < 8)
        {
            DialogWarning.Instance.notEnoughCard();
        }
        else
        {
            DialogPFW.Instance.setCardUiTemp = 0;
            DialogSetName.Instance.setName.SetActive(false);
            UIManager.fadeOutAllChild(xuanzhangjie);
            UIManager.fadeOutAllChild(chapt1);

            DialogPFW.Instance.setHomeNPCUI.SetActive(false);
            DialogPFW.Instance.setEmojiUI.SetActive(false);
            DialogPFW.Instance.setTowerEntityUI.SetActive(false);
            DialogPFW.Instance.setCardUI.SetActive(true);

            UIManager.scaleOut(menu2);
            menu.GetComponent<RectTransform>().DOAnchorPos(new Vector2(404, 833), 0.2f).OnComplete(() =>
            {
                UIManager.fadeInAllChild(setCard);
            });
            logo.transform.DOPath(new Vector3[] { new Vector3(6.6f, 0, 29) }, 0.2f, PathType.CatmullRom).SetEase(Ease.OutQuad);
            welcomeText.SetActive(false);
            DOVirtual.DelayedCall(0.2f, () =>
            {
                CardManager.Instance.setCard();
                sb.SetActive(false);
            });
        }
    }
    [Action("PVP")]
    public void transitionToPVP() // 打开 对战模式菜单
    {
        if (Utils.scene != "GameMenu") return;
        UIManager.fadeOutAllChild(setCard);
        UIManager.fadeOutAllChild(chapt1);
        UIManager.scaleOut(menu2);
        DialogPVP.Instance.inRoomUI.SetActive(false);
        DialogPVP.Instance.noRoomUI.SetActive(true);
        menu.GetComponent<RectTransform>().DOAnchorPos(new Vector2(404, 833), 0.2f).OnComplete(() =>
        {
            whiteBg.GetComponent<SpriteRenderer>().sprite = ImageManager.Instance.menuBg[1];
            whiteBg.transform.DOPath(new Vector3[] { new Vector3(0, 0, 0) }, 0.2f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                UIManager.fadeInAllChild(pvpUI);
            });
        });
        logo.transform.DOPath(new Vector3[] { new Vector3(6.6f, 0, 29) }, 0.2f, PathType.CatmullRom).SetEase(Ease.OutQuad);
        welcomeText.SetActive(false);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            sb.SetActive(false);
        });
    }
    [Action("minigame")]
    public void transitionToMiniGame()//益智时刻
    {
        if (Utils.scene != "GameMenu") return;
        UIManager.fadeOutAllChild(setCard);
        UIManager.scaleOut(menu2);
        miniGameCups.text = DataManager.Instance.data.cupCount + " / " + "2";
        menu.GetComponent<RectTransform>().DOAnchorPos(new Vector2(404, 833), 0.2f).OnComplete(() =>
        {
            whiteBg.GetComponent<SpriteRenderer>().sprite = ImageManager.Instance.menuBg[2];
            whiteBg.transform.DOPath(new Vector3[] { new Vector3(0, 0, 0) }, 0.2f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                UIManager.fadeInAllChild(miniGameUI);
            });
        });
        logo.transform.DOPath(new Vector3[] { new Vector3(6.6f, 0, 29) }, 0.2f, PathType.CatmullRom).SetEase(Ease.OutQuad);
        welcomeText.SetActive(false);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            sb.SetActive(false);
        });
    }
    [Action("quit game")]
    public void close()//退出游戏
    {
        Application.Quit();
    }
    /// <summary>
    /// 通过UI开始闯关模式
    /// </summary>
    /// <param name="chapter"></param>
    /// <param name="level"></param>
    public void startGame(int chapter, int level)
    {
        changeScene(GameMode.Normal, chapter, level,10);
    }
    [Action("start SRCS")]
    /// <summary>
    /// 通过UI开始尸如潮水
    /// </summary>
    /// <param name="chapter"></param>
    /// <param name="level"></param>
    public void startSRCS()
    {
        changeScene(GameMode.SRCS,100, 1,10);
    }
    /// <summary>
    /// 转换场景
    /// </summary>
    /// <param name="chapter"></param>
    /// <param name="level"></param>
    public void changeScene(GameMode gameMode, int chapter, int level,float speedMagn = 1)
    {
        UIManager.fadeOutAllChild(xuanzhangjie);
        UIManager.fadeOutAllChild(chapt1);
        UIManager.fadeOutAllChild(chapt2);
        UIManager.fadeOutAllChild(miniGameUI);
        UIManager.fadeOutAllChild(setCard);
        DOVirtual.DelayedCall(0.2f, () =>
        {
            ImageManager.Instance.jiesuan.SetActive(true);
            var fdOut = ImageManager.Instance.jiesuan.GetComponent<UI_FadeInFadeOut>();
            fdOut.alphaSpeed *= speedMagn;
            fdOut.UI_FadeIn_Event();
            DOVirtual.DelayedCall(5f / speedMagn, () =>
            {
                BridgeManager.Instance.chapter = chapter;
                BridgeManager.Instance.level = level;
                BridgeManager.Instance.gameMode = gameMode;
                SceneManager.LoadScene("Game");
            });
        });
    }
}
