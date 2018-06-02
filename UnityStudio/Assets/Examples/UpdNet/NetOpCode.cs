using System;

namespace VRNetLibrary
{
    /// <summary>
    /// 网络操作代码
    /// </summary>
    [Flags]
    public enum NetOpCode : byte
    {
        /// <summary>
        /// no process
        /// </summary>
        EMPTY = 0,
        /// <summary>
        /// 字节数据传输
        /// </summary>
        BINARY = 1,
        /// <summary>
        /// Ping操作
        /// </summary>
        PING = 2,
        /// <summary>
        /// Pong操作
        /// </summary>
        PONG = 4,
        /// <summary>
        /// 心跳操作
        /// </summary>
        HEART = 8
    }
}
