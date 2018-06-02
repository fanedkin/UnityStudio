using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace VRNetLibrary
{
    public class TcpServers : IServers
    {
        /// <summary>
        /// 服务器本机的IP和端口
        /// </summary>
        private IPEndPoint m_LocalIPEndPonit;
        /// <summary>
        /// TcpSocket句柄
        /// </summary>
        private TcpSocket mSocket;
        /// <summary>
        /// 消息回调委托
        /// </summary>
        private dReceiveMessageHandle m_MessageHandle;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="handle">回调委托</param>
        /// <param name="args">其他补充参数</param>
        public void Init(dReceiveMessageHandle handle, params object[] args)
        {
            if (handle == null)
            {
                Debug.LogError(base.GetType() + " handle 不能为空");
            }
            if (args == null || (args != null && args.Length < 3))
            {
                Debug.LogError(base.GetType() + " arg 错误");
            }
            //获取自身的IP和端口
            this.m_LocalIPEndPonit = new IPEndPoint(IPAddress.Parse((string)args[0]), (int)args[1]);
            m_MessageHandle = handle;
        }
        /// <summary>
        /// 服务器启动连接
        /// </summary>
        public void Connect()
        {
            this.mSocket = new TcpSocket();
            try
            {
                this.Close();
                this.mSocket = new TcpSocket();
                this.mSocket.Init(delegate (IPEndPoint iPEndPoint, byte[] buff)
                {
                    if (iPEndPoint != null&& buff != null && buff.Length!= 0)
                    {
                        mOnReceive(buff, buff.Length, iPEndPoint);
                    }
                });
                this.mSocket.Connect(this.m_LocalIPEndPonit);
            }
            catch (Exception arg)
            {
                this.Close();
                Debug.LogError("Connect Error!!!" + arg);
            }
        }
        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="package"></param>
        /// <param name="sendSessions"></param>
        /// <param name="cullSessions"></param>
        /// <param name="isSync"></param>
        void IServers.Broadcast(MessagePackage package, int[] sendSessions, int[] cullSessions = null, bool isSync = false)
        {
            if (sendSessions != null && sendSessions.Length != 0)
            {
                foreach (int num in sendSessions)
                {

                    package.SetSessionID(num);
                    this.Send_internal(package, false);

                }
            }
        }
        /// <summary>
        /// 发送消息到指定IP
        /// </summary>
        /// <param name="package"></param>
        /// <param name="isSync"></param>
        void IServers.SendTo(MessagePackage package, bool isSync = false)
        {
            this.Send_internal(package, false);
        }

        /// <summary>
        /// 关闭服务器
        /// </summary>
        public void Close()
        {
            if (this.mSocket != null)
            {
                this.mSocket.Close();
                this.mSocket = null;
            }
        }

        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            if (this.mSocket != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 重连
        /// </summary>
        public void ReConnect()
        {
            this.Connect();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
        public TcpSocket Socket
        {
            get { return mSocket; }
        }
        #region 私有方法
        private void Send_internal(MessagePackage package, bool isSync = false)
        {
            if (this.mSocket == null)
            {
                Debug.LogError(this + " m_Client == null!!!");
            }
            if (package.Head.SessionID != -1)
            {
                IPEndPoint ip = null;
                MessageBytes messageBytes = package.ToBytes();
                if (messageBytes.GetLength() == 0)
                {
                    Debug.LogError(this + " ContentLength=0,ProtocolID=" + package.Head.ProtocolID + " MsgID=" + package.Head.MsgID + " SessionID=" + package.Head.SessionID);
                }
                this.mSocket.Send(ip, messageBytes.GetBytes(), messageBytes.GetLength(), isSync);
            }
            else
            {
                Singleton<ClientManager>.Instance.OnMessage(package);
            }
        }
        /// <summary>
        /// 处理消息
        /// </summary>
        private void mOnReceive(byte[] bytes, int length, IPEndPoint ipinfo)
        {
            byte[] array = bytes;
            if (bytes.Length != length)
            {
                array = new byte[length];
                Buffer.BlockCopy(bytes, 0, array, 0, length);
            }
            MessagePackage package = MessagePackage.ToPackage(array);
            package.SetSessionID(HostServer.Instance.GetConnByIPEndPoint(ipinfo).sessionID);
            if (package.Head.OpCode == NetOpCode.BINARY)
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
            //else if (package.Head.OpCode == NetOpCode.PING)
            //{
            //    MessagePackage pongPackage = this.pingTools.pongPackage;
            //    pongPackage.SetSessionID(package.Head.SessionID);
            //    this.SendTo(pongPackage, false);
            //}
            else if (package.Head.OpCode != NetOpCode.PONG && package.Head.OpCode != NetOpCode.HEART)
            {
                Debug.LogError("为什么会有空包出现？ OpCode=NetOpCode.EMPTY,ipinfo=" + ipinfo);
            }
        }
        #endregion

    }
}

