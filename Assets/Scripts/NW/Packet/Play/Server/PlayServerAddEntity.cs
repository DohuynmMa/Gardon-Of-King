using System;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerAddEntity : IPacket
    {
        public PlayServerAddEntity(int cellId,int entityId,EntityType type,EntityGroup group,int entityAmount)
        {
            this.cellId = cellId;
            this.entityId = entityId;
            this.type = type;
            this.group = group;
            this.entityAmount = entityAmount != 0 ? entityAmount : 1;
        }
        /// <summary>
        /// 实体ID，用于唯一识别实体
        /// </summary>
        public int entityId;
        /// <summary>
        /// 格子ID
        /// </summary>
        public int cellId;
        /// <summary>
        /// 卡片TYPE
        /// </summary>
        public EntityType type;
        /// <summary>
        /// 因为放卡可能是服务端玩家主动的,这里需要判定group
        /// </summary>
        public EntityGroup group;
        public int entityAmount;
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                var cell = Utils.Utils.getCellById(cellId);
                if (cell == null) return;
                for (int i = 0; i < entityAmount; i++) {
                    handleAddEntityToCell(userId, cell, type, entityId);
                }
            });
        }
        private void handleAddEntityToCell(int userId, Cell cell, EntityType cardType, int entityID)
        {
            if (cell.currentEntity != null && cardType.isPlant())
            {
                UnityEngine.Debug.Log("有实体");
                return;
            }
            cell.addEntityByPacket(cardType, group, entityID);
        }
    }
}
