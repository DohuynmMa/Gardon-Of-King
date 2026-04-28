using System;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.Utils;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerSetTower : IPacket
    {
        public PlayServerSetTower(EntityType towerType,HomeNpcType homeType)
        {
            this.towerType = towerType;
            this.homeType = homeType;
        }
        /// <summary>
        /// 防御塔植物的TYPE
        /// </summary>
        public EntityType towerType;
        /// <summary>
        /// 防御塔植物的TYPE
        /// </summary>
        public HomeNpcType homeType;
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                var gm = GameManager.Instance;
                var entityType = DataManager.Instance.data.towerEntity;
                var homeNpcType = DataManager.Instance.data.homeNpc;

                gm.tower1 = gm.newTower(1, new(-4.61f, 0.84f, 0), EntityGroup.friend, entityType, max => max, Utils.Utils.inNight());
                gm.tower2 = gm.newTower(2, new(-4.61f, -2.8f, 0), EntityGroup.friend, entityType, max => max, Utils.Utils.inNight());
                gm.tower3 = gm.newTower(3, new(4.60f, -2.80f, 0), EntityGroup.enemy, towerType, max => max, Utils.Utils.inNight());
                gm.tower4 = gm.newTower(4, new(4.60f, 0.84f, 0), EntityGroup.enemy, towerType, max => max, Utils.Utils.inNight());

                gm.enemyTowerCount = 2;
                gm.friendTowerCount = 2;

                gm.home1 = gm.newHome(new(-7.9f, -2.27f, 0), EntityGroup.friend, homeNpcType);
                gm.home2 = gm.newHome(new(8.04f, -2.27f, 0), EntityGroup.enemy, homeType);
            });
        }
    }
}
