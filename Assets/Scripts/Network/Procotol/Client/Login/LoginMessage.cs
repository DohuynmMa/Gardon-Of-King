using DotNetty.Buffers;

namespace Assets.Scripts.Network.Procotol.Client.Login
{
    public class LoginMessage : PiranhaMessage
    {
        public LoginMessage(Device device) : base(device)
        {
            Id = 10101;
            device.CurrentState = Device.State.Login;
            Version = 10;
        }

        public long UserId { get; set; }
        public int ClientMajorVersion { get; set; }
        public int ClientBuildVersion { get; set; }
        public int ClientMinorVersion { get; set; }
        
        public override void Encode()
        {
            Writer.WriteLong(UserId);
            Writer.WriteInt(ClientMajorVersion);
            Writer.WriteInt(ClientBuildVersion);
            Writer.WriteInt(ClientMinorVersion);
        }
    }
}
