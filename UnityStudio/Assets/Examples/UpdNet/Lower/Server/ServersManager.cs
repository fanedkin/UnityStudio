using System;
using System.Net;
using UnityEngine;
using System.Collections.Generic;
namespace VRNetLibrary
{
    /// <summary>
    /// 主机管理器
    /// </summary>
    public class ServersManager : Singleton<ServersManager>
    {
        /// <summary>
        /// 是否初始化
        /// </summary>
        private bool isInit;

        /// <summary>
        /// 当前网络通讯类型
        /// </summary>
        private NetType mNetType = NetType.Tcp;

        /// <summary>
        /// 消息句柄
        /// </summary>
        private IServers mNetServer;

        /// <summary>
        /// 处理后的消息队列，用于消息回调
        /// </summary>
        private SwitchQueue<MessagePackage> mMessageQueue;


        private Dictionary<int, NetRoom> mIDToRoomDict = new Dictionary<int, NetRoom>();
        private IMessageHandle mMessageHandle;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="netType"></param>
        /// <param name="isHost"></param>
        /// <param name="args"></param>
        public void Init(NetType netType, NetARQType arqType, IMessageHandle localServers, params object[] args)
        {
            this.mMessageQueue = new SwitchQueue<MessagePackage>(1024);
            this.mNetType = netType;

            if (this.mNetType == NetType.Tcp)
            {
                this.mNetServer = new TcpServers();
            }
            if (this.mNetType == NetType.Multicast)
            {
                throw new NotImplementedException();
            }
            if (this.mNetType == NetType.Udp)
            {
                this.mNetServer = new UdpServers();
            }
            this.mNetServer.Init(this.OnMessage, args);
            this.mNetServer.Connect();
            this.isInit = true;
        }
        /// <summary>
        /// 发送消息到指定客户端
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isSync"></param>
        public void SendTo(int cmdType, int sessionId, object data, bool isSync = false)
        {
            if (!this.isInit)
            {
                Debug.LogError(this + " 没有初始化");
            }
            else if (sessionId != 0)
            {
                MessagePackage emptyPackage = MessagePackage.GetEmptyPackage();
                emptyPackage.SetSessionID(sessionId);
                emptyPackage.SetProtocolID(cmdType);
                emptyPackage.SetOpCode(NetOpCode.BINARY);
                emptyPackage.Data = data;
                this.mNetServer.SendTo(emptyPackage, isSync);
            }
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="cmdType">消息体类型</param>
        /// <param name="data">消息体内容</param>
        /// <param name="sendSessions">要发送的会话id</param>
        /// <param name="cullSessions">过滤的会话id</param>
        /// <param name="isSync">是否异步</param>
        public void Broadcast(int cmdType, object data, int[] sendSessions, int[] cullSessions = null, bool isSync = false)
        {
            if (!this.isInit)
            {
                Debug.LogError(this + " 没有初始化");
            }
            else if (sendSessions != null && sendSessions.Length != 0)
            {
                MessagePackage emptyPackage = MessagePackage.GetEmptyPackage();
                emptyPackage.SetSessionID(0);
                emptyPackage.SetProtocolID(cmdType);
                emptyPackage.SetOpCode(NetOpCode.BINARY);
                emptyPackage.Data = data;
                this.mNetServer.Broadcast(emptyPackage, sendSessions, cullSessions, isSync);
            }
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        private void MessageProcess()
        {
            this.mMessageQueue.Switch();
            if (!this.mMessageQueue.Empty())
            {
                while (!this.mMessageQueue.Empty())
                {
                    MessagePackage package = this.mMessageQueue.Dequeue();
                    mMessageHandle.Process(package);
                }
            }
        }

        /// <summary>
        /// 消息处理，加入回调队列(对应底层的接收信息的回调)
        /// </summary>
        /// <param name="buffer"></param>
        public void OnMessage(MessagePackage package)
        {
            if (!this.isInit)
            {
                Debug.LogError(this + " 没有初始化");
            }
            else
            {
                this.mMessageQueue.Enqueue(package);
            }
        }

        /// <summary>
        /// 循环更新函数，必须接入，否则无法回调
        /// </summary>
        public void OnUpdate()
        {
            if (this.isInit)
            {
                this.mNetServer.Update();
                this.MessageProcess();
            }
        }

        /// <summary>
        /// 关闭处理
        /// </summary>
        public void OnDisable()
        {
            if (this.isInit)
            {
                this.isInit = false;
                this.mMessageQueue = null;
                this.mNetServer.Close();
            }
        }

        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            if (!this.isInit)
            {
                Debug.LogError(this + " 没有初始化");
                return false;
            }
            return this.mNetServer.IsConnected();
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        public void ReConnect()
        {
            if (!this.isInit)
            {
                Debug.LogError(this + " 没有初始化");
            }
            else
            {
                this.mNetServer.ReConnect();
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (!this.isInit)
            {
                Debug.LogError(this + " 没有初始化");
            }
            else
            {
                this.OnDisable();
            }
        }
    }
}
