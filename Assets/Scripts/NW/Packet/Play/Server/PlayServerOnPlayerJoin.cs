using Assets.Scripts.NetWork.Server;
using System;
using System.Collections.Generic;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerOnPlayerJoin : IPacket
    {
        public PlayerInfo playerInfo;
        public PlayServerOnPlayerJoin(string name, EntityType towerEntity, HomeNpcType homeNpc, List<Card> cardList, bool hasShovel, int version)
        {
            playerInfo = new PlayerInfo(name, towerEntity, homeNpc, cardList, hasShovel, version);
        }
        
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                var u = DialogPVP.Instance;
                MultiGameManager.enemyName = playerInfo.name;
                u.playerName1.text = playerInfo.name;

                var clientVs = MultiGameManager.version;
                if (playerInfo.version != clientVs)
                {
                    u.leaveGameRoom();
                    u.ShowError("版本不同步");
                    return;
                }

                u.roomStateTXT.text = "房间已满";
                MultiGameManager.myUserId = userId;
            });
        }
    }
}
