namespace VRNetLibrary
{
    /// <summary>
    /// 字节结构包装
    /// </summary>
    public struct MessageBytes
    {
        public byte[] Bytes;

        public int Offset;

        public int Length;

        public byte[] GetBytes()
        {
            return this.Bytes;
        }

        public int GetLength()
        {
            return this.Length;
        }

        public int GetOffset()
        {
            return this.Length;
        }
    }
}
