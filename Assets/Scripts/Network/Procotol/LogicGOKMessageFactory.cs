using Assets.Scripts.Network.Procotol.Server.Login;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Network.Procotol
{
    public class LogicGOKMessageFactory
    {
        public static Dictionary<int, Type> Messages;

        static LogicGOKMessageFactory()
        {
            Messages = new Dictionary<int, Type>
            {
                {20100, typeof(ServerHelloMessage) },
                {20101, typeof(KeepAliveOkMessage) },//
            };
        }
    }
}