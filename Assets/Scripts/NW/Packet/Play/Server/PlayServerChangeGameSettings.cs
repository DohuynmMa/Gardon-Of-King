using Assets.Scripts.NetWork.Server;
using System;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerChangeGameSettings : IPacket
    {
        public MultiGameSetting settings;
        public int bgTemp;
        public PlayServerChangeGameSettings(MultiGameSetting settings,int bgTemp = 0)
        {
            this.settings = settings;
            this.bgTemp = bgTemp;
        }
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                var um = DialogPVP.Instance;
                um.阳光速度.text = settings.sunSpeed.ToString();
                um.最初阳光.text = settings.startSun.ToString();
                um.最高阳光.text = settings.maxSun.ToString();
                um.settingMapTemp = bgTemp;
                um.settingChangeMapTemp();
                um.onHosterChangeSettings();
            });
        }
    }
}
