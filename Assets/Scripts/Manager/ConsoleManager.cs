using Assets.Scripts.NetWork;
using Assets.Scripts.NetWork.Packet.Play.Client;
using Assets.Scripts.NetWork.Packet.Play.Server;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class ConsoleManager : MonoBehaviour
{
    public ScreenManager sm { get => ScreenManager.Instance; }
    public static ConsoleManager Instance;
    public ConsoleMessage showingMessagePrefab;
    public List<string> messages = new List<string>();
    public List<ConsoleMessage> showingMessages = new List<ConsoleMessage>();
    public List<string> sentMessageList = new List<string>();
    private static readonly string helpTxt =
        "指令帮助:\n" +
        "生成实体: /s <实体id> <格子id> (e为敌方阵营,g为敌方精英阵营,留空或填其他为己方阵营)\n" +
        "多格子同时生成实体: /s <实体id> [<格子id最小值>,<格子id最大值>] (e为敌方阵营,g为敌方精英阵营,留空或填其他为己方阵营)\n" +
        "击杀除防御塔的全部实体: /k <击杀范围:@a为全体,@e为敌方阵营,@s为己方阵营>\n";
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void onSend(string message)
    {
        if (message == "") return;
        var currentMessage = processMessage(message);
        if (currentMessage == "") return;
        var m = Instantiate(showingMessagePrefab);
        var parent = transform.GetChild(0).GetChild(1).GetChild(1).GetChild(0);
        m.transform.SetParent(parent);
        sentMessageList.Add(message);
        m.text.text = currentMessage;
        messages.Add(currentMessage); showingMessages.Add(m);
        m.waitToDes(10);
        updateShowingMessagePos();
    }
    public string processMessage(string message)
    {
        string currentMessage = "";
        if (message.StartsWith("/"))//command
        {
            if (Utils.scene == "GameMenu") return "";
            if (GameManager.Instance.gameMode == GameMode.MultiPlayer) return "当前版本不支持联机指令(问就是没写完";
            if (message.StartsWith("/k"))
            {
                var sts = message.Split(" ");
                if(sts.Length == 2)
                {
                    var aim = sts[1];
                    if(aim == "@e")
                    {
                        foreach (var e in Utils.findAllEntitiesByGroup(EntityGroup.enemy))
                        {
                            e.entityDie();
                        }
                        currentMessage = DataManager.Instance.data.playerName + "清除了全体敌方单位";
                    }
                    else if(aim == "@s")
                    {
                        foreach (var e in Utils.findAllEntitiesByGroup(EntityGroup.friend))
                        {
                            e.entityDie();
                        }
                        currentMessage = DataManager.Instance.data.playerName + "清除了全体己方单位";
                    }
                    else if(aim == "@a")
                    {
                        foreach (var e in Utils.findAllEntities())
                        {
                            e.entityDie();
                        }
                        currentMessage = DataManager.Instance.data.playerName + "清除了全体单位";
                    }
                    else
                    {
                        showHelp();
                    }
                }
            }
            else if (message.StartsWith("/s"))
            {
                var sts = message.Split(" ");
                if (sts.Length == 2)
                {
                    Enum.TryParse(sts[1], out EntityType entityType);
                    if (!entityType.GetType().IsEnum) entityType = EntityType.PeaShooter;
                    summonEntityByCommand(entityType, Utils.findCellById(10), EntityGroup.friend);
                    currentMessage = DataManager.Instance.data.playerName + "生成了: " + entityType;
                }
                else if (sts.Length == 3)
                {
                    Enum.TryParse(sts[1], out EntityType entityType);
                    var entityGroup = EntityGroup.friend;
                    bool multiCell = sts[2].StartsWith("[") && sts[2].EndsWith("]") && sts[2].Contains(",");
                    if (multiCell)
                    {
                        var trimmedInput = sts[2].Trim(new char[] { '[', ']' });
                        string[] parts = trimmedInput.Split(',');
                        var minCellId = parts[0] == "" ? 0 : int.Parse(parts[0]);
                        var maxCellId = parts[1] == "" ? 1 : int.Parse(parts[1]);
                        for (var i = minCellId; i < maxCellId + 1; i++)
                        {
                            var cellId = i;
                            summonEntityByCommand(entityType, Utils.findCellById(cellId), entityGroup);
                        }
                    }
                    else
                    {
                        var cellId = sts[2].IsNullOrEmpty() ? 10 : int.Parse(sts[2]);
                        if (cellId >= 60) cellId = 10;
                        summonEntityByCommand(entityType, Utils.findCellById(cellId), entityGroup);
                    }
                    currentMessage = DataManager.Instance.data.playerName + "生成了: " + entityType;
                }
                else if (sts.Length == 4)
                {
                    Enum.TryParse(sts[1], out EntityType entityType);
                    var entityGroup = (sts[3] == "e" || sts[3] == "g") ? EntityGroup.enemy : EntityGroup.friend;
                    bool multiCell = sts[2].StartsWith("[") && sts[2].EndsWith("]") && sts[2].Contains(",");
                    if (multiCell)
                    {
                        var trimmedInput = sts[2].Trim(new char[] { '[', ']' });
                        string[] parts = trimmedInput.Split(',');
                        var minCellId = parts[0] == "" ? 0 : int.Parse(parts[0]);
                        var maxCellId = parts[1] == "" ? 1 : int.Parse(parts[1]);
                        for (var i = minCellId; i < maxCellId + 1; i++)
                        {
                            var cellId = i;
                            summonEntityByCommand(entityType, Utils.findCellById(cellId), entityGroup, sts[3] == "g");
                        }
                    }
                    else
                    {
                        var cellId = sts[2].IsNullOrEmpty() ? 10 : int.Parse(sts[2]);
                        if (cellId >= 60) cellId = 10;
                        summonEntityByCommand(entityType, Utils.findCellById(cellId), entityGroup, sts[3] == "g");
                    }
                    currentMessage = DataManager.Instance.data.playerName + "生成了: " + (sts[3] == "g" ? "加强版" : "") + entityType;
                }
                else showHelp();
            }
            else
            {
                showHelp();
            }
        }
        else//chat
        {
            currentMessage = DataManager.Instance.data.playerName + "说: " + message;

            //客户端发包
            if (MultiGameManager.client != null)
            {
                MultiGameManager.client.Send(new PlayClientOnChat(DataManager.Instance.data.playerName, message));
            }
            //服务端发包
            if(MultiGameManager.server != null)
            {
                foreach (var userId in NetworkServerService.users.Keys)
                {
                    var user = NetworkServerService.users[userId];
                    if (user == null) continue;
                    user.Send(new PlayServerOnChat(DataManager.Instance.data.playerName,message));
                }
            }
        }
        return currentMessage;
    }
    public void updateShowingMessagePos()
    {
        if (showingMessages.Count > 6)
        {
            var toClean = showingMessages[0];
            showingMessages.RemoveAt(0);
            toClean.onMessageDestroy();
        }
        foreach (var m in showingMessages)
        {
            var mr = m.GetComponent<RectTransform>();
            var bl = (float)Screen.safeArea.height / (float)Screen.height;
            mr.localPosition = new Vector3(0, (showingMessages.Count - showingMessages.IndexOf(m)) * 55f, 0) / bl;
        }
        sm.updateMessageObjScale();
    }
    public static void summonEntityByCommand(EntityType entityType, Cell cell,EntityGroup entityGroup,bool golden = false)
    {
        if (cell == null)
        {
            print("null cell");
            return;
        }
        if (cell.currentEntity != null && entityType.isPlant())
        {
            Debug.Log("格子已存在实体! Cell Id:" + cell.ID);
            return;
        }
        var entityPrefab = HandManager.Instance.getEntityPrefeb(entityType);
        if (entityPrefab == null) entityPrefab = HandManager.Instance.getEntityPrefeb(EntityType.NormalZombie);
        var entity = Instantiate(entityPrefab);
        cell.addEntityDirectly(entity, entityGroup, golden);
    }
    private void showHelp()
    {
        var helpMS = helpTxt.Split('\n');
        foreach (var line in helpMS)
        {
            if (line.Length > 0)
            {
                var m = Instantiate(showingMessagePrefab);
                var parent = transform.GetChild(0).GetChild(1).GetChild(1).GetChild(0);
                m.transform.SetParent(parent);
                sentMessageList.Add("/help");
                m.text.text = line;
                messages.Add(line); showingMessages.Add(m);
                m.waitToDes(10);
            }
        }
        updateShowingMessagePos();
    }
}
