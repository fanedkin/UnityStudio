// automatically generated by the FlatBuffers compiler, do not modify

namespace Flat
{

using System;
using FlatBuffers;

public struct S2CLobbyOperationNotify : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static S2CLobbyOperationNotify GetRootAsS2CLobbyOperationNotify(ByteBuffer _bb) { return GetRootAsS2CLobbyOperationNotify(_bb, new S2CLobbyOperationNotify()); }
  public static S2CLobbyOperationNotify GetRootAsS2CLobbyOperationNotify(ByteBuffer _bb, S2CLobbyOperationNotify obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public S2CLobbyOperationNotify __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int OperationType { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int OperationPlayerId { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int OperationValue { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }

  public static Offset<S2CLobbyOperationNotify> CreateS2CLobbyOperationNotify(FlatBufferBuilder builder,
      int OperationType = 0,
      int OperationPlayerId = 0,
      int OperationValue = 0) {
    builder.StartObject(3);
    S2CLobbyOperationNotify.AddOperationValue(builder, OperationValue);
    S2CLobbyOperationNotify.AddOperationPlayerId(builder, OperationPlayerId);
    S2CLobbyOperationNotify.AddOperationType(builder, OperationType);
    return S2CLobbyOperationNotify.EndS2CLobbyOperationNotify(builder);
  }

  public static void StartS2CLobbyOperationNotify(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddOperationType(FlatBufferBuilder builder, int OperationType) { builder.AddInt(0, OperationType, 0); }
  public static void AddOperationPlayerId(FlatBufferBuilder builder, int OperationPlayerId) { builder.AddInt(1, OperationPlayerId, 0); }
  public static void AddOperationValue(FlatBufferBuilder builder, int OperationValue) { builder.AddInt(2, OperationValue, 0); }
  public static Offset<S2CLobbyOperationNotify> EndS2CLobbyOperationNotify(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<S2CLobbyOperationNotify>(o);
  }
};


}
