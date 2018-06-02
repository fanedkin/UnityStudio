using System;
using UnityEngine;

namespace VRNetLibrary
{
    /// <summary>
    /// 客户端管理器
    /// </summary>
    public class ClientManager : Singleton<ClientManager>
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
        /// 当前网络通讯类型
        /// </summary>
        private NetARQType mNetARQType = NetARQType.None;

        /// <summary>
        /// 消息句柄
        /// </summary>
        private IClient mNetServer;

        /// <summary>
        /// 处理后的消息队列，用于消息回调
        /// </summary>
        private SwitchQueue<MessagePackage> mMessageQueue;

        /// <summary>
        /// 消息句柄
        /// </summary>
        private IMessageHandle mMessageHandle;

        public void Init(NetType netType, NetARQType arqType, IMessageHandle messageHandle, params object[] args)
        {
            this.mMessageQueue = new SwitchQueue<MessagePackage>(1024);
            this.mNetType = netType;
            this.mNetARQType = arqType;
            this.mMessageHandle = messageHandle;
            if (this.mNetType == NetType.Local)
            {
                this.mNetServer = new LocalClientServer();
            }
            else
            {
                if (this.mNetType == NetType.Tcp)
                {
                    throw new NotImplementedException();
                }
                if (this.mNetType == NetType.Multicast)
                {
                    throw new NotImplementedException();
                }
                if (this.mNetType == NetType.Udp)
                {
                    this.mNetServer = new UdpClientServer();
                }
            }
            this.mNetServer.Init(this.OnMessage, arqType, args);
            this.mNetServer.Connect();
            this.isInit = true;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isSync"></param>
        public void SendMessage(int cmdType, object data, bool isSync = false)
        {
            if (!this.isInit)
            {
                Debug.LogError(this + " 没有初始化");
            }
            else
            {
                MessagePackage emptyPackage = MessagePackage.GetEmptyPackage();
                emptyPackage.SetProtocolID(cmdType);
                emptyPackage.SetOpCode(NetOpCode.BINARY);
                emptyPackage.Data = data;
                this.mNetServer.SendMessage(emptyPackage, isSync);
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
                    this.mMessageHandle.Process(package);
                }
            }
        }

        /// <summary>
        /// 消息处理，加入回调队列
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
            if (!this.isInit)
            {
                Debug.LogError(this + " 没有初始化");
            }
            else
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

        /// <summary>
        /// 获取延迟
        /// </summary>
        /// <returns></returns>
        public double GetNetDelay()
        {
            if (!this.isInit)
            {
                Debug.LogError(this + " 没有初始化");
                return 0.0;
            }
            return this.mNetServer.GetNetDelay();
        }

        /// <summary>
        /// 是否检测延迟
        /// </summary>
        /// <param name="_status"></param>
        public void SetCheckNetDelayStatus(bool _status)
        {
            if (!this.isInit)
            {
                Debug.LogError(this + " 没有初始化");
            }
            else
            {
                this.mNetServer.SetCheckNetDelayStatus(_status);
            }
        }
    }
}
