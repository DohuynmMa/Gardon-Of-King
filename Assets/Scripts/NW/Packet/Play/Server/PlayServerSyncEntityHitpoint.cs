using System;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.Utils;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerSyncEntityHitpoint : IPacket
    {
        public PlayServerSyncEntityHitpoint(int entityId,float hitpoint)
        {
            this.entityId = entityId;
            this.hitpoint = hitpoint;
        }

        /// <summary>
        /// 实体ID
        /// </summary>
        public int entityId;
        /// <summary>
        /// 实体hp
        /// </summary>
        public float hitpoint;
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                Entity entity = Utils.Utils.findEntityByIDMultiGame(entityId);
                if (entity == null) return;
                entity.hitpoint = hitpoint;
                entity.updateHpBar();
                if (entity.hasParent)
                {
                    Tower tower = entity.transform.parent.GetComponent<Tower>();
                    if (tower == null) return;
                    tower.towerHp = hitpoint;
                    tower.updateHpBar();
                }
            });
        }
    }
}
