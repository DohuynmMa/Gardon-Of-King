using System;
using System.Diagnostics;
using System.Net;
using Assets.Scripts.Network.Procotol;
using Assets.Scripts.Network.Handlers;
using DotNetty.Buffers;

namespace Assets.Scripts
{
    public class Device
    {
        public Device(PacketHandler handler)
        {
            Handler = handler;
            CurrentState = State.Disconnected;
        }

        public bool IsConnected => Handler.Channel.Registered;

        /// <summary>
        ///     Process a message
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public void Process(IByteBuffer buffer)
        {
            var id = buffer.ReadUnsignedShort();
            var length = buffer.ReadMedium();
            var version = buffer.ReadUnsignedShort();

            if (id <= 20000) return;

            if (!LogicGOKMessageFactory.Messages.ContainsKey(id))
            {
                return;
            }

            if (!(Activator.CreateInstance(LogicGOKMessageFactory.Messages[id], this, buffer) is PiranhaMessage
                message)) return;

            try
            {
                message.Id = id;
                message.Length = length;
                message.Version = version;
                message.Decode();
                message.Process();
                UnityEngine.Debug.Log($"Message {id}:{length} ({message.GetType().Name})" + "已接受");
            }
            catch
            {
              
            }
        }

        /// <summary>
        ///     得到客户端的IP
        /// </summary>
        /// <returns></returns>
        public string GetIp()
        {
            return ((IPEndPoint) Handler.Channel.RemoteAddress).Address.MapToIPv4().ToString();
        }

        /// <summary>
        ///     断开连接
        /// </summary>
        /// <returns></returns>
        public async void Disconnect()
        {
            await Handler.Channel.CloseAsync();
        }

        #region Objects
        public PacketHandler Handler { get; set; }
        public DateTime LastVisitHome { get; set; }
        public DateTime LastSectorCommand { get; set; }

        public State CurrentState { get; set; }

        public enum State
        {
            Disconnected = 0,
            Login = 1,
            Battle = 2,
            Home = 3,
            Visit = 4,
            NotDefinied = 5
        }

        #endregion Objects
    }
}