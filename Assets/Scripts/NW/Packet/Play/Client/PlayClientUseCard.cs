using System;
using System.Collections.Generic;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.NetWork.Packet.Play.Server;
namespace Assets.Scripts.NetWork.Packet.Play.Client
{
    [Serializable]
    [PacketMeta(Side.Client)]
    class PlayClientUseCard : IPacket
    {
        public PlayClientUseCard(int cellId, EntityType type,int entityAmount)
        {
            this.cellId = cellId;
            this.type = type;
            this.entityAmount = entityAmount != 0 ? entityAmount : 1;
        }
        /// <summary>
        /// 格子ID
        /// </summary>
        public int cellId;
        /// <summary>
        /// 卡片TYPE
        /// </summary>
        public EntityType type;
        public int entityAmount;
        public void onReceive(int userId)
        {
            // 获取格子和卡片，并判断用户有没有该卡片，格子有没有占用等等
            // 然后再生成实体，返回实体数据回去
            if (!isPlayerHasCard(userId, type)) return;
            Utils.Utils.run(() =>
            {
                var cell = Utils.Utils.getCellById(cellId);
                if (cell == null) return;
                int ID = Utils.Utils.createEntityId();
                for(int i = 0; i < entityAmount; i++)
                {
                    handleAddEntityToCell(userId, cell, type, ID);
                }
                UnityEngine.Debug.Log("ClientUseCard");
                NetworkServerService.getUserById(userId).Send(new PlayServerAddEntity(Utils.Utils.getRelativeCellId(cellId), ID, type,EntityGroup.friend,HandManager.Instance.currentSummonEntityCount));
            });
        }

        private bool isPlayerHasCard(int userId, EntityType cardType)
        {
            return NetworkServerService.getPlayerInfoById(userId).cardList.Contains(cardType);
        }

        private void handleAddEntityToCell(int userId, Cell cell, EntityType cardType,int entityID)
        {
            if (cell.currentEntity != null && cardType.isPlant())
            {
                UnityEngine.Debug.Log("有实体");
                return;
            }
            var group = EntityGroup.enemy; // TODO: 通过用户ID获取阵营
            cell.addEntityByPacket(cardType, group, entityID);
        }
    }
}
