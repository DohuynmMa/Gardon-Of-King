using System;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.Utils;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerSyncEntityAim : IPacket
    {
        public PlayServerSyncEntityAim(int entityId,int aimId)
        {
            this.entityId = entityId;
            this.aimId = aimId;
        }

        /// <summary>
        /// 实体ID
        /// </summary>
        public int entityId;
        /// <summary>
        /// 目标实体ID
        /// </summary>
        public int aimId;
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                Entity entity = Utils.Utils.findEntityByIDMultiGame(entityId);
                if (entity == null) return;
                entity.syncAim(aimId);
            });
        }
    }
}
