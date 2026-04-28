using System;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.Utils;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerCornPultBulletUpdate : IPacket
    {
        public PlayServerCornPultBulletUpdate(int entityId,int bulletTemp)
        {
            this.entityId = entityId;
            this.bulletTemp = bulletTemp;
        }

        /// <summary>
        /// 实体ID
        /// </summary>
        public int entityId;
        /// <summary>
        /// 判断是黄油的子弹索引,3是黄油
        /// </summary>
        public int bulletTemp;
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                Entity entity = Utils.Utils.findEntityByIDMultiGame(entityId);
                if (entity == null || entity.entityType != EntityType.Cornpult || entity.GetComponent<Cornpult>() == null) return;
                Cornpult cornpult = entity.GetComponent<Cornpult>();
                cornpult.syncBulletByPacket(bulletTemp);
            });
        }
    }
}
