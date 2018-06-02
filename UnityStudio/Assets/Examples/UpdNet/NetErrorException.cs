// Decompiled with JetBrains decompiler
// Type: VRNetLibrary.NetErrorException
// Assembly: VRNetLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9495A955-A935-423F-9B13-668FE7A3FD9E
// Assembly location: E:\工作\VR_LargeSceneGame\Assets\Plugins\VRNetLib.dll

using System;

namespace VRNetLibrary
{
    internal class NetErrorException : ApplicationException
    {
        /// <summary>默认构造函数</summary>
        public NetErrorException()
        {
        }

        public NetErrorException(string message)
          : base(message)
        {
        }

        public NetErrorException(string message, Exception inner)
          : base(message, inner)
        {
        }
    }
}
