namespace VRNetLibrary
{
    /// <summary>
    /// 包索引生成器
    /// Vehicle
    /// 2017.4.24
    /// </summary>
    public class NetMsgIndexGenerator
    {
        /// <summary>
        /// 包索引值
        /// </summary>
        private uint mMsgIndex;

        /// <summary>
        /// 包索引值，自增
        /// </summary>
        public uint GetIndex
        {
            get
            {
                this.mMsgIndex = ((this.mMsgIndex == 4294967295u) ? 1 : (this.mMsgIndex += 1u));
                return this.mMsgIndex;
            }
        }
    }
}
