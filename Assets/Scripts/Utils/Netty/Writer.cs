using System;
using System.Linq;
using System.Text;
using DotNetty.Buffers;

namespace Assets.Scripts.Utils.Netty
{
    /// <summary>
    ///     一些扩展
    /// </summary>
    public static class Writer
    {
        /// <summary>
        ///     发送string
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        public static void WriteGOKString(this IByteBuffer buffer, string value)
        {
            if (value == null)
            {
                buffer.WriteInt(-1);
            }
            else if (value.Length == 0)
            {
                buffer.WriteInt(0);
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(value);

                buffer.WriteInt(bytes.Length);
                buffer.WriteString(value, Encoding.UTF8);
            }
        }
        /// <summary>
        ///     一般情况下不使用
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        public static void WriteHex(this IByteBuffer buffer, string value)
        {
            var tmp = value.Replace("-", string.Empty).Replace("-", string.Empty);
            buffer.WriteBytes(Enumerable.Range(0, tmp.Length).Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(tmp.Substring(x, 2), 16)).ToArray());
        }
    }
}