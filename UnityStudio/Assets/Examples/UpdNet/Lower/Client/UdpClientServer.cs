using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace VRNetLibrary
{
    /// <summary>
    /// UDP服务
    /// Vehicle
    /// 2017.4.6
    /// </summary>
    public class UdpClientServer : IClient
    {
        private IPEndPoint m_LocalIPEndPonit;

        private IPEndPoint m_RemoteIPEndPonit;

        private dReceiveMessageHandle m_MessageHandle;

        private NetMsgIndexGenerator mMsgIndexGenerato;

        private ISocket mSocket;

        private NetARQType mARQType = NetARQType.None;

        /// <summary>
        /// ping工具
        /// </summary>
        private PingTools pingTools = new PingTools();

        /// <summary>
        /// 是否ping
        /// </summary>
        private bool pingStatus;

        ~UdpClientServer()
        {
            this.mClose();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="handle"></param>
        private void mInit(dReceiveMessageHandle handle, string clientIP, int clientPort, string remoteIP, int remotePort)
        {
            if (string.IsNullOrEmpty(clientIP))
            {
                Debug.LogError(base.GetType() + " IP不能为空");
            }
            if (string.IsNullOrEmpty(remoteIP))
            {
                Debug.LogError(base.GetType() + " IP不能为空");
            }
            if (clientPort == 0)
            {
                Debug.LogError(base.GetType() + " ClientPort不能为空");
            }
            if (remotePort == 0)
            {
                Debug.LogError(base.GetType() + " RemotePort不能为空");
            }
            if (handle == null)
            {
                Debug.LogError(base.GetType() + " 消息接收句柄不能为空");
            }
            try
            {
                this.mMsgIndexGenerato = new NetMsgIndexGenerator();
                this.m_LocalIPEndPonit = new IPEndPoint(IPAddress.Parse(clientIP), clientPort);
                this.m_RemoteIPEndPonit = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort);
                this.m_MessageHandle = handle;
            }
            catch (Exception arg)
            {
                Debug.LogError(base.GetType() + " Init error!!!" + arg);
            }
        }

        /// <summary>
        /// 是否已经连接
        /// </summary>
        /// <returns></returns>
        private bool mIsConnected()
        {
            if (this.mSocket == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        private void mConnect()
        {
            try
            {
                this.mClose();
                if (this.mARQType == NetARQType.None)
                {
                    this.mSocket = new UdpSocket();
                }
                else
                {
                    Debug.LogError("UdpServers Connect!!! NetARQType cant fount,NetARQType=" + this.mARQType);
                }
                this.mSocket.Init(delegate (IPEndPoint ipInfo, byte[] receiveBytes)
                {
                    if (ipInfo.Equals(this.m_RemoteIPEndPonit) && receiveBytes != null && receiveBytes.Length != 0)
                    {
                        this.mOnReceive(receiveBytes, receiveBytes.Length);
                    }
                });
                this.mSocket.Connect(this.m_LocalIPEndPonit, this.m_RemoteIPEndPonit);
            }
            catch (Exception arg)
            {
                this.mClose();
                Debug.LogError("Connect Error!!!" + arg);
            }
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
            if (this.mSocket != null)
            {
                this.mSocket.Close();
                this.mSocket = null;
            }
        }

        /// <summary>
        /// 异步发送消息
        /// </summary>
        private void mSendMessage(MessagePackage package, bool isSync = false)
        {
            if (this.mSocket == null)
            {
                Debug.LogError(this + " m_Client == null!!!");
            }
            package.SetMsgID(this.mMsgIndexGenerato.GetIndex);
            MessageBytes messageBytes = package.ToBytes();
            if (messageBytes.GetLength() == 0)
            {
                Debug.LogError(this + " ContentLength=0,ProtocolID=" + package.Head.ProtocolID + " MsgID=" + package.Head.MsgID + " SessionID=" + package.Head.SessionID);
            }
            this.mSocket.Send(this.m_RemoteIPEndPonit, messageBytes.GetBytes(), messageBytes.GetLength(), isSync);
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        private void mOnReceive(byte[] bytes, int length)
        {
            byte[] array = bytes;
            if (bytes.Length != length)
            {
                array = new byte[length];
                Buffer.BlockCopy(bytes, 0, array, 0, length);
            }
            MessagePackage package = MessagePackage.ToPackage(array);
            if (package.Head.OpCode == NetOpCode.BINARY)
            {
                this.mOnReceivedMessage(package);
            }
            else if (package.Head.OpCode != NetOpCode.PING)
            {
                if (package.Head.OpCode == NetOpCode.PONG)
                {
                    this.StopPingProcessor();
                }
                else if (package.Head.OpCode != NetOpCode.HEART)
                {
                    Debug.LogError("为什么会有空包出现？ OpCode=NetOpCode.EMPTY package.ProtocolID=" + package.Head.ProtocolID);
                }
            }
        }

        /// <summary>
        /// 回调消息
        /// </summary>
        /// <param name="ms"></param>
        private void mOnReceivedMessage(MessagePackage package)
        {
            if (this.m_MessageHandle != null)
            {
                this.m_MessageHandle(package);
            }
            else
            {
                Debug.LogError(this.ToString() + " MessageHandle Is Empty!!!");
            }
        }

        /// <summary>
        /// 向链接写入数据流
        /// </summary>
        private void mOnWrite(IAsyncResult r)
        {
            try
            {
                (r.AsyncState as UdpClient).EndSend(r);
            }
            catch (Exception ex)
            {
                Debug.LogError("OnWrite--->>>" + ex.Message);
            }
        }

        /// <summary>
        /// ping更新
        /// </summary>
        private void PingProcessor()
        {
            this.pingTools.Update();
        }

        /// <summary>
        /// 开始ping
        /// </summary>
        private void StartPingProcessor()
        {
            if (!this.pingTools.isDoPing)
            {
                this.pingTools.StartPing();
                this.SendMessage(this.pingTools.pingPackage, false);
            }
        }

        /// <summary>
        /// 结束ping
        /// </summary>
        private void StopPingProcessor()
        {
            if (this.pingTools.isDoPing)
            {
                this.pingTools.EndPing();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="handle"></param>
        public void Init(dReceiveMessageHandle handle, NetARQType arqType, params object[] args)
        {
            if (args == null || args.Length < 4)
            {
                Debug.LogError("UdpServer需要clientPort,remoteIP,remoteIPPort");
            }
            this.mInit(handle, (string)args[0], (int)args[1], (string)args[2], (int)args[3]);
            this.mARQType = arqType;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (this.pingStatus)
            {
                this.PingProcessor();
                this.StartPingProcessor();
            }
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

        /// <summary>
        /// 发送到指定地址
        /// </summary>
        /// <param name="package"></param>
        /// <param name="isSync"></param>
        public void SendMessage(MessagePackage package, bool isSync = false)
        {
            this.mSendMessage(package, isSync);
        }

        /// <summary>
        /// 获取延迟
        /// </summary>
        /// <returns></returns>
        public double GetNetDelay()
        {
            return this.pingTools.PingTime;
        }

        /// <summary>
        /// 是否检测延迟
        /// </summary>
        /// <param name="_status"></param>
        public void SetCheckNetDelayStatus(bool _status)
        {
            this.pingStatus = _status;
        }
    }
}
