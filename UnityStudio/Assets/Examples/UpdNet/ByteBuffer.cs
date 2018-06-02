// Decompiled with JetBrains decompiler
// Type: VRNetLibrary.ByteBuffer
// Assembly: VRNetLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9495A955-A935-423F-9B13-668FE7A3FD9E
// Assembly location: E:\工作\VR_LargeSceneGame\Assets\Plugins\VRNetLib.dll

using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace VRNetLibrary
{
    public struct ByteBuffer
    {
        private static ByteBuffer empty = new ByteBuffer(0);
        private byte[] buffers;
        private int readerIndex;
        private int writerIndex;

        public static ByteBuffer Empty
        {
            get
            {
                return ByteBuffer.empty;
            }
        }

        /// <summary>带参构造函数 向字节数组中 写字节</summary>
        /// <param name="buffers">字节数组</param>
        public ByteBuffer(byte[] buffers)
        {
            this = new ByteBuffer(buffers.Length);
            this.Write(buffers);
        }

        /// <summary>带参构造函数 初始化字节数组</summary>
        /// <param name="initCapacity">初始容量</param>
        public ByteBuffer(int initCapacity)
        {
            this.buffers = new byte[initCapacity];
            this.readerIndex = 0;
            this.writerIndex = 0;
        }

        /// <summary>写一个byte数组到缓冲区，位置从dataOffset</summary>
        /// <param name="data"></param>
        /// <param name="dataOffset"></param>
        /// <param name="dataSize"></param>
        public void Write(byte[] data, int dataOffset, int dataSize)
        {
            this.EnsureWritable(dataSize);
            Buffer.BlockCopy((Array)data, dataOffset, (Array)this.buffers, this.writerIndex, dataSize);
            this.writerIndex = this.writerIndex + dataSize;
        }

        /// <summary>写一个byte数组到缓冲区</summary>
        /// <param name="data"></param>
        public void Write(byte[] data)
        {
            this.Write(data, 0, data.Length);
        }

        /// <summary>写一个byte</summary>
        /// <param name="data"></param>
        public void Write(byte data)
        {
            this.Write(new byte[1] { data });
        }

        /// <summary>写一个ByteBuffer</summary>
        /// <param name="buff"></param>
        public void Write(ByteBuffer buff)
        {
            this.Write(buff.ReadBytes(buff.ReadableBytes()));
        }

        /// <summary>写一个bool</summary>
        /// <param name="data"></param>
        public void Write(bool data)
        {
            this.Write(BitConverter.GetBytes(data));
        }

        /// <summary>写一个ushort</summary>
        /// <param name="data"></param>
        public void Write(ushort data)
        {
            this.Write(BitConverter.GetBytes(data));
        }

        /// <summary>写一个short</summary>
        /// <param name="data"></param>
        public void Write(short data)
        {
            this.Write(BitConverter.GetBytes(data));
        }

        /// <summary>写一个uint</summary>
        /// <param name="data"></param>
        public void Write(uint data)
        {
            this.Write(BitConverter.GetBytes(data));
        }

        /// <summary>写一个int</summary>
        /// <param name="data"></param>
        public void Write(int data)
        {
            this.Write(BitConverter.GetBytes(data));
        }

        /// <summary>写一个 float</summary>
        /// <param name="data"></param>
        public void Write(float data)
        {
            this.Write(BitConverter.GetBytes(data));
        }

        /// <summary>写一个 Double</summary>
        /// <param name="data"></param>
        public void Write(double data)
        {
            this.Write(BitConverter.GetBytes(data));
        }

        /// <summary>写一个UInt64</summary>
        /// <param name="data"></param>
        public void Write(ulong data)
        {
            this.Write(BitConverter.GetBytes(data));
        }

        /// <summary>写一个int64</summary>
        /// <param name="data"></param>
        public void Write(long data)
        {
            this.Write(BitConverter.GetBytes(data));
        }

        /// <summary>写一个string</summary>
        /// <param name="data"></param>
        public void Write(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            this.Write((ushort)bytes.Length);
            this.Write(bytes);
        }

        /// <summary>写一个结构体</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void Write<T>(T obj) where T : struct
        {
            this.Write(ByteBuffer.StructToBytes<T>(obj));
        }

        /// <summary>读一个byte[]</summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int length)
        {
            byte[] numArray = new byte[length];
            Buffer.BlockCopy((Array)this.buffers, this.readerIndex, (Array)numArray, 0, length);
            this.readerIndex = this.readerIndex + length;
            return numArray;
        }

        /// <summary>读一个byte</summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            int buffer = (int)this.buffers[this.readerIndex];
            this.readerIndex = this.readerIndex + 1;
            return (byte)buffer;
        }

        /// <summary>读一个Bool</summary>
        /// <returns></returns>
        public bool ReadBool()
        {
            return BitConverter.ToBoolean(this.ReadBytes(1), 0);
        }

        /// <summary>读一个short</summary>
        /// <returns></returns>
        public short ReadShort()
        {
            return BitConverter.ToInt16(this.ReadBytes(2), 0);
        }

        /// <summary>读一个ushort</summary>
        /// <returns></returns>
        public ushort ReadUShort()
        {
            return BitConverter.ToUInt16(this.ReadBytes(2), 0);
        }

        /// <summary>读一个float</summary>
        /// <returns></returns>
        public float ReadFloat()
        {
            return BitConverter.ToSingle(this.ReadBytes(4), 0);
        }

        /// <summary>读一个Double</summary>
        /// <returns></returns>
        public double ReadDouble()
        {
            return BitConverter.ToDouble(this.ReadBytes(8), 0);
        }

        /// <summary>读一个int64</summary>
        /// <returns></returns>
        public long ReadInt64()
        {
            return BitConverter.ToInt64(this.ReadBytes(8), 0);
        }

        /// <summary>读一个uint64</summary>
        /// <returns></returns>
        public ulong ReadUInt64()
        {
            return BitConverter.ToUInt64(this.ReadBytes(8), 0);
        }

        /// <summary>读一个int</summary>
        /// <returns></returns>
        public int ReadInt()
        {
            return BitConverter.ToInt32(this.ReadBytes(4), 0);
        }

        /// <summary>读一个uint</summary>
        /// <returns></returns>
        public uint ReadUInt()
        {
            return BitConverter.ToUInt32(this.ReadBytes(4), 0);
        }

        /// <summary>读一个string</summary>
        /// <returns></returns>
        public string ReadString()
        {
            return Encoding.UTF8.GetString(this.ReadBytes((int)this.ReadUShort()));
        }

        /// <summary>read一个ByteBuffer</summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public ByteBuffer ReadBuffer(int length)
        {
            return new ByteBuffer(this.ReadBytes(length));
        }

        /// <summary>读一个结构体</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public T ReadStruct<T>() where T : struct
        {
            return ByteBuffer.ByteToStruct<T>(this.ReadBytes(Marshal.SizeOf(typeof(T))));
        }

        public void Reset()
        {
            this.readerIndex = 0;
            this.writerIndex = 0;
        }

        public void Reset(byte[] _buffers)
        {
            this.readerIndex = 0;
            this.writerIndex = 0;
            this.buffers = _buffers;
        }

        /// <summary>获取一个字符串的byte字节长度</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int GetStringBytesLength(string data)
        {
            return Encoding.UTF8.GetBytes(data).Length;
        }

        /// <summary>获取byte数组</summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] numArray = new byte[this.buffers.Length];
            Buffer.BlockCopy((Array)this.buffers, 0, (Array)numArray, 0, this.buffers.Length);
            return numArray;
        }

        private int ReadableBytes()
        {
            return this.writerIndex - this.readerIndex;
        }

        /// 扩展容量
        private void EnsureWritable(int dataSize)
        {
            byte[] numArray = new byte[this.writerIndex + dataSize];
            if (this.writerIndex > 0)
                Buffer.BlockCopy((Array)this.buffers, 0, (Array)numArray, 0, this.writerIndex);
            this.buffers = numArray;
        }

        public int GetWriterIndex()
        {
            return this.writerIndex;
        }

        /// <summary>结构体转byte[]</summary>
        /// <param name="structObj"></param>
        /// <returns></returns>
        public static byte[] StructToBytes<T>(T structObj) where T : struct
        {
            try
            {
                int length = Marshal.SizeOf((object)structObj);
                byte[] destination = new byte[length];
                IntPtr num = Marshal.AllocHGlobal(length);
                Marshal.StructureToPtr((object)structObj, num, false);
                Marshal.Copy(num, destination, 0, length);
                Marshal.FreeHGlobal(num);
                return destination;
            }
            catch (Exception ex)
            {
                Debug.LogError((object)("StructToBytes Error!!!" + (object)ex));
                return (byte[])null;
            }
        }

        public static T ByteToStruct<T>(byte[] bytes) where T : struct
        {
            try
            {
                int num1 = Marshal.SizeOf(typeof(T));
                if (num1 != bytes.Length)
                    return default(T);
                IntPtr num2 = Marshal.AllocHGlobal(num1);
                Marshal.Copy(bytes, 0, num2, num1);
                object structure = Marshal.PtrToStructure(num2, typeof(T));
                Marshal.FreeHGlobal(num2);
                return (T)structure;
            }
            catch (Exception ex)
            {
                Debug.LogError((object)("ByteToStruct Error!!!" + (object)ex));
                return default(T);
            }
        }
    }
}
