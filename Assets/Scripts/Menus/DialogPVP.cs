using Assets.Scripts.Utils;
using System;
using TMPro;
using UnityEngine;
using Assets.Scripts.NetWork.Packet.Play.Client;
using Assets.Scripts.NetWork.Packet.Play.Server;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.NetWork;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogPVP : MonoBehaviour, UIHelper
{
    public static DialogPVP Instance { get; private set; }
    public UIManager ui { get => UIManager.Instance; }
    public GameObject 真正的联机总UI;
    public GameObject 局域网总UI;
    public GameObject 联机大厅总UI;
    public TextMeshProUGUI playerName1;
    public TextMeshProUGUI playerName2;
    public TextMeshProUGUI p1;
    public TextMeshProUGUI p2;
    public TextMeshProUGUI roomStateTXT;//房间状态

    public GameObject player1;
    public GameObject player2;
    public GameObject inRoomUI;
    public GameObject noRoomUI;

    public GameObject kickButton1;

    public TMP_InputField 创建房间IP;
    public TMP_InputField 创建房间端口号;
    public TMP_InputField 加入房间IP;
    public TMP_InputField 加入房间端口号;
    public GameObject 创建房间UI;
    public GameObject 加入房间UI;
    public TextMeshProUGUI 开始或准备按钮的文字;

    public GameObject 联机设置_背景;
    public GameObject 联机设置_主内容;
    public TMP_InputField 阳光速度;
    public TMP_InputField 最初阳光;
    public TMP_InputField 最高阳光;
    public float sunPointAddSpeed;
    public float startSunPoint;
    public float maxSunPoint;
    public List<Sprite> miniMapSprite;
    public Image mapSpriteImg;
    public int settingMapTemp;//0: day 1: night 2: random
    public BackgroundType backgroundType;
    public TextMeshProUGUI 设置更改提示文字;
    public string GetName()
    {
        return "PVP";
    }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        UIManager.registerUI(this);
    }
    [Action("LAN")]
    public void 局域网界面()
    {
        局域网总UI.SetActive(true);
        真正的联机总UI.SetActive(false);
    }
    [Action("return to choose mode")]
    public void 返回选择联机模式()
    {
        局域网总UI.SetActive(false);
        真正的联机总UI.SetActive(true);
    }
    public void createGameNetty()
    {
        string ip = 创建房间IP.text;
        int port = int.Parse(创建房间端口号.text);
        NettyMultiGameManager.Initialize(ip, port);
        playerName1.text = DataManager.Instance.data.playerName;

        player1.SetActive(true);
        player2.SetActive(false);
        p1.text = "P1 √";
        开始或准备按钮的文字.color = Color.red;
        开始或准备按钮的文字.text = "等待玩家";
        print("创建了房间");

        inRoomUI.SetActive(true);
        noRoomUI.SetActive(false);

        创建房间UI.SetActive(false);
    }
    [Action("to create room")]
    public void toCreateRoom()
    {
        创建房间UI.SetActive(true);
    }
    [Action("to join room")]
    public void toJoinRoom()
    {
        加入房间UI.SetActive(true);
    }
    [Action("create room")]
    public void createGame()
    {
        int.TryParse(创建房间端口号.text, out int port);
        if (port <= 0 || port > 65535)
        {
            ShowError("请输入正确端口号");
            return;
        }
        if (!Utils.IsPortAvailable(port))
        {
            ShowError("端口不可用");
            return;
        }
        MultiGameManager.server = NetworkHelper.startServer(port);
        playerName1.text = DataManager.Instance.data.playerName;

        player1.SetActive(true);
        player2.SetActive(false);
        p1.text = "P1 √";
        开始或准备按钮的文字.color = Color.red;
        开始或准备按钮的文字.text = "等待玩家";
        print("创建了房间");

        inRoomUI.SetActive(true);
        noRoomUI.SetActive(false);
        resetSettings();
        创建房间UI.SetActive(false);
    }
    [Action("join room")]
    public void joinGame()
    {
        int.TryParse(加入房间端口号.text, out int port);
        if (port <= 0 || port > 65535)
        {
            ShowError("请输入正确端口号");
            return;
        }

        string url = $"ws://{加入房间IP.text}:{port}";
        try
        {
            player1.SetActive(true);
            player2.SetActive(true);

            playerName2.text = DataManager.Instance.data.playerName;

            MultiGameManager.client = NetworkHelper.startClientAndConnect(url);
        }
        catch (System.ArgumentException)
        {
            ShowError($"连接失败，URI 无效");
        }
        catch (System.Exception)
        {
            ShowError($"连接失败");
        }
    }
    [Action("leave room")]
    public void leaveGameRoom()
    {
        if (MultiGameManager.server != null)
        {
            MultiGameManager.server.Stop();
            MultiGameManager.server = null;
        }
        if (MultiGameManager.client != null)
        {
            MultiGameManager.client.CloseAsync();
            MultiGameManager.client = null;
        }
        创建房间UI.SetActive(false);
        加入房间UI.SetActive(false);
        closeSettings();
        inRoomUI.SetActive(false);
        noRoomUI.SetActive(true);
    }
    [Action("start or prepare")]
    public void startOrPrepare()
    {
        if (MultiGameManager.server != null)
        {
            if (checkAllPlayerIsPrepared())
            {
                startMultiPlayerGame();
            }
        }
        if (MultiGameManager.client != null)
        {
            MultiGameManager.isPrepared = !MultiGameManager.isPrepared;
            MultiGameManager.client.Send(new PlayClientPrepare(MultiGameManager.isPrepared));

            if (MultiGameManager.isPrepared)
            {
                Sounds.bitClick.play();
                开始或准备按钮的文字.color = Color.red;
                开始或准备按钮的文字.text = "取消准备";
                p2.text = "P2 √";
            }
            else
            {
                开始或准备按钮的文字.color = Color.green;
                开始或准备按钮的文字.text = "准备";
                p2.text = "P2 X";
            }
        }
    }
    [Action("settings")]
    public void openSettings()
    {
        if (MultiGameManager.server == null)
        {
            ShowError("你不是房主,无权修改设置.");
            return;
        }
        最初阳光.interactable = true;
        最高阳光.interactable = true;
        阳光速度.interactable = true;
        联机设置_主内容.SetActive(true);
        联机设置_背景.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 150), 0.3f).SetEase(Ease.OutQuad);
        设置更改提示文字.gameObject.SetActive(false);
    }
    [Action("close settings")]
    public void closeSettings()
    {
        联机设置_主内容.SetActive(false);
        联机设置_背景.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 1000), 0.3f).SetEase(Ease.OutQuad);
        if (MultiGameManager.server != null)
        {
            float.TryParse(阳光速度.text, out sunPointAddSpeed);
            float.TryParse(最初阳光.text, out startSunPoint);
            float.TryParse(最高阳光.text, out maxSunPoint);
            //捡查数据合理性
            if(startSunPoint > maxSunPoint)
            {
                maxSunPoint = startSunPoint;
                最初阳光.text = 最高阳光.text;
            }
            if(startSunPoint < 0)
            {
                startSunPoint = 0;
                最初阳光.text = "0";
            }
            if(maxSunPoint <= 0)
            {
                maxSunPoint = 10;
                最高阳光.text = "10";
            }
            if(sunPointAddSpeed <= 0)
            {
                sunPointAddSpeed = 1;
                阳光速度.text = "1";
            }

            switch (settingMapTemp)
            {
                case 0:
                    backgroundType = BackgroundType.Day;
                    break;
                case 1:
                    backgroundType = BackgroundType.Night;
                    break;
                case 2:
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        backgroundType = BackgroundType.Night;
                    }
                    else
                    {
                        backgroundType = BackgroundType.Day;
                    }
                    break;
            }
            foreach (var userId in NetworkServerService.users.Keys)
            {
                var user = NetworkServerService.users[userId];
                if (user == null) continue;
                user.Send(new PlayServerChangeGameSettings(new MultiGameSetting(sunPointAddSpeed, maxSunPoint, startSunPoint, backgroundType),settingMapTemp));
            }
        }

    }
    public void onHosterChangeSettings()
    {
        if (MultiGameManager.client == null) return;
        print("房主更改了设置");
        联机设置_背景.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 1000), 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            最初阳光.interactable = false;
            最高阳光.interactable = false;
            阳光速度.interactable = false;
            联机设置_主内容.SetActive(true);
            设置更改提示文字.gameObject.SetActive(true);
            联机设置_背景.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 150), 0.3f).SetEase(Ease.OutQuad);
        });
    }
    public void resetSettings()
    {
        settingMapTemp = 0;
        sunPointAddSpeed = 1;
        maxSunPoint = 10;
        startSunPoint = 7;
        最初阳光.text = "7";
        最高阳光.text = "10";
        阳光速度.text = "1";
        mapSpriteImg.sprite = miniMapSprite[0];
    }
    public void settingChangeMapTemp()
    {
        if (MultiGameManager.server != null){
            settingMapTemp++;
            if (settingMapTemp > 2)
            {
                settingMapTemp = 0;
            }
        }
        mapSpriteImg.sprite = miniMapSprite[settingMapTemp];
    }
    public void kickPlayer(int playerID)
    {
        if (MultiGameManager.server == null) return;
        NetworkServerService.getUserById(playerID).Send(new PlayServerKickPlayer());
    }
    public void startMultiPlayerGame()
    {
        print("游戏开始");
        var bm = BridgeManager.Instance;
        bm.startSunPoint = startSunPoint;
        bm.maxSunPoint = maxSunPoint;
        bm.sunPointAddSpeed = sunPointAddSpeed;
        bm.backgroundType = backgroundType;
        DialogMainMenu.Instance.changeScene(GameMode.MultiPlayer, 100, 0,10);
        foreach (var userId in NetworkServerService.users.Keys)
        {
            var user = NetworkServerService.users[userId];
            if (user == null) continue;
            user.Send(new PlayServerGameStart(new MultiGameSetting(sunPointAddSpeed,maxSunPoint,startSunPoint,backgroundType)));
        }
    }
    public bool checkAllPlayerIsPrepared()
    {
        if (MultiGameManager.server == null) return false;
        if (NetworkServerService.users.Count == 0) return false;
        foreach (int userId in NetworkServerService.users.Keys)
        {
            NetworkServerService user = NetworkServerService.users[userId];
            if (user == null) continue;
            if (!user.isPrepared) return false;
        }

        return true;
    }
    public void ShowError(string message)
    {
        Utils.showDialog(message, 18.1f, Color.white, "确 定", 20f, Color.white);
        Sounds.buzzer.play();
    }
}
