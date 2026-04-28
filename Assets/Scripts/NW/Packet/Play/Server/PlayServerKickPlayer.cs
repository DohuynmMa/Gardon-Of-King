using System;
using Assets.Scripts.NetWork.Server;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerKickPlayer : IPacket
    {
        public PlayServerKickPlayer()
        {
        }
        public PlayServerKickPlayer(string reason)
        {
            this.reason = reason;
        }
        public string reason = "你被房主请出";
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                DialogPVP.Instance.leaveGameRoom();
                DialogPVP.Instance.ShowError(reason);
            });
        }
    }
}
