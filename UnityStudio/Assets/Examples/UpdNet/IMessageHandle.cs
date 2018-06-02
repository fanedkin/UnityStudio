namespace VRNetLibrary
{
    public interface IMessageHandle
    {
        void Process(MessagePackage package);
    }
}
