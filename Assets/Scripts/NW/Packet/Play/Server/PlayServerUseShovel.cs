using System;
using Assets.Scripts.NetWork.Server;
using Assets.Scripts.Utils;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerUseShovel : IPacket
    {
        public PlayServerUseShovel(int entityId)
        {
            this.entityId = entityId;
        }

        /// <summary>
        /// 实体ID
        /// </summary>
        public int entityId;
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                Sounds.使用铲子.play();
                Sounds.种植物.play();
                Entity entity = Utils.Utils.findEntityByIDMultiGame(entityId);
                if (entity == null)
                {
                    UnityEngine.Debug.Log("null entity server → client");
                    return;
                }
                if (entity.anim != null)
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
