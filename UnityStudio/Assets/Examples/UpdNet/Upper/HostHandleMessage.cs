//using Flat;
using FlatBuffers;
using System.Collections.Generic;
using UnityEngine;
using VRNetLibrary;

/// <summary>
/// 主机处理信息
/// </summary>
public class HostHandleMessage : IMessageHandle
{
    #region 生命周期
    /// <summary>
    /// 消息处理
    /// </summary>
    /// <param name="package"></param>
    public void Process(MessagePackage package)
    {
        if (package.Head.ContentLength == 0)
            return;

        NetMessageType.CmdCode type = (NetMessageType.CmdCode)package.Head.ProtocolID;

        switch (type)
        {
            case NetMessageType.CmdCode.ConnectToHost: //连接主机
                //ProcessConnectToHost(package);
                break;
            //case NetMessageType.CmdCode.DisconnectToHost: //断开与主机的连接
            //    //ProcessDisConnectToHost(package);
            //    break;
            //case NetMessageType.CmdCode.SyncPlayerGlobalInfo://玩家信息同步
            //    //ProcessSyncPlayerGlobalInfo(package);
            //    break;
            //case NetMessageType.CmdCode.FireBulletCreate:   //子弹创建
            //    //ProcessFireBulletCreate(package);
            //    break;
            //case NetMessageType.CmdCode.HitDamage:          //击中伤害消息
            //    //ProcessHitDamage(package);
            //    break;
            //case NetMessageType.CmdCode.PlayerRevive:          //玩家复活消息
            //    //ProcessPlayerRevive(package);
            //    break;
            default:
                {
                    Debug.LogError("LocalServer还没处理的消息(" + package.Head.ProtocolID + ");");
                }
                break;
        }
    }
    #endregion

    #region 消息发送
    /// <summary>
    /// 发送回应消息给指定Client
    /// </summary>
    /// <param name="cmdType">消息体类型</param>
    /// <param name="sessionId">客户端会话id</param>
    /// <param name="data">消息体内容</param>
    /// <param name="isSync">是否同步发送</param>
    public void SendTo(NetMessageType.CmdCode cmdType, int sessionId, object data, bool isSync = false)
    {
        ServersManager.Instance.SendTo((int)cmdType, sessionId, data, isSync);
    }

    /// <summary>
    /// 固定数组，减少GC。
    /// </summary>
    private int[] cullSessions = new int[1];

    /// <summary>
    /// 广播消息
    /// </summary>
    /// <param name="cmdType">消息体类型</param>
    /// <param name="data">消息体内容</param>
    /// <param name="cullSessionId">过滤的会话id</param>
    /// <param name="isSync">是否同步</param>
    public void Broadcast(NetMessageType.CmdCode cmdType, object data, int cullSessionId = 0, bool isSync = false)
    {
        if (cullSessionId != 0)
        {
            cullSessions[0] = cullSessionId;
            ServersManager.Instance.Broadcast((int)cmdType, data, HostServer.Instance.sessionArray, cullSessions, isSync);
        }
        else
        {
            ServersManager.Instance.Broadcast((int)cmdType, data, HostServer.Instance.sessionArray, null, isSync);
        }
    }

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
    #endregion

    ///// <summary>
    ///// 协议处理： 玩家连接主机
    ///// </summary>
    ///// <param name="package"></param>
    //private void ProcessConnectToHost(MessagePackage package)
    //{
    //    int sessionId = package.Head.SessionID;

    //    var playerData = HostServer.Instance.GetPlayerDataBySessionId(sessionId, false);

    //    if (playerData != null)  //玩家已经存在，不处理。
    //    {
    //        //Debug.Log("ProcessConnectToHost() ERROR, player unit is exist, id =" + playerId);
    //        return;
    //    }

    //    var reqData = C2SConnectToHost.GetRootAsC2SConnectToHost(GetMessagePackageByteBuffer(package));

    //    SettingManager.PlayerInfo info;
    //    HostServer.Instance.GetPlayerInfoBySID(sessionId, out info);  //根据Session查询信息

    //    int playerId = HostServer.Instance.SpawnPlayerID(sessionId);
    //    bool joinstatus = HostServer.Instance.GetCanjoinGame(); //获取是否可以加入游戏            

    //    HostServer.Instance.RegisterPlayerSession(sessionId, playerId, reqData.MachineName, reqData.MachineId, joinstatus, info.playerGroup, info.playeType, info.playerWeapon); //注册Session信息

    //    if (HostServer.Instance.IsCanFilterLocalServer(sessionId))
    //        return;

    //    //Response
    //    //1) 下发连接结果协议
    //    var tempbuilder = GetBuilder();
    //    List<Offset<ConnectPlayer>> temps = new List<Offset<ConnectPlayer>>();
    //    foreach (var pair in HostServer.Instance.GetPlayerDatas(SettingManager.Instance.ServerType == e_Server_Type.Server ? true : false))
    //    {
    //        int cur_playerid = pair.PlayerId;

    //        var playerdata = HostServer.Instance.GetPlayerDataById(cur_playerid);

    //        var mechineName = tempbuilder.CreateString(playerdata.MechineName);
    //        var mechineId = tempbuilder.CreateString(playerdata.MechineId);

    //        //====================================================================//
    //        //请求者本身
    //        if (cur_playerid == playerId)
    //        {
    //            temps.Add(ConnectPlayer.CreateConnectPlayer(tempbuilder, playerId, mechineName, mechineId, (int)info.playerWeapon, (int)info.playeType, (int)info.playerGroup, joinstatus));
    //            continue;
    //        }
    //        //====================================================================//
    //        //相对于请求者的其他玩家
    //        WUnit unit = GameWorld.GlobalBattle.GetUnitById(cur_playerid);
    //        if (unit != null)
    //        {
    //            DataAgent dataAgent = unit.dataAgent;
    //            temps.Add(ConnectPlayer.CreateConnectPlayer(tempbuilder, cur_playerid, mechineName, mechineId, (int)(unit.weaponAgent.weaponType), (int)(unit.dataAgent.playerStyle), (int)unit.GroupId, playerdata.JoinGameStatus));
    //        }
    //        else
    //        {
    //            SettingManager.PlayerInfo playerInfoStruct;
    //            HostServer.Instance.GetPlayerInfoByID(cur_playerid, out playerInfoStruct);
    //            temps.Add(ConnectPlayer.CreateConnectPlayer(tempbuilder, cur_playerid, mechineName, mechineId, (int)(playerInfoStruct.playerWeapon), (int)(playerInfoStruct.playeType), (int)playerInfoStruct.playerGroup, playerdata.JoinGameStatus));
    //        }
    //    }

    //    //2) 返回请求客户端，玩家进入
    //    var playerListOffset = S2CConnectToHost.CreatePlayerListVector(tempbuilder, temps.ToArray());
    //    S2CConnectToHost.StartS2CConnectToHost(tempbuilder);
    //    S2CConnectToHost.AddIssucceed(tempbuilder, false);
    //    S2CConnectToHost.AddPlayerId(tempbuilder, playerId);
    //    S2CConnectToHost.AddPlayerList(tempbuilder, playerListOffset);
    //    tempbuilder.Finish(S2CConnectToHost.EndS2CConnectToHost(tempbuilder).Value);
    //    SendTo(NetMessageType.CmdCode.ConnectToHost, package.Head.SessionID, tempbuilder);

    //    //3) 推送给其它客户端，玩家进入
    //    tempbuilder = GetBuilder();
    //    var tempData = HostServer.Instance.GetPlayerDataById(playerId);
    //    var playerOffset = ConnectPlayer.CreateConnectPlayer(tempbuilder, playerId, tempbuilder.CreateString(tempData.MechineName), tempbuilder.CreateString(tempData.MechineId), (int)info.playerWeapon, (int)info.playeType, (int)info.playerGroup, joinstatus);
    //    S2CPlayerEnterNotify.StartS2CPlayerEnterNotify(tempbuilder);
    //    S2CPlayerEnterNotify.AddPlayer(tempbuilder, playerOffset);
    //    tempbuilder.Finish(S2CPlayerEnterNotify.EndS2CPlayerEnterNotify(tempbuilder).Value);
    //    Broadcast(NetMessageType.CmdCode.PlayerEnterNotify, tempbuilder, sessionId);

    //    if (GameManager.Instance.ServerConsoleUI.IsShow())
    //        GameManager.Instance.ServerConsoleUI.Refresh();
    //}

    ///// <summary>
    ///// 协议处理： 玩家断开与主机的连接
    ///// </summary>
    ///// <param name="package"></param>
    //private void ProcessDisConnectToHost(MessagePackage package)
    //{
    //    int sessionId = package.Head.SessionID;

    //    if (HostServer.Instance.IsCanFilterLocalServer(sessionId))
    //        return;

    //    var playerData = HostServer.Instance.GetPlayerDataBySessionId(sessionId, false);

    //    if (playerData == null)
    //        return;

    //    HostServer.Instance.UnRegisterPlayerSession(sessionId);

    //    //Response
    //    //1) 下发结果协议
    //    var tempbuilder = GetBuilder();
    //    tempbuilder.Finish(S2CDisConnectToHost.CreateS2CDisConnectToHost(tempbuilder, true).Value);
    //    SendTo(NetMessageType.CmdCode.DisconnectToHost, sessionId, tempbuilder);

    //    //2) 推送给其它客户端，玩家离开
    //    tempbuilder.Clear();
    //    tempbuilder.Finish(S2CPlayerQuitNotify.CreateS2CPlayerQuitNotify(tempbuilder, playerData.PlayerId).Value);
    //    Broadcast(NetMessageType.CmdCode.PlayerQuitNotify, tempbuilder, sessionId);

    //    if (GameManager.Instance.ServerConsoleUI.IsShow())
    //        GameManager.Instance.ServerConsoleUI.Refresh();
    //}

    ///// <summary>
    ///// 协议处理： 玩家信息同步
    ///// </summary>
    ///// <param name="package"></param>
    //private void ProcessSyncPlayerGlobalInfo(MessagePackage package)
    //{
    //    int sessionId = package.Head.SessionID;

    //    var playerData = HostServer.Instance.GetPlayerDataBySessionId(sessionId, false);

    //    if (playerData == null)
    //        return;

    //    var data = C2SSyncPlayerGlobalInfo.GetRootAsC2SSyncPlayerGlobalInfo(GetMessagePackageByteBuffer(package));

    //    var broadcastData = GetBuilder();
    //    S2CSyncPlayerGlobalInfo.StartS2CSyncPlayerGlobalInfo(broadcastData);
    //    S2CSyncPlayerGlobalInfo.AddObjId(broadcastData, playerData.PlayerId);

    //    if (data.PlayerPos.HasValue)
    //    {
    //        S2CSyncPlayerGlobalInfo.AddPlayerPos(broadcastData, UtilityTool.ConvertVector3ToFlat(broadcastData, data.PlayerPos.Value));
    //        S2CSyncPlayerGlobalInfo.AddPlayerRotate(broadcastData, UtilityTool.ConvertQuaternionToFlat(broadcastData, data.PlayerRotate.Value));
    //    }

    //    if (data.GunPos.HasValue)
    //    {
    //        S2CSyncPlayerGlobalInfo.AddGunPos(broadcastData, UtilityTool.ConvertVector3ToFlat(broadcastData, data.GunPos.Value));
    //        S2CSyncPlayerGlobalInfo.AddGunRotate(broadcastData, UtilityTool.ConvertQuaternionToFlat(broadcastData, data.GunRotate.Value));
    //        S2CSyncPlayerGlobalInfo.AddGunForward(broadcastData, UtilityTool.ConvertVector3ToFlat(broadcastData, data.GunForward.Value));
    //    }

    //    if (data.VRHeadPos.HasValue)
    //    {
    //        S2CSyncPlayerGlobalInfo.AddVRHeadPos(broadcastData, UtilityTool.ConvertVector3ToFlat(broadcastData, data.VRHeadPos.Value));
    //        S2CSyncPlayerGlobalInfo.AddVRHeadRotate(broadcastData, UtilityTool.ConvertQuaternionToFlat(broadcastData, data.VRHeadRotate.Value));
    //    }

    //    broadcastData.Finish(S2CSyncPlayerGlobalInfo.EndS2CSyncPlayerGlobalInfo(broadcastData).Value);
    //    Broadcast(NetMessageType.CmdCode.SyncPlayerGlobalInfo, broadcastData, sessionId);
    //}

    ///// <summary>
    ///// 协议处理： 子弹创建
    ///// </summary>
    ///// <param name="package"></param>
    //private void ProcessFireBulletCreate(MessagePackage package)
    //{
    //    var data = C2SFireBulletCreate.GetRootAsC2SFireBulletCreate(GetMessagePackageByteBuffer(package));

    //    int objId = data.ObjId;

    //    var unit = GameWorld.GlobalBattle.GetUnitById(objId);
    //    if (unit == null)
    //        return;

    //    var tempbuilder = GetBuilder();
    //    S2CFireBulletCreate.StartS2CFireBulletCreate(tempbuilder);
    //    S2CFireBulletCreate.AddObjId(tempbuilder, objId);
    //    S2CFireBulletCreate.AddPos(tempbuilder, UtilityTool.ConvertVector3ToFlat(tempbuilder, data.Pos.Value));
    //    S2CFireBulletCreate.AddRotate(tempbuilder, UtilityTool.ConvertQuaternionToFlat(tempbuilder, data.Rotate.Value));
    //    tempbuilder.Finish(S2CFireBulletCreate.EndS2CFireBulletCreate(tempbuilder).Value);

    //    Broadcast(NetMessageType.CmdCode.FireBulletCreate, tempbuilder, package.Head.SessionID);
    //    //Debug.LogWarning("~~~~~~~~~~~~~~~~~~~~Servers ProcessFireBulletCreate Id=" + playerId +",MsgId=" + package.Head.MsgID); //测试输出

    //    ////记录射击次数
    //    //if(unit.roleType == e_type_role.HERO)
    //    //    StatisticalManager.Instance.RecordPlayerAttack(objId);
    //}

    ///// <summary>
    ///// 协议处理：客户端击中伤害信息
    ///// </summary>
    ///// <param name="package"></param>
    //private void ProcessHitDamage(MessagePackage package)
    //{
    //    int sessionId = package.Head.SessionID;

    //    var playerData = HostServer.Instance.GetPlayerDataBySessionId(sessionId, false);

    //    if (playerData == null)
    //        return;

    //    int playerId = playerData.PlayerId;

    //    if (GameWorld.GlobalBattle.GetUnitById(playerId) == null)
    //        return;

    //    var data = C2SHitDamage.GetRootAsC2SHitDamage(GetMessagePackageByteBuffer(package));

    //    var hitResult = HitResultData.CreateHitResultData(data);

    //    GameHitManager.HandleC2SHitDamageNotify(hitResult);
    //}

    ///// <summary>
    ///// 协议处理：客户端请求复活消息
    ///// </summary>
    ///// <param name="package"></param>
    //private void ProcessPlayerRevive(MessagePackage package)
    //{
    //    var data = C2SPlayerRevive.GetRootAsC2SPlayerRevive(GetMessagePackageByteBuffer(package));

    //    var id = data.PlayerObjId;

    //    var unit = GameWorld.GlobalBattle.GetUnitById(id);

    //    if (unit == null)
    //        return;

    //    if (unit.dataAgent.CurHP > 0)
    //        return;

    //    HostSendMessage.BroadcastPlayerReviveNotify(id);
    //}
}