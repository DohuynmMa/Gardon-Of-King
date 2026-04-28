using Assets.Scripts.NetWork.Packet;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Assets.Scripts.NetWork.Server
{
    /// <summary>
    /// WebSocket 服务端行为
    /// </summary>
    class NetworkServerService : WebSocketBehavior
    {
        public static Dictionary<int, NetworkServerService> users = new Dictionary<int, NetworkServerService>();
        private PlayerInfo playerInfo;
        public bool isPrepared = false;
        public static NetworkServerService getUserById(int userId)
        {
            if (users.ContainsKey(userId)) return users[userId];
            return null;
        }
        public static PlayerInfo getPlayerInfoById(int userId)
        {
            if (users.ContainsKey(userId)) return users[userId].playerInfo;
            return null;
        }
        public void setPlayerInfo(PlayerInfo playerInfo)
        {
            this.playerInfo = playerInfo;
        }
        public void Send(IPacket packet)
        {
            string message = PacketManager.Serialize(packet);
            Send(message);
        }
        public void close()
        {
            Close();
        }

        private static int _number = 0;
        private static int getIncrementNumber()
        {
            return Interlocked.Increment(ref _number);
        }
        private static int getDecrementNumber()
        {
            return Interlocked.Decrement(ref _number);
        }
        public int userId { get; private set; }

        /// <summary>
        /// 有客户端连接进来时执行
        /// </summary>
        protected override void OnOpen()
        {
            if (roomFull())
            {
                UnityEngine.Debug.Log("由于人数达到上限,用户" + (_number + 1) + "被禁止连接");
                return;
            }
            userId = getIncrementNumber(); // 唯一的自增ID
            users.Add(userId, this);
            UnityEngine.Debug.Log("[服务端] 用户 " + userId + " 已连接");
        }

        /// <summary>
        /// 收到客户端消息时执行
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsText)
            {
                var message = e.Data; // 收到的消息
                PacketManager.resolveAndHandlePacket(Side.Client, userId, message);
            }
        }

        /// <summary>
        /// 收到错误时执行
        /// </summary>
        /// <param name="e"></param>
        protected override void OnError(ErrorEventArgs e)
        {
            UnityEngine.Debug.LogException(e.Exception);
        }

        /// <summary>
        /// 客户端关闭连接时执行
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClose(CloseEventArgs e)
        {
            var code = e.Code;
            var reason = e.Reason;

            UnityEngine.Debug.Log("[服务端] 用户 " + userId + " 断开连接,原因:" + reason + "(代码:" + code + ")");

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
                else if (Utils.Utils.scene == "GameMenu")
                {
                    u.playerName2.text = "";
                    u.roomStateTXT.text = "等待玩家";
                    u.player2.SetActive(false);
                    u.开始或准备按钮的文字.color = Color.red;
                    u.开始或准备按钮的文字.text = "等待";
                    MultiGameManager.enemyName = "";
                }
            });

            getDecrementNumber();
            users.Remove(userId);
        }
        protected bool roomFull()
        {
            bool full = _number >= 1;
            return full;
        }
    }
    [System.Serializable]
    public class PlayerInfo
    {
        public string name;
        public EntityType towerEntity = EntityType.PeaShooter;
        public HomeNpcType homeNpc = HomeNpcType.Dave;
        public List<EntityType> cardList;
        public bool hasShovel = false;
        public int version;
        public PlayerInfo(string name, EntityType towerEntity, HomeNpcType homeNpc, List<Card> cardList, bool hasShovel, int version)
        {
            this.name = name;
            this.towerEntity = towerEntity;
            this.homeNpc = homeNpc;
            this.cardList = turnCardListIntoCardTypeList(cardList);
            this.hasShovel = hasShovel;
            this.version = version;
        }
        public List<EntityType> turnCardListIntoCardTypeList(List<Card> cardList)
        {
            List<EntityType> typeList = new List<EntityType>();
            foreach (Card card in cardList)
            {
                if (card != null)
                {
                    typeList.Add(card.entityType);
                }
                else
                {
                    typeList.Add(EntityType.PeaShooter);
                }
            }
            return typeList;
        }
    }
    [System.Serializable]
    public class MultiGameSetting
    {
        public float sunSpeed;
        public float maxSun;
        public float startSun;
        public BackgroundType backgroundType;
        public MultiGameSetting(float sunSpeed = 1,float maxSun = 10,float startSun = 7,BackgroundType backgroundType = BackgroundType.Day)
        {
            this.sunSpeed = sunSpeed > 0 ? sunSpeed : 1;
            this.maxSun = maxSun;
            this.startSun = startSun > maxSun ? maxSun : startSun;
            this.backgroundType = backgroundType;
        }
    }
}
