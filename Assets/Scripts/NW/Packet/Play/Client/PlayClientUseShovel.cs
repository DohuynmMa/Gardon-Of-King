using System;
using System.Collections.Generic;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.NetWork.Packet.Play.Server;
namespace Assets.Scripts.NetWork.Packet.Play.Client
{
    [Serializable]
    [PacketMeta(Side.Client)]
    class PlayClientUseShovel : IPacket
    {
        public PlayClientUseShovel(int entityId)
        {
            this.entityId = entityId;
        }
        /// <summary>
        /// 实体ID
        /// </summary>
        public int entityId;
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                if (!NetworkServerService.getPlayerInfoById(userId).hasShovel) return;
                Entity entity = Utils.Utils.findEntityByIDMultiGame(entityId);
                if (entity == null)
                {
                    UnityEngine.Debug.Log("null entity client → server");
                    return;
                }

                if (entity.anim != null)
                {
                    entity.anim.SetBool("Die", true);
                }
                else
                {
                    entity.entityDie();
                }
                Sounds.使用铲子.play();
                Sounds.种植物.play();
                NetworkServerService.getUserById(userId).Send(new PlayServerUseShovel(entityId));
            });
        }
    }
}
