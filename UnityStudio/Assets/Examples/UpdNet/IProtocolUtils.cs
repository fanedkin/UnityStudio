namespace VRNetLibrary
{
    /// <summary>
    /// 协议工具接口
    /// </summary>
    public interface IProtocolUtils
    {
        byte[] Serialize(object protoData);

        T DeSerialize<T>(byte[] bytes);
    }
}
