using System;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.Utils;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerAddBuffToEntity : IPacket
    {
        public PlayServerAddBuffToEntity(int entityId,float buffTime,BuffType buffType)
        {
            this.entityId = entityId;
            this.buffType = buffType;
            this.buffTime = buffTime;
        }

        /// <summary>
        /// 实体ID
        /// </summary>
        public int entityId;
        /// <summary>
        /// buff time
        /// </summary>
        public float buffTime;
        /// <summary>
        /// 目标实体ID
        /// </summary>
        public BuffType buffType;
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                Entity entity = Utils.Utils.findEntityByIDMultiGame(entityId);
                if (entity == null) return;
                BuffManager.Instance.addBuff(entity, buffTime, buffType,true);
            });
        }
    }
}
