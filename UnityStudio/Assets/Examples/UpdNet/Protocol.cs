using UnityEngine;

namespace VRNetLibrary
{
    /// <summary>
    /// 协议工具，通用化的协议工具
    /// </summary>
    public class Protocol : Singleton<Protocol>
    {
        /// <summary>
        /// 工具接口
        /// </summary>
        private IProtocolUtils mProtocolUtils;

        /// <summary>
        /// 初始化工具
        /// </summary>
        /// <param name="utils"></param>
        public void init(IProtocolUtils utils)
        {
            this.mProtocolUtils = utils;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public T DeSerialize<T>(byte[] bytes) where T : class
        {
            if (this.mProtocolUtils == null)
            {
                Debug.LogError("Protocol 工具没有初始化!!!");
            }
            return this.mProtocolUtils.DeSerialize<T>(bytes);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Serialize(object data)
        {
            if (this.mProtocolUtils == null)
            {
                Debug.LogError("Protocol 工具没有初始化!!!");
            }
            return this.mProtocolUtils.Serialize(data);
        }
    }
}
