using System;
namespace Assets.Scripts.NetWork.Packet.Play.Server
{
    [Serializable]
    [PacketMeta(Side.Server)]
    class PlayServerGameOver : IPacket
    {
        public PlayServerGameOver(string winnerName)
        {
            this.winnerName = winnerName;
        }
        public string winnerName;
        public void onReceive(int userId)
        {
            Utils.Utils.run(() =>
            {
                GameManager.Instance.gameOver(winnerName == DataManager.Instance.data.playerName ? EntityGroup.friend : EntityGroup.enemy,true);
                if (winnerName == DataManager.Instance.data.playerName)
                {
                    Musics.胜利音乐.play(false);
                }
                else
                {
                    Musics.失败音乐.play(false);
                }
            });
        }
    }
}
