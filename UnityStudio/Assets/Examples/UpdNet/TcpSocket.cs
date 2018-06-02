using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace VRNetLibrary
{
    public class TcpSocket : ISocket
    {
        private TcpListener tcpListener;
        //最大链接数
        public int maxConn = 50;
        //客户端链接
        public Conn[] conns;
        private Action<IPEndPoint, byte[]> mReceiveCallback;
        
        #region 接口公共方法
        public void Init(Action<IPEndPoint, byte[]> receiveCallback, params object[] args)
        {
            if (receiveCallback == null)
                Debug.LogError((object)"TcpSocket.Init receiveCallback == null");
            else
                this.mReceiveCallback = receiveCallback;
            conns = new Conn[maxConn];
            for (int i = 0; i < maxConn; i++)
            {
                conns[i] = new Conn(i);
            }
        }
        /// <summary>连接</summary>
        /// <param name="localEndPoint"></param>
        public void Connect(IPEndPoint localIPEndPoint, params IPEndPoint[] ipArgs)
        {
            try
            {
                this.Close();
                TcpListener tcpListener = new TcpListener(localIPEndPoint);
                tcpListener.Start(maxConn);
                tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), tcpListener);
            }
            catch (Exception ex)
            {
                Debug.LogError((object)("TcpSocket.Connect " + (object)ex));
            }
        }
        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                TcpListener lstn = (TcpListener)ar.AsyncState;
                Socket socket = lstn.EndAcceptSocket(ar);
                int index = NewIndex();

                if (index < 0)
                {
                    socket.Close();
                    Console.Write("[警告]链接已满");
                }
                else
                {
                    Conn conn = conns[index];
                    conn.Init(socket, index);
                    HostServer.Instance.AddConnToDict(conn.sessionID, conn);
                    HostServer.Instance.AddConnToDict(conn.iPEndPoint, conn);
                    string adr = conn.GetAdress();
                    Console.WriteLine("客户端连接 [" + adr + "] conn池ID：" + index);
                    conn.socket.BeginReceive(conn.readBuff,
                                             conn.buffCount, conn.BuffRemain(),
                                             SocketFlags.None, ReceiveCb, conn);
                }
                tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), tcpListener);
            }
            catch (Exception e)
            {
                Console.WriteLine("AcceptCb失败:" + e.Message);
            }
        }

        public void Send(IPEndPoint ipInfo, byte[] bytes, int length, bool isSync = false)
        {
            Conn conn = HostServer.Instance.GetConnByIPEndPoint(ipInfo);
            if (conn == null)
            {
                Debug.LogError(string.Format("TcoSocket.Send Conn{0} == null", conn.index));
                return;
            }
            byte[] lengthByte = BitConverter.GetBytes(length);
            byte[] sendbuff = lengthByte.Concat(bytes).ToArray();
            try
            {
                conn.socket.BeginSend(sendbuff, 0, sendbuff.Length, SocketFlags.None, null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("[发送消息]" + conn.GetAdress() + " : " + e.Message);
            }
        }
        public void Close()
        {
            throw new NotImplementedException();
        }



        #endregion


        #region 私有方法
        //最底层的接收信息回调
        private void ReceiveCb(IAsyncResult ar)
        {
            Conn conn = (Conn)ar.AsyncState;
            lock (conn)
            {
                try
                {
                    //收到的数据长度
                    int count = conn.socket.EndReceive(ar);
                    //关闭信号
                    if (count <= 0)
                    {
                        Console.WriteLine("收到 [" + conn.GetAdress() + "] 断开链接");
                        conn.Close();
                        return;
                    }
                    conn.buffCount += count;
                    ProcessData(conn);
                    //继续接收	
                    conn.socket.BeginReceive(conn.readBuff,
                                             conn.buffCount, conn.BuffRemain(),
                                             SocketFlags.None, ReceiveCb, conn);
                }
                catch (Exception e)
                {
                    Console.WriteLine("收到 [" + conn.GetAdress() + "] 断开链接 " + e.Message);
                    conn.Close();
                }
            }
        }
        //处理数据
        private void ProcessData(Conn conn)
        {
            //小于长度字节
            if (conn.buffCount < sizeof(Int32))
            {
                return;
            }
            //消息长度
            Array.Copy(conn.readBuff, conn.lenBytes, sizeof(Int32));
            conn.msgLength = BitConverter.ToInt32(conn.lenBytes, 0);
            if (conn.buffCount < conn.msgLength + sizeof(Int32))
            {
                return;
            }
            //处理消息
            //ProtocolBase protocol = proto.Decode(conn.readBuff, sizeof(Int32), conn.msgLength);
            if (this.mReceiveCallback != null)
                this.mReceiveCallback(conn.iPEndPoint, conn.readBuff);
            //清除已处理的消息
            int count = conn.buffCount - conn.msgLength - sizeof(Int32);
            Array.Copy(conn.readBuff, sizeof(Int32) + conn.msgLength, conn.readBuff, 0, count);
            conn.buffCount = count;
            if (conn.buffCount > 0)
            {
                ProcessData(conn);
            }
        }


        //获取链接池索引，返回负数表示获取失败
        private int NewIndex()
        {
            if (conns == null)
                return -1;
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                {
                    conns[i] = new Conn(i);
                    return i;
                }
                else if (conns[i].isUse == false)
                {
                    return i;
                }
            }
            return -1;
        }
        #endregion
    }
}

