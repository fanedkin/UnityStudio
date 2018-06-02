using Net;
using System;
using UnityEngine;

namespace VRNetLibrary
{
    /// <summary>
    /// 消息包
    /// Vehicle
    /// 2017.4.24
    /// </summary>
    public struct MessagePackage
    {
        /// <summary>
        /// 消息主体数据
        /// </summary>
        private byte[] mDataBytes;

        /// <summary>
        /// 消息主体
        /// </summary>
        private object mData;

        /// <summary>
        /// 消息头
        /// </summary>
        public MessageHead Head
        {
            get;
            private set;
        }

        /// <summary>
        /// 消息主体数据
        /// </summary>
        public byte[] DataBytes
        {
            get
            {
                return this.mDataBytes;
            }
            set
            {
                if (this.mDataBytes != value)
                {
                    this.mDataBytes = value;
                    if (this.mDataBytes != null)
                    {
                        this.SetContentLength(this.mDataBytes.Length);
                    }
                }
            }
        }

        /// <summary>
        /// 消息数据
        /// </summary>
        public object Data
        {
            get
            {
                return this.mData;
            }
            set
            {
                this.mData = value;
                if (this.mData != null)
                {
                    this.Serialize();
                }
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        public MessagePackage(MessageHead head)
        {
            this = new MessagePackage(head, null);
        }

        /// <summary>
        /// 构造
        /// </summary>
        public MessagePackage(MessageHead head, byte[] dataBytes)
        {

            this.Head = head;
            this.mDataBytes = dataBytes;
            this.mData = null;
        }

        public void SetOpCode(byte id)
        {
            this.SetOpCode((NetOpCode)id);
        }

        public void SetOpCode(NetOpCode id)
        {
            MessageHead head = this.Head;
            head.OpCode = id;
            this.Head = head;
        }

        public void SetMsgID(uint id)
        {
            MessageHead head = this.Head;
            head.MsgID = id;
            this.Head = head;
        }

        public void SetProtocolID(int id)
        {
            MessageHead head = this.Head;
            head.ProtocolID = id;
            this.Head = head;
        }

        public void SetSessionID(int id)
        {
            MessageHead head = this.Head;
            head.SessionID = id;
            this.Head = head;
        }

        public void SetContentLength(int length)
        {
            MessageHead head = this.Head;
            head.ContentLength = length;
            this.Head = head;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        private void Serialize()
        {
            if (this.mData == null)
            {
                Debug.LogError("ProtocolID:" + this.Head.ProtocolID + " 消息体为空，请检查！！！");
            }
            this.DataBytes = Singleton<Protocol>.Instance.Serialize(this.mData);
            if (this.DataBytes == null)
            {
                Debug.LogError("ProtocolID:" + this.Head.ProtocolID + " 序列化失败，请检查！！！");
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public T DeSerialize<T>() where T : class
        {
            if (this.Head.ContentLength == 0)
            {
                return null;
            }
            if (this.DataBytes != null && this.DataBytes.Length != 0 && this.DataBytes.Length == this.Head.ContentLength)
            {
                return Singleton<Protocol>.Instance.DeSerialize<T>(this.DataBytes);
            }
            Debug.LogError("ProtocolID:" + this.Head.ProtocolID + " DeSerializeProtocol stream is null or contentLength error!!!!");
            return null;
        }

        /// <summary>
        /// 转为字节
        /// </summary>
        /// <returns></returns>
        public unsafe MessageBytes ToBytes()
        {
            byte[] array = new byte[17 + this.Head.ContentLength];
            byte[] array2 = array;
            fixed (byte* ptr = array2)
            {
                //byte* ptr = (byte*)(long)((array2 != null && array3.Length != 0) ? ((IntPtr)(&array3[0])) : ((IntPtr)(void*)null));
                int num = 0;
                ptr[num] = (byte)this.Head.OpCode;
                num++;
                *(int*)(ptr + num) = (int)Converter.GetLittleEndian(this.Head.MsgID);
                num += 4;
                *(int*)(ptr + num) = Converter.GetLittleEndian(this.Head.ProtocolID);
                num += 4;
                *(int*)(ptr + num) = Converter.GetLittleEndian(this.Head.SessionID);
                num += 4;
                *(int*)(ptr + num) = Converter.GetLittleEndian(this.Head.ContentLength);
                if (this.Head.ContentLength != 0)
                {
                    Buffer.BlockCopy(this.DataBytes, 0, array, 17, this.Head.ContentLength);
                }
            }
            MessageBytes result = default(MessageBytes);
            result.Bytes = array;
            result.Offset = 0;
            result.Length = array.Length;
            return result;
        }

        /// <summary>
        /// 转为消息包
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public unsafe static MessagePackage ToPackage(byte[] bytes)
        {
            if (bytes != null && bytes.Length >= 17)
            {
                MessagePackage emptyPackage = MessagePackage.GetEmptyPackage();
                fixed (byte* ptr = bytes)
                {
                    //byte* ptr = (byte*)(long)((bytes != null && array.Length != 0) ? ((IntPtr)(&array[0])) : ((IntPtr)(void*)null));
                    int num = 0;
                    emptyPackage.SetOpCode(ptr[num]);
                    num++;
                    emptyPackage.SetMsgID(Converter.GetLittleEndian(*(uint*)(ptr + num)));
                    num += 4;
                    emptyPackage.SetProtocolID(Converter.GetLittleEndian(*(int*)(ptr + num)));
                    num += 4;
                    emptyPackage.SetSessionID(Converter.GetLittleEndian(*(int*)(ptr + num)));
                    num += 4;
                    emptyPackage.SetContentLength(Converter.GetLittleEndian(*(int*)(ptr + num)));
                    if (emptyPackage.Head.ContentLength != 0)
                    {
                        emptyPackage.DataBytes = new byte[emptyPackage.Head.ContentLength];
                        Buffer.BlockCopy(bytes, 17, emptyPackage.DataBytes, 0, emptyPackage.Head.ContentLength);
                    }
                }
                return emptyPackage;
            }
            return MessagePackage.GetEmptyPackage();
        }

        /// <summary>
        /// 获取空消息包
        /// </summary>
        /// <returns></returns>
        public static MessagePackage GetEmptyPackage()
        {
            return new MessagePackage(MessageHead.GetEmptyHead());
        }
    }
}
