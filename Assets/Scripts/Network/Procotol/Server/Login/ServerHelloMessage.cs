using DotNetty.Buffers;
using Assets.Scripts.Network.Procotol.Client.Login;

namespace Assets.Scripts.Network.Procotol.Server.Login
{
    public class ServerHelloMessage : PiranhaMessage
    {
        public ServerHelloMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 20100;
        }

        public override async void Process()
        {
            await new KeepAliveMessage(Device).SendAsync();
        }
    }
}