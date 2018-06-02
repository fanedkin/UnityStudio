using System;
using System.Net;

namespace VRNetLibrary
{
    public interface ISocket
    {
        void Init(Action<IPEndPoint, byte[]> receiveCallback, params object[] args);
        void Connect(IPEndPoint localEndPoint, params IPEndPoint[] ipArgs);
        void Close();
        void Send(IPEndPoint ipInfo, byte[] bytes, int length, bool isSync = false);
    }
}
