using System;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.Utils;
using Assets.Scripts.NetWork.Packet.Play.Client;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerLetEntityShoot : IPacket
    {
        public PlayServerLetEntityShoot(int entityId)
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
                if(entity.anim != null)
                {
                    entity.anim.SetBool("attacking", true);
                    entity.anim.SetTrigger("attack");
                    entity.hitTimer = entity.hitTime;
                }
            });
        }
    }
}
