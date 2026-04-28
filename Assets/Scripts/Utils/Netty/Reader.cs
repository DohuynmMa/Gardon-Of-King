using System.Text;
using DotNetty.Buffers;

namespace Assets.Scripts.Utils.Netty
{
    /// <summary>
    ///     一些扩展
    /// </summary>
    public static class Reader
    {
        /// <summary>
        ///     读取string
        /// </summary>
        /// <param name="byteBuffer"></param>
        /// <returns></returns>
        public static string ReadGOKString(this IByteBuffer byteBuffer)
        {
            var length = byteBuffer.ReadInt();

            if (length <= 0 || length > 900000)
                return string.Empty;

            return byteBuffer.ReadString(length, Encoding.UTF8);
        }
    }
}