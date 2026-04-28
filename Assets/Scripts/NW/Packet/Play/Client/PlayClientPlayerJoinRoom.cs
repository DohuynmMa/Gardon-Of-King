using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.NetWork.Packet.Play.Server;
using Assets.Scripts.NetWork.Server;
namespace Assets.Scripts.NetWork.Packet.Play.Client
{
    [Serializable]
    [PacketMeta(Side.Client)]
    class PlayClientPlayerJoinRoom : IPacket
    {
        public PlayClientPlayerJoinRoom(string name, EntityType towerEntity, HomeNpcType homeNpc, List<Card> cardList, bool hasShovel,int version)
        {
            joinPlayerInfo = new PlayerInfo(name, towerEntity, homeNpc, cardList, hasShovel,version);
        }
        /// <summary>
        /// 玩家名字
        /// </summary>
        public PlayerInfo joinPlayerInfo;
        public void onReceive(int userId)
        {
            Debug.Log(joinPlayerInfo.name + "加入了房间");
            Utils.Utils.run(() =>
            {
                MultiGameManager.enemyName = joinPlayerInfo.name;
                DialogPVP.Instance.playerName2.text = joinPlayerInfo.name;
                DialogPVP.Instance.roomStateTXT.text = "房间已满";
                DialogPVP.Instance.player2.SetActive(true);
                DialogPVP.Instance.p2.text = "P2 ×";
                DialogPVP.Instance.kickButton1.SetActive(true);

                var serverVs = MultiGameManager.version;
                if(joinPlayerInfo.version != serverVs)
                {
                    NetworkServerService.getUserById(userId).Send(new PlayServerKickPlayer("版本不同步"));
                    return;
                }

                NetworkServerService.getUserById(userId).setPlayerInfo(joinPlayerInfo);
                var dat = DataManager.Instance.data;
                NetworkServerService.getUserById(userId).Send(new PlayServerOnPlayerJoin(dat.playerName, dat.towerEntity,dat.homeNpc,dat.cardList,dat.hasShovel,MultiGameManager.version));
            });
        }
    }
}
