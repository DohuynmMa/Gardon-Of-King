using UnityEngine;
using TMPro;
using DG.Tweening;
using Assets.Scripts.Utils;
public enum DialogingNpcType
{
    Dave,
    King
}

public class DialogManager : MonoBehaviour
{
    public DialogingNPC npcDave;//说话的戴夫NPC
    public DialogingNPC npcKing;//说话的国王NPC

    public int count;//对话次数

    public GameObject dialogUICanvas;//对话框Canvas
    public GameObject npcBox;//装npc有关的父物体
    public Dialog dialogUI;//对话框

    public TextMeshProUGUI dialogText;//对话文字
    public TextMeshProUGUI dialogText2;//点击继续
    public TextMeshProUGUI skipTip;//快进提示
    public TextMeshProUGUI tishiText1;//关卡提示1
    public TextMeshProUGUI tishiText2;//关卡提示2
    public GameObject gaoshi;//关卡告示牌OBJ
    public GameObject zbhks;//准备好开始PREFAB
    //属性
    public static DialogManager Instance { get; private set; }
    public BridgeManager bm { get => BridgeManager.Instance; }
    public GameManager gm { get => GameManager.Instance; }
    public DataManager dm { get => DataManager.Instance; }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if(bm != null)
        {
            dialogUI.chapter = bm.chapter;
            dialogUI.level = bm.level;
        }
        if(dialogUI.level == 0 || dialogUI.chapter == 0)
        {
            dialogUI.level = 2;
            dialogUI.chapter = 1;
        }
        skipTip.text = Application.isMobilePlatform ? "长按 快进" : "“X”快进";
        //对话的初始化
        npcBox.SetActive(true);
        if (gm.gameMode == GameMode.Normal)
        {
            gaoshi.SetActive(false);
            npcDave.anim.enabled = false;
            npcKing.anim.enabled = false;
            npcDave.gameObject.SetActive(false);
            npcKing.gameObject.SetActive(false);
            
            //主线的阳光设置为固定,所以没把bm的设置往里面写
            if (dialogUI.chapter == 1)
            {
                
                Utils.changeBackground(BackgroundType.Day);
                if (dialogUI.level == 1)
                {
                    if (dm.data.timeC1L1 == 0)
                    {
                        npcDave.gameObject.SetActive(true);
                        count = 1;
                        c1l1();
                        dialogText.raycastTarget = false;
                        dialogText2.raycastTarget = false;
                    }
                    else
                    {
                        this.showTargetDialogAndStartGame(
                            "游戏目标:\n\n突破戴夫的防御",
                            "注意:左上角有D的卡牌为临时卡牌,将在获得新卡牌后依次从卡牌列表中删除.",
                            Musics.第一章bgm, dialogUI.chapter, dialogUI.level
                        );
                    }
                }
                else if (dialogUI.level == 2)
                {
                    if (dm.data.timeC1L2 == 0)
                    {
                        npcDave.gameObject.SetActive(true);
                        count = 1;
                        c1l2();
                        dialogText.raycastTarget = false;
                        dialogText2.raycastTarget = false;
                    }
                    else
                    {
                        this.showTargetDialogAndStartGame(
                            "游戏目标:\n\n突破戴夫的防御",
                            "注意:左上角有D的卡牌为临时卡牌,将在获得新卡牌后依次从卡牌列表中删除.",
                            Musics.第一章bgm, dialogUI.chapter, dialogUI.level
                        );
                    }
                }
                else if (dialogUI.level == 3)
                {
                    this.showTargetDialogAndStartGame(
                        "游戏目标:\n\n突破戴夫的防御",
                        "注意:左上角有D的卡牌为临时卡牌,将在获得新卡牌后依次从卡牌列表中删除.",
                        Musics.第一章bgm, dialogUI.chapter, dialogUI.level
                    );
                }
                else if (dialogUI.level == 4)
                {
                    if (dm.data.timeC1L4 == 0)
                    {
                        npcDave.gameObject.SetActive(true);
                        count = 1;
                        c1l4();
                        dialogText.raycastTarget = false;
                        dialogText2.raycastTarget = false;
                    }
                    else
                    {
                        this.showTargetDialogAndStartGame(
                            "游戏目标:\n\n突破戴夫的防御",
                            "注意:左上角有D的卡牌为临时卡牌,将在获得新卡牌后依次从卡牌列表中删除.",
                            Musics.第一章bgm, dialogUI.chapter, dialogUI.level
                        );
                    }
                }
                else if (dialogUI.level == 5)
                {
                    if (dm.data.timeC1L5 == 0)
                    {
                        npcDave.gameObject.SetActive(true);
                        count = 1;
                        c1l5();
                        dialogText.raycastTarget = false;
                        dialogText2.raycastTarget = false;
                    }
                    else
                    {
                        this.showTargetDialogAndStartGame(
                            "游戏目标:\n\n突破戴夫的防御",
                            "警告:请不要惊动对面二楼的戴夫!",
                            Musics.第一章bgm, dialogUI.chapter, dialogUI.level
                        );
                    }
                }
                else if (dialogUI.level == 6 || dialogUI.level == 7)
                {
                    this.showTargetDialogAndStartGame(
                        "游戏目标:\n\n突破戴夫的防御",
                        "警告:请不要惊动对面二楼的戴夫!",
                        Musics.第一章bgm, dialogUI.chapter, dialogUI.level
                    );
                }
                else if (dialogUI.level == 8)
                {
                    if (dm.data.timeC1L8 == 0)
                    {
                        npcDave.gameObject.SetActive(true);
                        count = 1;
                        c1l8();
                        dialogText.raycastTarget = false;
                        dialogText2.raycastTarget = false;
                    }
                    else
                    {
                        this.showTargetDialogAndStartGame(
                            "游戏目标:\n\n突破戴夫的防御",
                            "警告:在戴夫被击败之前,敌方房子为无敌状态!!",
                            Musics.戴夫BOSS一阶段, dialogUI.chapter, dialogUI.level
                        );
                    }
                }
                else print("null level");
            }
            else if (dialogUI.chapter == 2)
            {
                Utils.changeBackground(BackgroundType.Night);
                if (dialogUI.level == 1)
                {
                    if (dm.data.timeC2L1 == 0)
                    {
                        npcDave.gameObject.SetActive(true);
                        count = 1;
                        c2l1();
                        dialogText.raycastTarget = false;
                        dialogText2.raycastTarget = false;
                    }
                    else
                    {
                        this.showTargetDialogAndStartGame(
                            "游戏目标:\n\n先适应一下环境吧?",
                            "毕竟...之后有无穷无尽的试炼等着你?",
                            Musics.第二章bgm, dialogUI.chapter, dialogUI.level
                        );
                    }
                }
                else if(dialogUI.level == 2)
                {
                    if (dm.data.timeC2L2 == 0)
                    {
                        npcKing.gameObject.SetActive(true);
                        count = 1;
                        c2l2();
                        dialogText.raycastTarget = false;
                        dialogText2.raycastTarget = false;
                    }
                    else
                    {
                        this.showTargetDialogAndStartGame(
                            "游戏目标:\n\n击溃老国王的简易防线",
                            "你的能见度似乎越来越低,但请不要害怕...",
                            Musics.第二章bgm, dialogUI.chapter, dialogUI.level
                        );
                    }
                }
                else if (dialogUI.level == 3)
                {
                    if (dm.data.timeC2L3 == 0)
                    {
                        npcKing.gameObject.SetActive(true);
                        count = 1;
                        c2l3();
                        dialogText.raycastTarget = false;
                        dialogText2.raycastTarget = false;
                    }
                    else
                    {
                        this.showTargetDialogAndStartGame(
                            "游戏目标:\n\n击溃老国王的简易防线",
                            "你的能见度似乎越来越低,但请不要害怕...",
                            Musics.第二章bgm, dialogUI.chapter, dialogUI.level
                        );
                    }
                }
                else if (dialogUI.level == 4)
                {
                    if (dm.data.timeC2L4 == 0)
                    {
                        npcKing.gameObject.SetActive(true);
                        count = 1;
                        c2l4();
                        dialogText.raycastTarget = false;
                        dialogText2.raycastTarget = false;
                    }
                    else
                    {
                        this.showTargetDialogAndStartGame(
                            "游戏目标:\n\n击溃老国王的简易防线",
                            "现在是光线最暗的时候,希望不要有坏事发生..",
                            Musics.第二章bgm, dialogUI.chapter, dialogUI.level
                        );
                    }
                }
                //todo more levels
                else print("null level");
            }
            else if (dialogUI.chapter == 99)
            {
                Utils.changeBackground(BackgroundType.Day);
                if (dialogUI.level == 1)
                {
                    var t = TestManager.Instance;
                    this.showTargetDialogAndStartGame(
                        "Test",
                        "测试关卡",
                        Musics.第一章bgm, dialogUI.chapter, dialogUI.level,t.测试关卡开始时阳光数,t.测试关卡最高阳光数,t.测试关卡阳光增长倍数
                    );
                }
            }
            else print("null chapter");
        }
        if (gm.gameMode == GameMode.SRCS)
        {
            gaoshi.SetActive(false);
            npcDave.anim.enabled = false;
            npcKing.anim.enabled = false;
            npcDave.gameObject.SetActive(false);
            npcKing.gameObject.SetActive(false);
            Utils.changeBackground(BackgroundType.Day);
            if (!dm.data.havePlayedSCRS)
            {
                npcDave.gameObject.SetActive(true);
                count = 1;
                SCRS();
                dialogText.raycastTarget = false;
                dialogText2.raycastTarget = false;
            }
            else
            {
                this.showTargetDialogAndStartGame(
                    "游戏目标:\n\n抵御莫名其妙的僵尸",
                    "注意:请保护好房门!",
                    Musics.花园bgm, 100, 0,7,10,1,true,GameMode.SRCS
                );
            }
        }
        if (gm.gameMode == GameMode.MultiPlayer)
        {
            gaoshi.SetActive(false);
            npcDave.anim.enabled = false;
            npcKing.anim.enabled = false;
            npcDave.gameObject.SetActive(false);
            npcKing.gameObject.SetActive(false);
            Utils.changeBackground(bm.backgroundType);
            this.showTargetDialogAndStartGame(
                "游戏目标:\n\n击败你的对手",
                "",
                Musics.UltimateBattle, 100, 0, bm.startSunPoint, bm.maxSunPoint, bm.sunPointAddSpeed, true, GameMode.MultiPlayer
            );
        }
        if (gm.gameMode.isMiniGame())
        {
            gaoshi.SetActive(false);
            npcDave.anim.enabled = false;
            npcKing.anim.enabled = false;
            npcDave.gameObject.SetActive(false);
            npcKing.gameObject.SetActive(false);
            float sunSpeed = 1f;
            string aim = "";
            string tips = "";
            switch (gm.gameMode)
            {
                case GameMode.MiniGame_TETF:
                    sunSpeed = 1f;
                    aim = "击败敌人";
                    tips = "被击败的单位会被策反,似乎可通过某些方式避免单位被策反?";
                    break;
                case GameMode.MiniGame_WGWE:
                    sunSpeed = 3f;
                    aim = "击败敌人";
                    tips = "一旦单位开始攻击,单位会每隔1.5s爆炸,卡牌的选择很重要.";
                    break;
                case GameMode.MiniGame_BS:
                    sunSpeed = 3f;
                    aim = "喂饱戴夫";
                    tips = "喂饭的同时也要当心你的家门.";
                    break;
            }
            Utils.changeBackground(BackgroundType.Day);
            this.showTargetDialogAndStartGame(
            "游戏目标:\n\n" + aim,
            tips,
            Musics.Loonboon, dialogUI.chapter, dialogUI.level, 7, 20, sunSpeed, true, gm.gameMode
            );
        }
    }
    public void showDialog(string txt, float animateTime = -1, DialogingNpcType dialogingNpcType = DialogingNpcType.Dave)
    {
        dialogUICanvas.SetActive(true);
        dialogText.text = txt;
        System.Action sound = null;
        if (animateTime < 0) return;
        switch (dialogingNpcType)
        {
            case DialogingNpcType.Dave:
                npcDave.anim.SetBool("saying", true);
                DOVirtual.DelayedCall(animateTime, () => npcDave.anim.SetBool("saying", false));
                sound = animateTime >= 1.5f ? daveLongSaySound : daveShortSaySound;
                break;
             case DialogingNpcType.King:
                sound = kingSmile;
                break;
        }
        dialogUI.GetComponent<RectTransform>().localScale = new Vector3(1.8f * (dialogingNpcType == DialogingNpcType.Dave ? 1 : -1), 1.8f, 1.8f);
        if(sound != null) sound.Invoke();
    }
    public void stopDialoging(string tips1, string tips2, Musics bgm) // 停止对话
    {
        dialogUICanvas.SetActive(false);
        Musics.等待音乐.play(true);
        gaoshi.SetActive(true);
        tishiText1.text = tips1;
        tishiText2.text = tips2;
        npcDave.anim.enabled = false;
        npcKing.anim.enabled = false;
        npcDave.transform.DOPath(new Vector3[] { new Vector3(-12f, -17.92f, 0) }, 0.1f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            npcDave.gameObject.SetActive(false);
            npcKing.gameObject.SetActive(false);
            gaoshi.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -14f), TestManager.Instance.快进开始游戏前木牌动画 ? 0.01f : 1f).OnComplete(() => {
                npcDave.anim.SetBool("saying", false);
                DOVirtual.DelayedCall(TestManager.Instance.快进开始游戏前木牌动画 ? 0.01f : 1f, () =>
                {
                    this.showTargetDialogAndStartGame(
                        tips1, tips2,
                        bgm, dialogUI.chapter, dialogUI.level, 7, 10, 1, false
                    );
                });
            });
        });
    }
    #region DaveSaySounds
    public void daveShortSaySound()
    {
        npcDave.anim.speed = Random.Range(0.4f, 0.5f);
        new[]
        {
            Voices.戴夫短1, Voices.戴夫短2, Voices.戴夫短3,
        }
        .random().play();
    }
    public void daveLongSaySound()
    {
        npcDave.anim.speed = Random.Range(0.7f, 0.9f);
        new[]
        {
            Voices.戴夫长1, Voices.戴夫长2, Voices.戴夫长3,
            Voices.戴夫长4, Voices.戴夫长5, Voices.戴夫长6,
        }
        .random().play();
    }
    public void daveScreamSound()
    {
        new[]
        {
            Voices.戴夫叫1, Voices.戴夫叫2,
        }
        .random().play();
    }
    public void daveCrazySound()
    {
        SoundsManager.playVoice(11);
    }
    public void kingSmile()
    {
        new[]
        {
            Voices.国王笑1, Voices.国王笑2, Voices.国王笑3,
            Voices.国王笑4, Voices.国王笑5, Voices.国王笑6,
        }
        .random().play();
    }
    #endregion
    #region chapter 1
    public void c1l1()//1-1
    {
        npcDave.transform.DOPath(new Vector3[] { new Vector3(-5.63f, -17.92f, 0) }, 0.1f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            npcDave.anim.enabled = true;
        });

        switch (count)
        {
            case 1:
                showDialog("哦," + dm.data.playerName + ",我的新邻居!", 1f);
                break;
            case 2:
                showDialog("我是疯狂戴夫,欢迎来到这个花园!", 1f);
                break;
            case 3:
                showDialog("或许你在问:接下来要怎么做?", 1f);
                break;
            case 4:
                showDialog("每场游戏你只能携带8张卡牌.", 1f); 
                break;
            case 5:
                showDialog("开局会随机抽取四张卡置于卡槽,剩下四张卡则处于冷却状态.", 2.5f); 
                break;
            case 6:
                showDialog("当你放置卡牌,最先冷却完毕的卡牌会率先回到卡槽部分.", 2.5f);
                break;
            case 7:
                showDialog("放置卡牌需要消耗相应的阳光,因为现在是白天,阳光会随时间恢复.", 2.5f);
                break;
            case 8:
                showDialog("请注意,合理使用阳光是你制胜的关键.", 2.5f);
                break;
            case 9:
                showDialog("那么,如何放置卡牌呢?", 1f);
                break;
            case 10:
                showDialog("只需将卡槽中的卡牌拖动至战场即可.", 1.2f);
                break;
            case 11:
                showDialog("这局我将不放置卡牌.", 1f);
                break;
            case 12:
                showDialog("你还没有卡牌吧?那么...我借你这几张......", 2f);
                break;
            case 13:
                showDialog("使用手中的卡牌,完成我的第一场试炼吧!", 1.5f);
                break;
            case 14:
                npcDave.anim.speed = 1;
                stopDialoging(
                    "游戏目标:\n\n突破戴夫的防御",
                    "注意:左上角有D的卡牌为临时卡牌,将在获得新卡牌后依次从卡牌列表中删除.",
                    Musics.第一章bgm
                );
                break;
            default:
                break;
        }
    }
    public void c1l2()//1-2
    {
        npcDave.transform.DOPath(new Vector3[] { new Vector3(-5.63f, -17.92f, 0) }, 0.1f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            npcDave.anim.enabled = true;
        });

        switch (count)
        {
            case 1:
                showDialog("学的很快嘛!新邻居!", 1f);
                break;
            case 2:
                showDialog("在这次的试炼,你要学会如何防御.", 1f);
                break;
            case 3:
                showDialog("守护家门,卡槽中植物是不错的选择!", 1f);
                break;
            case 4:
                showDialog("我将对你发起进攻,请注意防御!", 1f);
                break;
            case 5:
                npcDave.anim.speed = 1;
                stopDialoging(
                    "游戏目标:\n\n突破戴夫的防御",
                    "注意:左上角有D的卡牌为临时卡牌,将在获得新卡牌后依次从卡牌列表中删除.",
                    Musics.第一章bgm
                );
                break;
            default:
                break;
        }
    }
    public void c1l4()//1-4
    {
        npcDave.transform.DOPath(new Vector3[] { new Vector3(-5.63f, -17.92f, 0) }, 0.1f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            npcDave.anim.enabled = true;
        });

        switch (count)
        {
            case 1:
                showDialog("哦,我的邻居.", 1f);
                break;
            case 2:
                showDialog("我打算送你一个小礼物.", 1f);
                break;
            case 3:
                showDialog("只要你能通过这场试炼,那个礼物就是你的了.", 2f);
                break;
            case 4:
                showDialog("你准备好了吗?", 1f);
                break;
            case 5:
                npcDave.anim.speed = 1;
                stopDialoging(
                    "游戏目标:\n\n突破戴夫的防御",
                    "注意:左上角有D的卡牌为临时卡牌,将在获得新卡牌后依次从卡牌列表中删除.",
                     Musics.第一章bgm
                );
                break;
        }
    }
    public void c1l5()//1-5
    {
        npcDave.transform.DOPath(new Vector3[] { new Vector3(-5.63f, -17.92f, 0) }, 0.1f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            npcDave.anim.enabled = true;
        });

        switch (count)
        {
            case 1:
                showDialog("我的邻居,你的成长速度十分惊人啊!", 1.5f);
                break;
            case 2:
                showDialog("接下来的试炼,我会为你增加一点小难度!", 2f);
                break;
            case 3:
                showDialog("就像...这样!", 1f);
                break;
            case 4:
                npcDave.anim.speed = 1;
                breakScreen();
                break;
            case 5:
                showDialog("为什么我会这么做?", 1f);
                break;
            case 6:
                showDialog("因为我疯掉了!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                npcDave.anim.SetTrigger("shout");
                daveCrazySound();
                break;
            case 7:
                npcDave.anim.speed = 1;
                stopDialoging(
                    "游戏目标:\n\n突破戴夫的防御",
                    "警告:请不要惊动对面二楼的戴夫!",
                     Musics.第一章bgm
                );
                break;
        }
    }
    public void c1l8()//1-8
    {
        npcDave.transform.DOPath(new Vector3[] { new Vector3(-5.63f, -17.92f, 0) }, 0.1f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            npcDave.anim.enabled = true;
        });

        switch (count)
        {
            case 1:
                showDialog("邻居,天色也不早了.", 1.5f);
                break;
            case 2:
                showDialog("看你似乎技术不错.", 2f);
                break;
            case 3:
                showDialog("那么......", 1f);
                break;
            case 4:
                showDialog("我也不让着你了!!!!!!!!!!!!!!");
                npcDave.anim.SetTrigger("shout");
                daveCrazySound();
                break;
            case 5:
                showDialog("我会用绝对的力量,夺回我的铲子和卡牌!!!!");
                npcDave.anim.SetTrigger("shout");
                daveCrazySound();
                break;
            case 6:
                showDialog("畏惧吧!!!!!!");
                npcDave.anim.SetTrigger("shout");
                daveCrazySound();
                break;
            case 7:
                npcDave.anim.speed = 1;
                stopDialoging(
                    "游戏目标:\n\n击败戴夫",
                    "警告:在戴夫被击败之前,敌方房子为无敌状态!!",
                     Musics.戴夫BOSS一阶段
                );
                break;
        }
    }
    #endregion
    #region chapter 2
    public void c2l1()//2-1
    {
        npcDave.transform.DOPath(new Vector3[] { new Vector3(-5.63f, -17.92f, 0) }, 0.1f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            npcDave.anim.enabled = true;
        });

        switch (count)
        {
            case 1:
                showDialog("我的邻居,你通过了我的考验.", 1f);
                break;
            case 2:
                showDialog("你似乎很好奇,为什么我刚才会变成这样?", 1.5f);
                break;
            case 3:
                showDialog("昨天我除完草后闲得无聊,就到处捡东西玩.", 1.5f);
                break;
            case 4:
                showDialog("我看到一瓶粉红色的药水,认为它很好喝,于是便一饮而尽.", 2f);
                break;
            case 5:
                showDialog("喝完之后,我感觉浑身充满了力量!!!!", 1.4f);
                break;
            case 6:
                showDialog("不用担心我的身体,我现在好多了.", 1.4f);
                break;
            case 7:
                showDialog("哦!时间过得好快,已经晚上了.", 1.2f);
                break;
            case 8:
                showDialog("忘记告诉你了,因为我刚才用力过猛,把电线拉坏了.", 1.6f);
                break;
            case 9:
                showDialog("附近似乎没有什么可照明的道具,这将会是一个充满黑暗与刺激的夜晚!", 2.2f);
                break;
            case 10:
                showDialog("我的朋友很快会来找你,你先适应一下这里的环境吧.", 1.6f);
                break;
            case 11:
                showDialog("我先回家了.",-1);
                break;
            case 12:
                npcDave.anim.speed = 1;
                stopDialoging(
                    "游戏目标:\n\n先适应一下环境吧?",
                    "毕竟...之后有无穷无尽的试炼等着你?",
                    Musics.第二章bgm
                );
                break;
            default:
                break;
        }
    }
    public void c2l2()//2-2
    {
        npcKing.transform.DOPath(new Vector3[] { new Vector3(4, -2.55f, 0) }, 0.1f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            npcKing.anim.enabled = true;
        });

        switch (count)
        {
            case 1:
                showDialog("你好啊,挑战者.", 1f,DialogingNpcType.King);
                break;
            case 2:
                showDialog("相信你已经注意到了,今天晚上很黑.", 1f, DialogingNpcType.King);
                break;
            case 3:
                showDialog("这的确很奇怪,所以我们需要保持戒备.", 1f, DialogingNpcType.King);
                break;
            case 4:
                showDialog("现在,你需要想办法通过我的试炼.", 1f, DialogingNpcType.King);
                break;
            case 5:
                showDialog("我是国王,在许多险地指挥过战斗,这点黑暗对于我来说不成问题.", 1f, DialogingNpcType.King);
                break;
            case 6:
                showDialog("但你需要一些时间适应.", 1f, DialogingNpcType.King);
                break;
            case 7:
                showDialog("现在,尝试跨越黑暗,击溃我的简易防线吧!", -1, DialogingNpcType.King);
                break;
            case 8:
                npcKing.anim.speed = 1;
                stopDialoging(
                    "游戏目标:\n\n击溃老国王的简易防线",
                    "你的能见度似乎越来越低,但请不要害怕...",
                    Musics.第二章bgm
                );
                break;
        }
    }
    public void c2l3()//2-3
    {
        npcKing.transform.DOPath(new Vector3[] { new Vector3(4, -2.55f, 0) }, 0.1f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            npcKing.anim.enabled = true;
        });

        switch (count)
        {
            case 1:
                showDialog("你似乎很在意上一局的*不明飞行物*", 1f, DialogingNpcType.King);
                break;
            case 2:
                showDialog("是时候给你介绍一下它们了.", 1f, DialogingNpcType.King);
                break;
            case 3:
                showDialog("我有一种全新的卡牌: *法术牌*.", 1f, DialogingNpcType.King);
                break;
            case 4:
                showDialog("法术牌可以在指定区域施加特定的效果,可以是爆炸,也可以是一滩药水.", 1f, DialogingNpcType.King);
                break;
            case 5:
                showDialog("合理使用法术,可以帮助你扭转战局.", 1f, DialogingNpcType.King);
                break;
            case 6:
                showDialog("在接下来的战斗,你会遇到更多种类的法术.", 1f, DialogingNpcType.King);
                break;
            case 7:
                showDialog("而且,总有一天,你也会集齐它们的.", -1, DialogingNpcType.King);
                break;
            case 8:
                npcKing.anim.speed = 1;
                stopDialoging(
                    "游戏目标:\n\n击溃老国王的简易防线",
                    "你的能见度似乎越来越低,但请不要害怕...",
                    Musics.第二章bgm
                );
                break;
        }
    }
    public void c2l4()//2-4
    {
        npcKing.transform.DOPath(new Vector3[] { new Vector3(4, -2.55f, 0) }, 0.1f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            npcKing.anim.enabled = true;
        });

        switch (count)
        {
            case 1:
                showDialog("嗯,看来时间已经来到午夜了.", 1f, DialogingNpcType.King);
                break;
            case 2:
                showDialog("请不要担心,现在是你能见度最低的时候.", 1f, DialogingNpcType.King);
                break;
            case 3:
                showDialog("之后你的视野会逐渐变亮的.", 1f, DialogingNpcType.King);
                break;
            case 4:
                showDialog("为你的勇气点赞!挑战者!", 1f, DialogingNpcType.King);
                break;
            case 5:
                npcKing.anim.speed = 1;
                stopDialoging(
                    "游戏目标:\n\n击溃老国王的简易防线",
                    "现在是光线最暗的时候,希望不要有坏事发生..",
                    Musics.第二章bgm
                );
                break;
        }
    }
    #endregion
    #region others
    public void SCRS()//尸潮如水新手教程
    {
        npcDave.transform.DOPath(new Vector3[] { new Vector3(-5.63f, -17.92f, 0) }, 0.1f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            npcDave.anim.enabled = true;
            dialogUICanvas.SetActive(true);
        });
        dialogUI.chapter = 100;
        dialogUI.level = 0;

        switch (count)
        {
            case 1:
                showDialog("嘘,伙计,小声点!", 1f);
                break;
            case 2:
                showDialog("看见对面的房子了吗?", 1f);
                break;
            case 3:
                showDialog("最近这个花园发生了一些奇怪的事件.", 1.45f);
                break;
            case 4:
                showDialog("对面的房子内似乎出现了一个<不存在的玩家>.", 1.45f);
                break;
            case 5:
                showDialog("他似乎对你的脑子很感兴趣.", 1.45f);
                break;
            case 6:
                showDialog("在对面没玩家的时候他会开始行动,放置僵尸来攻击你.", 2f);
                break;
            case 7:
                showDialog("这些僵尸异常暴躁,似乎还没经过驯化,不能做成卡牌.", 2.5f);
                break;
            case 8:
                showDialog("请你多加注意,别让家门被攻破.", 2f);
                break;
            case 9:
                showDialog("在这里,你的植物/防御塔/家门生命值会增加,且植物生命不会随时间流失.", 2.5f);
                break;
            case 10:
                showDialog("同时,阳光收集速度也会翻倍.", 1.2f);
                break;
            case 11:
                showDialog("请你利用这些特性,保护好自己!", 1.2f);
                break;
            case 12:
                showDialog("我和国王会在后方给予你支援.", 1f);
                break;
            case 13:
                showDialog("希望我们能早日弄清这件事背后的真相!", 1.3f);
                break;
            case 14:
                showDialog("你可以种植植物到合适区域,并对其进行照顾", 1.8f);
                break;
            case 15:
                showDialog("植物有5分钟生长期,生长期期间,植物会帮助你抵御敌人", 2f);
                break;
            case 16:
                showDialog("生长期过后,便会进入收获期,植物会死亡,你也会获得一定数量金币奖励.", 2.5f);
                break;
            case 17:
                showDialog("同样地,你可以放置僵尸抵御敌方的僵尸.", 1.4f);
                break;
            case 18:
                showDialog("敌方概率会放出特殊的僵尸,击杀它后可将它驯服", 1.8f);
                break;
            case 19:
                showDialog("被驯服的僵尸会变成卡牌,你可以使用这张卡牌参与其他战斗!", 2f);
                break;
            case 20:
                showDialog("战斗期间,你可随时退回到主菜单编辑卡组,然后继续战斗!", 2f);
                break;
            case 21:
                showDialog("金币可用于房子设施升级(修复),也可以前往主菜单-商店购买物品.", 2f);
                break;
            case 22:
                showDialog("第一波僵尸来了!请注意防御!");
                npcDave.anim.SetTrigger("shout");
                daveScreamSound();
                break;
            case 23:
                npcDave.anim.speed = 1;
                stopDialoging(
                    "游戏目标:\n\n抵御莫名其妙的僵尸",
                    "注意:请保护好房门,房门被攻破存档将会丢失!",
                    Musics.花园bgm
                );
                break;
        }
    }
    #endregion
    public void breakScreen()
    {
        npcDave.anim.SetTrigger("shoot");
        DOVirtual.DelayedCall(1.3f, () => {
            CameraManager.Instance.shake(-0.2f);
            SoundsManager.playSounds(37);
            Instantiate(Utils.findEffectByType(AreaEffectType.GlassBroken));
        });
    }

}
