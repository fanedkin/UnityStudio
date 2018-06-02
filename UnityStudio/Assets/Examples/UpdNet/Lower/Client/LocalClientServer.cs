namespace VRNetLibrary
{
    /// <summary>
    /// 本地客户端
    /// Vehicle
    /// 2017.4.14
    /// </summary>
    public class LocalClientServer : IClient
    {
        /// <summary>
        /// 模拟是否连接
        /// </summary>
        private bool m_IsConnected;

        private NetMsgIndexGenerator mMsgIndexGenerato;

        ~LocalClientServer()
        {
            this.mClose();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="handle"></param>
        private void mInit(dReceiveMessageHandle handle)
        {
            this.mMsgIndexGenerato = new NetMsgIndexGenerator();
        }

        /// <summary>
        /// 是否已经连接
        /// </summary>
        /// <returns></returns>
        private bool mIsConnected()
        {
            return this.m_IsConnected;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        private void mConnect()
        {
            this.m_IsConnected = true;
            this.mOnConnect();
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        private void mReConnect()
        {
            this.mConnect();
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        private void mClose()
        {
            this.m_IsConnected = false;
        }

        /// <summary>
        /// 同步发送消息
        /// </summary>
        private void mSendMessageSync(MessagePackage package)
        {
            if (this.mIsConnected())
            {
                package.SetSessionID(-1);
                Singleton<ServersManager>.Instance.OnMessage(package);
            }
        }

        /// <summary>
        /// 连接上服务器
        /// </summary>
        private void mOnConnect()
        {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="handle"></param>
        public void Init(dReceiveMessageHandle handle, NetARQType arqType, params object[] args)
        {
            this.mInit(handle);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            this.mClose();
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        public void Connect()
        {
            this.mConnect();
        }

        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return this.mIsConnected();
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        public void ReConnect()
        {
            this.mReConnect();
        }

        public void SendMessage(MessagePackage package, bool isSync = false)
        {
            package.SetMsgID(this.mMsgIndexGenerato.GetIndex);
            this.mSendMessageSync(package);
        }

        public double GetNetDelay()
        {
            return 0.0;
        }

        public void SetCheckNetDelayStatus(bool status)
        {
        }
    }
}
