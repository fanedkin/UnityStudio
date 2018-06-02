// Decompiled with JetBrains decompiler
// Type: VRNetLibrary.UdpSocket
// Assembly: VRNetLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9495A955-A935-423F-9B13-668FE7A3FD9E
// Assembly location: E:\工作\VR_LargeSceneGame_PVE\Assets\Plugins\VRNetLib.dll

using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace VRNetLibrary
{
    /// <summary>Udp服务</summary>
    internal class UdpSocket : ISocket
    {
        /// <summary>udp客户端</summary>
        private UdpClient mClient;
        /// <summary>接受回调</summary>
        private Action<IPEndPoint, byte[]> mReceiveCallback;
        /// <summary>临时ip变量</summary>
        private IPEndPoint mTempIPEndPonit;

        /// <summary>关闭</summary>
        public void Close()
        {
            if (this.mClient == null)
                return;
            this.mClient.Close();
            this.mClient = (UdpClient)null;
        }

        /// <summary>连接</summary>
        /// <param name="localEndPoint"></param>
        public void Connect(IPEndPoint localEndPoint, params IPEndPoint[] ipArgs)
        {
            try
            {
                this.Close();
                this.mClient = new UdpClient(localEndPoint);
                this.mClient.BeginReceive(new AsyncCallback(this.ReceiveCB), (object)this);
            }
            catch (Exception ex)
            {
                Debug.LogError((object)("UdpSocket.Connect " + (object)ex));
            }
        }

        /// <summary>初始化</summary>
        /// <param name="receiveCallback"></param>
        public void Init(Action<IPEndPoint, byte[]> receiveCallback, params object[] args)
        {
            if (receiveCallback == null)
                Debug.LogError((object)"UdpSocket.Init receiveCallback == null");
            else
                this.mReceiveCallback = receiveCallback;
        }

        /// <summary>发送</summary>
        /// <param name="bytes"></param>
        /// <param name="isSync"></param>
        public void Send(IPEndPoint ipInfo, byte[] bytes, int length, bool isSync = false)
        {
            if (this.mClient == null)
                Debug.LogError((object)"UdpSocket.Send mClient == null");
            else if (bytes == null || bytes.Length == 0)
                Debug.LogError((object)"UdpSocket.Send bytes == null");
            else if (isSync)
                this.mClient.Send(bytes, length, ipInfo);
            else
                this.mClient.BeginSend(bytes, length, ipInfo, new AsyncCallback(this.SendCB), (object)this);
        }

        /// <summary>BeginReceive回调方法</summary>
        /// <param name="ar"></param>
        private void ReceiveCB(IAsyncResult ar)
        {
            if (this.mClient == null)
            {
                Debug.LogError((object)"UdpSocket.ReceiveCB mClient == null 如果是关闭游戏看到此报错,则是正常的.");
            }
            else
            {
                try
                {
                    byte[] numArray = this.mClient.EndReceive(ar, ref this.mTempIPEndPonit);
                    if (numArray == null || numArray.Length == 0)
                        Debug.LogError((object)("UdpSocket.ReceiveCB receiveBytes==null ipInfo=" + this.mTempIPEndPonit.ToString()));
                    else if (this.mReceiveCallback != null)
                        this.mReceiveCallback(this.mTempIPEndPonit, numArray);
                    else
                        Debug.LogError((object)("UdpSocket.ReceiveCB mReceiveCallback==null ipInfo=" + this.mTempIPEndPonit.ToString()));
                }
                catch (Exception ex)
                {
                    Debug.LogError((object)("UdpSocket.ReceiveCB Exception!!! ipInfo=" + this.mTempIPEndPonit.ToString()));
                    Debug.LogException(ex);
                }
                finally
                {
                    if (this.mClient != null)
                        this.mClient.BeginReceive(new AsyncCallback(this.ReceiveCB), (object)this);
                }
            }
        }

        /// <summary>BeginSend回调方法</summary>
        /// <param name="ar"></param>
        private void SendCB(IAsyncResult ar)
        {
            if (this.mClient == null)
                UnityEngine.Debug.LogError((object)(this.ToString() + " SendCB mClient == null!!!"));
            else if (ar == null)
                Debug.LogError((object) (this.ToString() + " SendCB IAsyncResult == null!!!"));
            else
            {
                int sendCount = mClient.EndSend(ar);
                if (sendCount == 0)
                { Debug.LogError("Send a message failure..."); }
            }
        }
    }
}
