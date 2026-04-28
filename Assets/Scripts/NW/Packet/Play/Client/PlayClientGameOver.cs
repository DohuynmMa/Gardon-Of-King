using UnityEngine;
using System;
namespace Assets.Scripts.NetWork.Packet.Play.Client
{
    [Serializable]
    [PacketMeta(Side.Client)]
    class PlayClientGameOver : IPacket
    {
        public PlayClientGameOver(string winnerName)
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
                    Musics.Ê¤ÀûÒôÀÖ.play(false);
                }
                else
                {
                    Musics.Ê§°ÜÒôÀÖ.play(false);
                }
            });
        }
    }
}
