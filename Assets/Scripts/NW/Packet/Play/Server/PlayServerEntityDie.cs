using System;
using Assets.Scripts.NetWork.Packet.Play.Client;
using UnityEngine;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerEntityDie : IPacket
    {
        public PlayServerEntityDie(int entityId)
        {
            this.entityId = entityId;
        }

        /// <summary>
        ///  µÃÂID
        /// </summary>
        public int entityId;
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                Entity entity = Utils.Utils.findEntityByIDMultiGame(entityId);
                if (entity == null) return;
                if (entity.hasParent)
                {
                    Tower tower = entity.transform.parent.GetComponent<Tower>();
                    if (tower == null) return;
                    tower.towerBreak();
                }
                if(entity.anim != null)
                {
                    entity.anim.SetBool("Die", true);
                }
                else
                {
                    entity.entityDie();
                }
            });
        }
    }
}
