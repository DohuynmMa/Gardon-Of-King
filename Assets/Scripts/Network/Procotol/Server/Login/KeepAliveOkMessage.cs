

namespace Assets.Scripts.Network.Procotol.Server.Login
{
    public class KeepAliveOkMessage : PiranhaMessage
    {
        public KeepAliveOkMessage(Device device) : base(device)
        {
            Id = 20101;
        }
    }
}
