namespace VRNetLibrary
{
    public interface IClient
    {
        void Init(dReceiveMessageHandle handle, NetARQType arqType, params object[] args);

        void SendMessage(MessagePackage package, bool isSync = false);

        void Connect();

        void ReConnect();

        void Close();

        void Update();

        bool IsConnected();

        double GetNetDelay();

        void SetCheckNetDelayStatus(bool status);
    }
}
