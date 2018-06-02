using System.Net;

namespace VRNetLibrary
{
    public interface IServers
    {
        void Init(dReceiveMessageHandle handle,params object[] args);

        void SendTo(MessagePackage package, bool isSync = false);

        void Broadcast(MessagePackage package, int[] sendSessions, int[] cullSessions = null, bool isSync = false);

        void Connect();

        void ReConnect();

        void Close();

        void Update();

        bool IsConnected();

    }
}
