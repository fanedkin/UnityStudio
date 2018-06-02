// Decompiled with JetBrains decompiler
// Type: Net.Converter
// Assembly: VRNetLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9495A955-A935-423F-9B13-668FE7A3FD9E
// Assembly location: E:\工作\VR_LargeSceneGame\Assets\Plugins\VRNetLib.dll

using System;

namespace Net
{
    public class Converter
    {
        public static int GetBigEndian(int value)
        {
            if (BitConverter.IsLittleEndian)
                return (int)Converter.swapByteOrder((uint)value);
            return value;
        }

        public static ushort GetBigEndian(ushort value)
        {
            if (BitConverter.IsLittleEndian)
                return Converter.swapByteOrder(value);
            return value;
        }

        public static uint GetBigEndian(uint value)
        {
            if (BitConverter.IsLittleEndian)
                return Converter.swapByteOrder(value);
            return value;
        }

        public static long GetBigEndian(long value)
        {
            if (BitConverter.IsLittleEndian)
                return (long)Converter.swapByteOrder((ulong)value);
            return value;
        }

        public static double GetBigEndian(double value)
        {
            if (BitConverter.IsLittleEndian)
                return (double)Converter.swapByteOrder((ulong)value);
            return value;
        }

        public static float GetBigEndian(float value)
        {
            if (BitConverter.IsLittleEndian)
                return (float)Converter.swapByteOrder((uint)value);
            return value;
        }

        public static int GetLittleEndian(int value)
        {
            if (BitConverter.IsLittleEndian)
                return value;
            return (int)Converter.swapByteOrder((uint)value);
        }

        public static uint GetLittleEndian(uint value)
        {
            if (BitConverter.IsLittleEndian)
                return value;
            return Converter.swapByteOrder(value);
        }

        public static ushort GetLittleEndian(ushort value)
        {
            if (BitConverter.IsLittleEndian)
                return value;
            return Converter.swapByteOrder(value);
        }

        public static double GetLittleEndian(double value)
        {
            if (BitConverter.IsLittleEndian)
                return value;
            return (double)Converter.swapByteOrder((ulong)value);
        }

        private static ulong swapByteOrder(ulong value)
        {
            return (ulong)((long)byte.MaxValue & (long)(value >> 56) | 65280L & (long)(value >> 40) | 16711680L & (long)(value >> 24) | 4278190080L & (long)(value >> 8) | 1095216660480L & (long)value << 8 | 280375465082880L & (long)value << 24 | 71776119061217280L & (long)value << 40 | -72057594037927936L & (long)value << 56);
        }

        private static ushort swapByteOrder(ushort value)
        {
            return (ushort)((int)byte.MaxValue & (int)value >> 8 | 65280 & (int)value << 8);
        }

        private static uint swapByteOrder(uint value)
        {
            return (uint)((int)byte.MaxValue & (int)(value >> 24) | 65280 & (int)(value >> 8) | 16711680 & (int)value << 8 | -16777216 & (int)value << 24);
        }
    }
}
