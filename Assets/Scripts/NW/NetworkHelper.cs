using Assets.Scripts.NetWork.Packet;
using Assets.Scripts.NetWork.Server;
using System;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using Assets.Scripts.NetWork.Packet.Play.Client;
public enum ServerOrClient
{
    None,
    Server,
    Client
}
namespace Assets.Scripts.NetWork
{
    public static class NetworkHelper
    {
        public static void Send(this WebSocket ws, IPacket packet)
        {
            Send(ws, packet, it => { });
        }
        public static void Send(this WebSocket ws, IPacket packet, Action<bool> result)
        {
            string message = PacketManager.Serialize(packet);
            ws.SendAsync(message, result);
        }
        /// <summary>
        /// 开启服务端
        /// </summary>
        /// <param name="port">端口号</param>
        public static WebSocketServer startServer(int port)
        {
            var server = new WebSocketServer(port);
            server.Start();
            server.AddWebSocketService<NetworkServerService>("/"); // 在根目录添加一个服务
            return server;
        }

        /// <summary>
        /// 开启客户端并连接
        /// </summary>
        /// <param name="host">服务端地址</param>
        public static WebSocket startClientAndConnect(string host)
        {
            var client = new WebSocket(host);
            client.ConnectAsync();
            client.OnOpen += (sender, e) =>
            {
                var dat = DataManager.Instance.data;
                var u = DialogPVP.Instance;
                Utils.Utils.run(() =>
                {
                    MultiGameManager.isPrepared = false;
                    u.kickButton1.SetActive(false);
                    u.p2.text = "P2 X";
                    u.p1.text = "P1 √";
                    u.开始或准备按钮的文字.color = Color.green;
                    u.开始或准备按钮的文字.text = "准备";
                    u.inRoomUI.SetActive(true);
                    u.noRoomUI.SetActive(false);
                    u.加入房间UI.SetActive(false);
                    client.Send(new PlayClientPlayerJoinRoom(dat.playerName, dat.towerEntity, dat.homeNpc, dat.cardList, dat.hasShovel, MultiGameManager.version));
                });
            };
            client.OnMessage += (sender, e) =>
            {
                if (e.IsText)
                {
                    var message = e.Data;
                    PacketManager.resolveAndHandlePacket(Side.Server, 0, message);
                }
            };
            client.OnError += (sender, e) =>
            {
                Debug.LogException(e.Exception);
            };
            client.OnClose += (sender, e) =>
            {
                var code = e.Code;
                var reason = e.Reason;

                Debug.Log("服务端断开了连接.代码" + code + "原因" + reason);
                var u = DialogPVP.Instance;
                Utils.Utils.run(() =>
                {
                    if(Utils.Utils.scene == "Game")
                    {
                        if (GameManager.Instance.gameMode == GameMode.MultiPlayer)
                        {
                            Utils.Utils.showDialog("游戏结束\n对方断开了连接", 18.1f, Color.white, "退 出", 20f, Color.white);
                            GameManager.Instance.gameOver(EntityGroup.friend);
                        }
                    }
                    else if(Utils.Utils.scene == "GameMenu")
                    {
                        u.playerName2.text = "";
                        u.roomStateTXT.text = "等待玩家";
                        u.player2.SetActive(false);
                        u.开始或准备按钮的文字.color = Color.red;
                        u.开始或准备按钮的文字.text = "等待";
                        MultiGameManager.enemyName = "";
                        u.leaveGameRoom();
                        if (code == 1006) u.ShowError("无法连接");
                        if(code == 1001) u.ShowError("与服务器断开连接");
                    }
                });
            };
            return client;
        }
    }
}
