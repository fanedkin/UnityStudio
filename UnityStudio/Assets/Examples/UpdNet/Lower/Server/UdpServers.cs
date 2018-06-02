using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace VRNetLibrary
{
    /// <summary>
    /// UDP服务器
    /// </summary>
    public class UdpServers : IServers
    {
        private IPEndPoint m_LocalIPEndPonit;

        private ISocket mSocket;

        private dReceiveMessageHandle m_MessageHandle;

        /// <summary>
        /// 包索引生成器字典，key=SessionId
        /// </summary>
        private Dictionary<int, NetMsgIndexGenerator> mMsgIndexGeneratorDic = new Dictionary<int, NetMsgIndexGenerator>();

        private IPEndPoint[] IPEndPoints;

        private Dictionary<IPEndPoint, int> ipToSessionId = new Dictionary<IPEndPoint, int>();

        private Dictionary<int, IPEndPoint> sessionIdToIp = new Dictionary<int, IPEndPoint>();

        private NetARQType mARQType = NetARQType.None;

        /// <summary>
        /// ping工具
        /// </summary>
        private PingTools pingTools = new PingTools();

        public void Close()
        {
            if (this.mSocket != null)
            {
                this.mSocket.Close();
                this.mSocket = null;
            }
        }

        public void Connect()
        {
            try
            {
                this.Close();
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
                    if (this.CheckValidIP(ipInfo) && receiveBytes != null && receiveBytes.Length != 0)
                    {
                        this.mOnReceive(receiveBytes, receiveBytes.Length, ipInfo);
                    }
                });
                this.mSocket.Connect(this.m_LocalIPEndPonit, this.IPEndPoints);
            }
            catch (Exception arg)
            {
                this.Close();
                Debug.LogError("Connect Error!!!" + arg);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle">接受消息回调</param>
        /// <param name="arqType"></param>
        /// <param name="args">args[0]=服务器ip,args[1]=服务器端口，args[2]=客户端ip数组，args[3]=客户端端口数组</param>
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
            try
            {
                this.m_LocalIPEndPonit = new IPEndPoint(IPAddress.Parse((string)args[0]), (int)args[1]);
                string[] ips = args[2] as string[];
                int[] ponits = args[3] as int[];
                if (ips.Length == 0 || ponits.Length == 0 || ips.Length != ponits.Length)
                {
                    Debug.LogError(base.GetType() + " 客户端ip或者端口 错误");
                }
                this.ipToSessionId.Add(this.m_LocalIPEndPonit, -1);
                this.sessionIdToIp.Add(-1, this.m_LocalIPEndPonit);
                this.IPEndPoints = new IPEndPoint[ips.Length];
                for (int i = 0; i < ips.Length; i++)
                {
                    IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ips[i]), ponits[i]);
                    int hashCode = iPEndPoint.GetHashCode();
                    this.ipToSessionId.Add(iPEndPoint, hashCode);
                    this.sessionIdToIp.Add(hashCode, iPEndPoint);
                    this.IPEndPoints[i] = iPEndPoint;
                }
                this.m_MessageHandle = handle;
            }
            catch (Exception arg)
            {
                Debug.LogError(base.GetType() + " Init error!!!" + arg);
            }
        }

        public bool IsConnected()
        {
            if (this.mSocket != null)
            {
                return true;
            }
            return false;
        }

        public void ReConnect()
        {
            this.Connect();
        }

        public void Update()
        {
            this.pingTools.Update();
        }

        /// <summary>
        /// 根据ip获取SessionID
        /// </summary>
        /// <returns></returns>
        public int GetSessionID(IPEndPoint ipinfo)
        {
            int result = default(int);
            if (this.ipToSessionId.TryGetValue(ipinfo, out result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        /// 根据SessionID获取ip
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        public IPEndPoint GetIp(int sessionID)
        {
            IPEndPoint result = default(IPEndPoint);
            if (this.sessionIdToIp.TryGetValue(sessionID, out result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 单播
        /// </summary>
        /// <param name="package"></param>
        /// <param name="isSync"></param>
        public void SendTo(MessagePackage package, bool isSync = false)
        {
            this.Send_internal(package, false);
        }

        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="package"></param>
        /// <param name="isSync"></param>
        public void Broadcast(MessagePackage package, int[] sendSessions, int[] cullSessions = null, bool isSync = false)
        {
            if (sendSessions != null && sendSessions.Length != 0)
            {
                foreach (int num in sendSessions)
                {
                    if (this.CheckValidSessionID(num) && !this.CheckCull(cullSessions, num))
                    {
                        package.SetSessionID(num);
                        this.Send_internal(package, false);
                    }
                }
            }
        }

        ~UdpServers()
        {
            this.Close();
        }

        /// <summary>
        /// 检测是否有效ip
        /// </summary>
        /// <param name="ipinfo"></param>
        /// <returns></returns>
        private bool CheckValidIP(IPEndPoint ipinfo)
        {
            return this.ipToSessionId.ContainsKey(ipinfo);
        }

        /// <summary>
        /// 检测是否有效SessionID
        /// </summary>
        /// <param name="ipinfo"></param>
        /// <returns></returns>
        private bool CheckValidSessionID(int sessionID)
        {
            return this.sessionIdToIp.ContainsKey(sessionID);
        }

        /// <summary>
        /// 根据sessionid获取包索引
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        private uint GetMsgIndexBySessionId(int sessionId)
        {
            NetMsgIndexGenerator netMsgIndexGenerator = default(NetMsgIndexGenerator);
            if (!this.mMsgIndexGeneratorDic.TryGetValue(sessionId, out netMsgIndexGenerator))
            {
                netMsgIndexGenerator = new NetMsgIndexGenerator();
                this.mMsgIndexGeneratorDic.Add(sessionId, netMsgIndexGenerator);
            }
            return netMsgIndexGenerator.GetIndex;
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
            package.SetSessionID(this.GetSessionID(ipinfo));
            if (package.Head.OpCode == NetOpCode.BINARY)
            {
                this.mOnReceivedMessage(package);
            }
            else if (package.Head.OpCode == NetOpCode.PING)
            {
                MessagePackage pongPackage = this.pingTools.pongPackage;
                pongPackage.SetSessionID(package.Head.SessionID);
                this.SendTo(pongPackage, false);
            }
            else if (package.Head.OpCode != NetOpCode.PONG && package.Head.OpCode != NetOpCode.HEART)
            {
                Debug.LogError("为什么会有空包出现？ OpCode=NetOpCode.EMPTY,ipinfo=" + ipinfo);
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

        private void Send_internal(MessagePackage package, bool isSync = false)
        {
            if (this.mSocket == null)
            {
                Debug.LogError(this + " m_Client == null!!!");
            }
            package.SetMsgID(this.GetMsgIndexBySessionId(package.Head.SessionID));
            if (package.Head.SessionID != -1)
            {
                IPEndPoint ip = this.GetIp(package.Head.SessionID);
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
        /// 判断是否过滤
        /// </summary>
        /// <param name="cullSessions"></param>
        /// <param name="curSessionID"></param>
        /// <returns></returns>
        private bool CheckCull(int[] cullSessions, int curSessionID)
        {
            if (cullSessions == null)
            {
                return false;
            }
            if (Array.IndexOf(cullSessions, curSessionID) >= 0)
            {
                return true;
            }
            return false;
        }
    }
}
