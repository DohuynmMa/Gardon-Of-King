using System;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.Utils;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerSyncHomeNpcAim : IPacket
    {
        public PlayServerSyncHomeNpcAim(int parentHomeId,int entityId)
        {
            this.entityId = entityId;
            this.parentHomeId = parentHomeId;
        }

        /// <summary>
        /// 实体ID
        /// </summary>
        public int entityId;
        /// <summary>
        /// HomeNpc的Home的EntityId
        /// </summary>
        public int parentHomeId;
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                Entity entity = Utils.Utils.findEntityByIDMultiGame(parentHomeId);
                if (entity == null || entity.tag != "Home") return;
                Home home = entity.GetComponent<Home>();
                if (home.npc == null) return;
                home.npc.syncAim(entityId);
            });
        }
    }
}
