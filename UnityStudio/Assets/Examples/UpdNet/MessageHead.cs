namespace VRNetLibrary
{
    /// <summary>
    /// 消息头结构
    /// Vehicle
    /// 2017.4.24
    /// </summary>
    public struct MessageHead
    {
        /// <summary>
        /// 本地SessionID
        /// </summary>
        public const int LocalClientMessageSessionID = -1;

        /// <summary>
        /// 消息头结构大小
        /// </summary>
        public const int MessageHeadSize = 17;

        /// <summary>
        /// 操作类型
        /// </summary>
        public NetOpCode OpCode;

        /// <summary>
        /// 消息索引
        /// </summary>
        public uint MsgID;

        /// <summary>
        /// 协议id
        /// </summary>
        public int ProtocolID;

        /// <summary>
        /// 会话id，-1为本地客户端的Session
        /// </summary>
        public int SessionID;

        /// <summary>
        /// 消息体总字节
        /// </summary>
        public int ContentLength;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="bytes"></param>
        public void Init()
        {
            this.OpCode = NetOpCode.EMPTY;
            this.MsgID = 0u;
            this.ProtocolID = 0;
            this.SessionID = -1;
            this.ContentLength = 0;
        }

        /// <summary>
        /// 获取一个空消息头
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static MessageHead GetEmptyHead()
        {
            return default(MessageHead);
        }
    }
}
