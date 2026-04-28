using Assets.Scripts.NetWork.Server;
using System;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerGameStart : IPacket
    {
        public MultiGameSetting settings;

        public PlayServerGameStart(MultiGameSetting settings)
        {
            this.settings = settings;
        }
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                var bm = BridgeManager.Instance;
                bm.startSunPoint = settings.startSun;
                bm.maxSunPoint = settings.maxSun;
                bm.sunPointAddSpeed = settings.sunSpeed;
                bm.backgroundType = settings.backgroundType;
                DialogMainMenu.Instance.changeScene(GameMode.MultiPlayer, 100, 0, 10);
            });
        }
    }
}
