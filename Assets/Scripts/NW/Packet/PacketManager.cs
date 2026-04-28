using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.NetWork.Packet
{
    class PacketManager
    {
        private static readonly Dictionary<string, PacketInfo> serverSidePackets = new Dictionary<string, PacketInfo>();
        private static readonly Dictionary<string, PacketInfo> clientSidePackets = new Dictionary<string, PacketInfo>();
        /// <summary>
        /// 初始化包管理器，扫描所有带有 PacketMeta 注解的类，添加到字典中待用
        /// </summary>
        public static void init()
        {
            if (serverSidePackets.Count > 0 || clientSidePackets.Count > 0) return;
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            Assembly assembly = Assembly.Load(assemblyName);
            Type[] types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (!type.IsClass) continue;
                PacketMetaAttribute attribute = type.GetCustomAttribute<PacketMetaAttribute>();
                if (attribute == null) continue;
                var id = attribute.packetId == string.Empty ? type.Name : attribute.packetId;
                var info = new PacketInfo(id, attribute.packetSide, type);
                Dictionary<string, PacketInfo> packets = attribute.packetSide == Side.Server ? serverSidePackets : clientSidePackets;
                packets.Add(info.id, info);
            }
            UnityEngine.Debug.Log("加载了 " + serverSidePackets.Count + " 个服务端包，" + clientSidePackets.Count + " 个客户端包");
        }

        public static string Serialize(IPacket packet)
        {
            if (packet == null) return null;
            var type = packet.GetType();
            var attribute = type.GetCustomAttribute<PacketMetaAttribute>();
            if (attribute == null) return type.Name;
            var id = attribute.packetId == string.Empty ? type.Name : attribute.packetId;
            return id + "|" + JsonUtility.ToJson(packet);
        }

        /// <summary>
        /// 解析数据包并处理
        /// </summary>
        /// <param name="side">来自哪里，客户端还是服务端</param>
        /// <param name="message">收到的消息</param>
        public static void resolveAndHandlePacket(Side side, int userId, string message)
        {
            var split = message.Split('|', 2);
            if (split.Length != 2) return;
            var id = split[0];
            var json = split[1];
            Dictionary<string, PacketInfo> packets = side == Side.Server ? serverSidePackets : clientSidePackets;
            if (!packets.ContainsKey(split[0]))
            {
                var sideName = side == Side.Server ? "服务端" : "客户端";
                UnityEngine.Debug.LogWarning("收到了无效的" + sideName + "包 (id=" + id + ", data=" + json + ")");
                return;
            }
            PacketInfo info = packets[split[0]];
            object packet = JsonUtility.FromJson(json, info.type);
            if (packet is IPacket)
            {
                ((IPacket)packet).onReceive(userId);
            }
        }

    }

    public interface IPacket
    {
        /// <summary>
        /// 接收包时执行的操作
        /// </summary>
        /// <param name="userId">包来源用户ID，来自服务端恒为0，来自客户端大于0</param>
        void onReceive(int userId);
    }

    /// <summary>
    /// 数据包的来源
    /// </summary>
    public enum Side
    {
        /// <summary>
        /// 这个数据包来自服务端，发给客户端
        /// </summary>
        Server,
        /// <summary>
        /// 这个数据包来自客户端，发给服务端
        /// </summary>
        Client
    }

    public class PacketInfo
    {
        public string id { get; private set; }
        public Side side { get; private set; }
        public Type type { get; private set; }
        public PacketInfo(string id, Side side, Type type)
        {
            this.id = id;
            this.side = side;
            this.type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class PacketMetaAttribute : Attribute
    {
        public Side packetSide { get; private set; }
        public string packetId { get; private set; }
        public PacketMetaAttribute(Side side, string id = "")
        {
            packetSide = side;
            packetId = id;
        }
    }
}
