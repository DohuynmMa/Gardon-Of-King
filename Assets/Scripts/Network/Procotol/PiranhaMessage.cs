using System;
using System.Threading.Tasks;
using DotNetty.Buffers;

namespace Assets.Scripts.Network.Procotol
{
    public class PiranhaMessage
    {
        public PiranhaMessage(Device device)
        {
            Device = device;
            Writer = Unpooled.Buffer();
        }

        public PiranhaMessage(Device device, IByteBuffer buffer)
        {
            Device = device;
            Reader = buffer;
        }

        public IByteBuffer Writer { get; set; }
        public IByteBuffer Reader { get; set; }
        public Device Device { get; set; }
        public ushort Id { get; set; }
        public int Length { get; set; }
        public ushort Version { get; set; }
        public bool Save { get; set; }

        public virtual void Decode()
        {
        }

        public virtual void Encode()
        {
        }

        public virtual void Process()
        {
        }
        //
        /// <summary>
        ///     Writes this message to the server channel
        /// </summary>
        /// <returns></returns>
        public async Task SendAsync()
        {
            try
            {
                await Device.Handler.Channel.WriteAndFlushAsync(this);

                UnityEngine.Debug.Log($"[C] Message {Id} ({GetType().Name}) sent.");
            }
            catch (Exception)
            {
                UnityEngine.Debug.Log($"Failed to send {Id}.");
            }
        }
    }
}