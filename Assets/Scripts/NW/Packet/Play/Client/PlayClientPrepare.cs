using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.NetWork.Server;
namespace Assets.Scripts.NetWork.Packet.Play.Client
{
    [Serializable]
    [PacketMeta(Side.Client)]
    class PlayClientPrepare : IPacket
    {
        public PlayClientPrepare(bool isPrepared)
        {
            this.isPrepared = isPrepared;
        }
        public bool isPrepared;
        public void onReceive(int userId)
        {
            NetworkServerService.getUserById(userId).isPrepared = isPrepared;
            Utils.Utils.run(() =>
            {
                if (isPrepared)
                {
                    Sounds.bitClick.play();
                    DialogPVP.Instance.开始或准备按钮的文字.color = Color.green;
                    DialogPVP.Instance.开始或准备按钮的文字.text = "开始";
                    DialogPVP.Instance.p2.text = "P2 √";
                }
                else
                {
                    DialogPVP.Instance.开始或准备按钮的文字.color = Color.red;
                    DialogPVP.Instance.开始或准备按钮的文字.text = "等待";
                    DialogPVP.Instance.p2.text = "P2 X";
                }
            });
        }
    }
}
