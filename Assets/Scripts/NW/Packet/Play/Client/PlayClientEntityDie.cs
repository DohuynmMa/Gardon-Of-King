using System;
namespace Assets.Scripts.NetWork.Packet.Play.Client
{
    [Serializable]
    [PacketMeta(Side.Client)]
    class PlayClientEntityDie : IPacket
    {
        public PlayClientEntityDie(int entityId)
        {
            this.entityId = entityId;
        }
        /// <summary>
        /// Entity ID
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
