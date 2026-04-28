using System;
using System.Net;
using Assets.Scripts.Network.Procotol.Client.Login;
using Assets.Scripts.Network.Procotol.Server.Login;
using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;

namespace Assets.Scripts.Network.Handlers
{
    public class PacketHandler : ChannelHandlerAdapter
    {
        public PacketHandler()
        {
            Throttler = new Throttler(10, 500);
            Device = new Device(this);
        }

        public Device Device { get; set; }
        public IChannel Channel { get; set; }
        public Throttler Throttler { get; set; }
        public override async void ChannelActive(IChannelHandlerContext context)
        {
            await new LoginMessage(Device).SendAsync();
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer) message;
            if (buffer == null) return;

            if (Throttler.CanProcess())
            {
                Device.Process(buffer);
            }
            else
            {
                Device.Disconnect();
            }
        }
        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();
        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            Channel = context.Channel;

            var remoteAddress = (IPEndPoint) Channel.RemoteAddress;

            base.ChannelRegistered(context);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            if (exception.GetType() != typeof(ReadTimeoutException) &&
                exception.GetType() != typeof(WriteTimeoutException))
            context.CloseAsync();
        }
    }
}