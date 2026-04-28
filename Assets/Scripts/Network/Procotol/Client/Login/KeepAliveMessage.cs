using DotNetty.Buffers;
using Assets.Scripts.Network.Procotol.Server.Login;
namespace Assets.Scripts.Network.Procotol.Client.Login
{
    public class KeepAliveMessage : PiranhaMessage
    {
        public KeepAliveMessage(Device device) : base(device)
        {
            Id = 10102;
        }

        public override async void Process()
        {
            await new KeepAliveOkMessage(Device).SendAsync();
        }
    }
}
