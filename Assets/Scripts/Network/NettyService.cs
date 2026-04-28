using System.Net;
using System.Threading.Tasks;
using Assets.Scripts.Network.Handlers;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Assets.Scripts.Network
{
    public class NettyService
    {
        public MultithreadEventLoopGroup Group { get; set; }

        public Bootstrap Bootstrap { get; set; }
        public IChannel ClientChannel { get; set; }

        /// <summary>
        ///     Run the server
        /// </summary>
        /// <returns></returns>
        public async Task RunServerAsync()
        {
            Group = new MultithreadEventLoopGroup();
           

            Bootstrap = new Bootstrap();
            Bootstrap.Group(Group);
            Bootstrap.Channel<TcpSocketChannel>();

            Bootstrap
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddFirst("FrameDecoder", new LengthFieldBasedFrameDecoder(4096, 2, 3, 2, 0));
                    pipeline.AddFirst("ReadTimeoutHandler", new ReadTimeoutHandler(30));
                    pipeline.AddLast("PacketHandler", new PacketHandler());
                    pipeline.AddLast("WriteTimeoutHandler", new WriteTimeoutHandler(30));
                    pipeline.AddLast("PacketEncoder", new PacketEncoder());
                }));

            ClientChannel = await Bootstrap.ConnectAsync(NettyMultiGameManager.IPAddress, NettyMultiGameManager.Port);
        }

        /// <summary>
        ///     Close all channels and disconnects clients
        /// </summary>
        /// <returns></returns>
        public async Task Shutdown()
        {
            await ClientChannel.CloseAsync();
        }

        /// <summary>
        ///     Shutdown all workers of netty
        /// </summary>
        /// <returns></returns>
        public async Task ShutdownWorkers()
        {
            await Group.ShutdownGracefullyAsync();
        }
    }
}