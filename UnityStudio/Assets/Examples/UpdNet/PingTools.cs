using System.Diagnostics;

namespace VRNetLibrary
{
    /// <summary>
    /// ping工具
    /// </summary>
    public class PingTools
    {
        private MessagePackage _pingPackage;

        private MessagePackage _pongPackage;

        /// <summary>
        /// ping计时器
        /// </summary>
        private Stopwatch stopwatch;

        private double _pingTime;

        /// <summary>
        ///
        /// </summary>
        public MessagePackage pingPackage
        {
            get
            {
                return this._pingPackage;
            }
            private set
            {
                this._pingPackage = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public MessagePackage pongPackage
        {
            get
            {
                return this._pongPackage;
            }
            private set
            {
                this._pongPackage = value;
            }
        }

        /// <summary>
        /// ping时间。-1为超时
        /// </summary>
        public double PingTime
        {
            get
            {
                return this._pingTime;
            }
            private set
            {
                this._pingTime = value;
            }
        }

        /// <summary>
        /// 是否开始ping
        /// </summary>
        public bool isDoPing
        {
            get
            {
                return this.stopwatch.IsRunning;
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        public PingTools()
        {
            MessagePackage messagePackage = default(MessagePackage);
            messagePackage.SetOpCode(NetOpCode.PING);
            this.pingPackage = messagePackage;
            messagePackage = default(MessagePackage);
            messagePackage.SetOpCode(NetOpCode.PONG);
            this.pongPackage = messagePackage;
            this.stopwatch = new Stopwatch();
        }

        /// <summary>
        /// 开始ping
        /// </summary>
        public void StartPing()
        {
            if (!this.isDoPing)
            {
                this.stopwatch.Reset();
                this.stopwatch.Start();
            }
        }

        /// <summary>
        /// 结束ping
        /// </summary>
        public void EndPing()
        {
            if (this.isDoPing)
            {
                this.stopwatch.Stop();
                this.PingTime = this.stopwatch.Elapsed.TotalMilliseconds;
            }
        }

        /// <summary>
        /// 更新判断超时
        /// </summary>
        public void Update()
        {
            if (this.isDoPing && this.stopwatch.Elapsed.TotalMilliseconds >= 1000.0)
            {
                this.EndPing();
                this.PingTime = -1.0;
            }
        }
    }
}
