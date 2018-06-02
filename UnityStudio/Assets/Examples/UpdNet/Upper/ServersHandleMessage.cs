using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRNetLibrary;
using FlatBuffers;
public class ServersHandleMessage : IMessageHandle
{
    FlatBufferBuilder mBuilder = new FlatBufferBuilder(1472);

    public FlatBufferBuilder GetBuilder()
    {
        mBuilder.Clear();
        return mBuilder;
    }

    public FlatBuffers.ByteBuffer GetMessagePackageByteBuffer(MessagePackage package)
    {
        return package.DeSerialize<FlatBuffers.ByteBuffer>();
    }
    void IMessageHandle.Process(MessagePackage package)
    {
        NetMessageType.CmdCode type = (NetMessageType.CmdCode)package.Head.ProtocolID;
        Conn conn = HostServer.Instance.GetConnBySessionID(package.Head.SessionID);
        switch (type)
        {
            case NetMessageType.CmdCode.ConnectToHost: //连接主机
                ProcessConnectToHost(conn,package);
                break;
            case NetMessageType.CmdCode.DisconnectToHost: //断开与主机的连接
                ProcessDisconnectToHost(package);
                break;
            case NetMessageType.CmdCode.JoinInRoom: //加入主机
                ProcessJoinInRoom(package);
                break;
        }
    }
    private void ProcessConnectToHost(Conn conn,MessagePackage package)
    {

    }
    private void ProcessJoinInRoom(MessagePackage package)
    {

    }
    private void ProcessDisconnectToHost(MessagePackage package)
    {
        //int sessionId = package.Head.SessionID;

        //var playerData = HostServer.Instance.GetPlayerDataBySessionId(sessionId, false);

        //if (playerData != null)  //玩家已经存在，不处理。
        //{
        //    //Debug.Log("ProcessConnectToHost() ERROR, player unit is exist, id =" + playerId);
        //    return;
        //}

        //var reqData = C2SConnectToHost.GetRootAsC2SConnectToHost(GetMessagePackageByteBuffer(package));

        //SettingManager.PlayerInfo info;
        //HostServer.Instance.GetPlayerInfoBySID(sessionId, out info);  //根据Session查询信息

        //int playerId = HostServer.Instance.SpawnPlayerID(sessionId);
        //bool joinstatus = HostServer.Instance.GetCanjoinGame(); //获取是否可以加入游戏            

        //HostServer.Instance.RegisterPlayerSession(sessionId, playerId, reqData.MachineName, reqData.MachineId, joinstatus, info.playerGroup, info.playeType, info.playerWeapon); //注册Session信息

        //if (HostServer.Instance.IsCanFilterLocalServer(sessionId))
        //    return;
    }
}
