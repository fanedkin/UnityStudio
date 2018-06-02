using System;
using System.Collections;
using System.Collections.Generic;
using VRNetLibrary;
using FlatBuffers;
//using Flat;

/// <summary>
/// Flatbuffers序列化类
/// </summary>
public class FlatbuffersUtils : IProtocolUtils
{
    FlatBuffers.ByteBuffer byteBuffer = new FlatBuffers.ByteBuffer();

    public FlatBuffers.ByteBuffer ByteBuffer
    {
        get
        {
            return byteBuffer;
        }
    }

    /// <summary>
    /// Flatbuffers序列化
    /// </summary>
    /// <param name="protoData"></param>
    /// <returns></returns>
    public byte[] Serialize(object protoData)
    {
        var builder = protoData as FlatBufferBuilder;
        byte[] temp = null;
        if (builder != null)
        {
            var offset = builder.DataBuffer.Position;
            var length = builder.Offset;
            temp = new byte[length];
            Buffer.BlockCopy(builder.DataBuffer.Data, offset, temp, 0, length);
        }
        return temp;
    }

    /// <summary>
    /// Flatbuffers反序列化
    /// </summary>
    /// <returns></returns>
    public T DeSerialize<T>(byte[] bytes) /*where T : class*/
    {
        byteBuffer.Reset(bytes, 0);
        return (T)(object)byteBuffer;
    }
}