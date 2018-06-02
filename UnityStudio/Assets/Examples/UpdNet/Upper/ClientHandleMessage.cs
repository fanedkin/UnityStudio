//using Flat;
using System.Collections.Generic;
using UnityEngine;
using VRNetLibrary;

/// <summary>
/// 客户端消息句柄
/// </summary>
public class ClientHandleMessage : IMessageHandle
{
    #region 生命周期
    /// <summary>
    /// 消息监听回调字典
    /// </summary>
    private Dictionary<int, dReceiveMessageHandle> mProtocalDic = new Dictionary<int, dReceiveMessageHandle>();

    public ClientHandleMessage()
    {
        //this.mProtocalDic[(int)NetMessageType.CmdCode.ConnectToHost] = HandleConnectToHost;
        //this.mProtocalDic[(int)NetMessageType.CmdCode.PlayerEnterNotify] = HandlePlayerEnterNotify;
        //this.mProtocalDic[(int)NetMessageType.CmdCode.DisconnectToHost] = HandleDisConnectToHost;
        //this.mProtocalDic[(int)NetMessageType.CmdCode.PlayerQuitNotify] = HandlePlayerQuitNotify;
        //this.mProtocalDic[(int)NetMessageType.CmdCode.PlayerRevive] = HandlePlayerReviveNotify;

        //this.mProtocalDic[(int)NetMessageType.CmdCode.SyncPlayerGlobalInfo] = HandleSyncPlayerGlobalInfo;

        //this.mProtocalDic[(int)NetMessageType.CmdCode.FireBulletCreate] = HandleFireBulletCreate;
        //this.mProtocalDic[(int)NetMessageType.CmdCode.ReloadMagazine] = HandleReloadMagazineNotify;

        //this.mProtocalDic[(int)NetMessageType.CmdCode.MonsterCreateNotify] = HandleMonsterCreateNotify;
        //this.mProtocalDic[(int)NetMessageType.CmdCode.MonsterAnimationSyncNotify] = HandleMonsterAnimationSyncNotify;
        //this.mProtocalDic[(int)NetMessageType.CmdCode.UnitInfoSyncNotify] = HandleUnitInfoSyncNotify;
        //this.mProtocalDic[(int)NetMessageType.CmdCode.MonsterBulletCretateNotify] = HandleMonsterBulletCretateNotify;

        //this.mProtocalDic[(int)NetMessageType.CmdCode.HitDamage] = HandleHitDamage;

        //this.mProtocalDic[(int)NetMessageType.CmdCode.CutScenesNotify] = HandleCutScenesNotify;

        //this.mProtocalDic[(int)NetMessageType.CmdCode.BattleStatisticalResultsNotify] = HandleBattleStatisticalResultsNotify;

        //this.mProtocalDic[(int)NetMessageType.CmdCode.PlayAmbientEffectNotify] = HandlePlayAmbientEffectNotify;

        //this.mProtocalDic[(int)NetMessageType.CmdCode.NpcCreateNotify] = HandleNpcCreateNotify;

        //this.mProtocalDic[(int)NetMessageType.CmdCode.BattleStateSwitchNotify] = HandleBattleStateSwitchNotify;
        //this.mProtocalDic[(int)NetMessageType.CmdCode.GameLogicSwitchNotify] = HandleGameLogicSwitchNotify;
        //this.mProtocalDic[(int)NetMessageType.CmdCode.GameScoreNotify] = HandleGameScoreNotify;

        ////===========================大厅相关=========================================
        //this.mProtocalDic[(int)NetMessageType.CmdCode.LobbyOperationNotify] = HandleLobbyOperationNotify;
    }

    /// <summary>
    /// 消息处理
    /// </summary>
    /// <param name="package"></param>
    public void Process(MessagePackage package)
    {
        dReceiveMessageHandle handle = null;
        if (mProtocalDic.TryGetValue(package.Head.ProtocolID, out handle))
        {
            handle(package);
        }
        else
        {
            Debug.LogWarning(" ######### ClientHandleMessage.Process 收到未处理的消息: ProtocolID = " + package.Head.ProtocolID);
        }
    }

    public FlatBuffers.ByteBuffer GetMessagePackageByteBuffer(MessagePackage package)
    {
        return package.DeSerialize<FlatBuffers.ByteBuffer>();
    }
    #endregion

    //#region 玩家相关
    ///// <summary>
    ///// 协议处理：玩家连接主机
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleConnectToHost(MessagePackage package)
    //{
    //    var data = S2CConnectToHost.GetRootAsS2CConnectToHost(GetMessagePackageByteBuffer(package));

    //    //标记连接主机成功
    //    GameWorld.IsLogin = true;

    //    int myPlayerId = data.PlayerId;

    //    if (data.PlayerListLength != 0)
    //    {
    //        for (int i = 0; i < data.PlayerListLength; i++)
    //        {
    //            var cell = data.PlayerList(i);

    //            if (!cell.HasValue)
    //                continue;

    //            var cellData = cell.Value;

    //            //创建玩家
    //            WUnit player = GameWorld.GlobalBattle.GetUnitById(cellData.Id);

    //            if (player != null)
    //                GameWorld.GlobalBattle.RemoveUnit(player);

    //            //注册玩家数据
    //            if (ClientServer.Instance.CheckPlayerData(cellData.Id))
    //                ClientServer.Instance.UnRegisterPlayerData(cellData.Id);
    //            ClientServer.Instance.RegisterPlayerData(
    //                package.Head.SessionID,
    //                cellData.Id,
    //                cellData.MechineName,
    //                cellData.Mechineid,
    //                cellData.JoinState,
    //                (e_group_type)cellData.GroupId,
    //                (e_Player_Style)cellData.PlayerStyle,
    //                (e_weapon_type)cellData.WeaponType);

    //            if (cellData.JoinState)
    //            {
    //                //自己
    //                if (cellData.Id == myPlayerId)
    //                {
    //                    player = UnitManager.Instance.CreateLocalPlayerUnit(cellData.Id, (e_group_type)cellData.GroupId, (e_Player_Style)cellData.PlayerStyle, (e_weapon_type)cellData.WeaponType);
    //                    GameWorld.GlobalBattle.ControlUnit = player;
    //                }
    //                else
    //                {
    //                    player = UnitManager.Instance.CreateOtherPlayer(cellData.Id, (e_group_type)cellData.GroupId, (e_Player_Style)cellData.PlayerStyle, (e_weapon_type)cellData.WeaponType);
    //                }

    //                //设置加入游戏状态
    //                player.JoinGame(cell.Value.JoinState);

    //                //加入战场管理
    //                GameWorld.GlobalBattle.AddUnit(player);
    //            }
    //        }
    //    }

    //    //连接进来，重置战场
    //    //GameWorld.BattleReady();

    //    Debug.Log("##收到消息：玩家连接到主机！当前PlayerList.size = " + data.PlayerListLength + ",myPlayerId=" + myPlayerId);
    //}

    ///// <summary>
    ///// 协议处理：其它玩家进入
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandlePlayerEnterNotify(MessagePackage package)
    //{
    //    var data = S2CPlayerEnterNotify.GetRootAsS2CPlayerEnterNotify(GetMessagePackageByteBuffer(package));

    //    var player = data.Player;

    //    if (!data.Player.HasValue)
    //        return;

    //    WUnit unit = GameWorld.GlobalBattle.GetUnitById(data.Player.Value.Id);
    //    if (unit != null)
    //    {
    //        Debug.LogError("HandlePlayerEnterNotify 为啥会有重复id的玩家进入？请检查。 id=" + data.Player.Value.Id);
    //        return;
    //    }

    //    //注册玩家数据
    //    if (ClientServer.Instance.CheckPlayerData(player.Value.Id))
    //        ClientServer.Instance.UnRegisterPlayerData(player.Value.Id);
    //    ClientServer.Instance.RegisterPlayerData(
    //        package.Head.SessionID,
    //        player.Value.Id,
    //        player.Value.MechineName,
    //        player.Value.Mechineid,
    //        player.Value.JoinState,
    //        (e_group_type)player.Value.GroupId,
    //        (e_Player_Style)player.Value.PlayerStyle,
    //        (e_weapon_type)player.Value.WeaponType);

    //    if (player.Value.JoinState)
    //    {
    //        WUnit player1 = UnitManager.Instance.CreateOtherPlayer(data.Player.Value.Id, (e_group_type)data.Player.Value.GroupId, (e_Player_Style)data.Player.Value.PlayerStyle, (e_weapon_type)data.Player.Value.WeaponType);
    //        if (player1 != null)
    //        {
    //            GameWorld.GlobalBattle.AddUnit(player1);
    //            player1.JoinGame(player.Value.JoinState);
    //            Debug.Log("##收到消息：其它玩家进入,id= " + player1.Id);
    //        }
    //    }

    //    //if (GameManager.Instance.ServerConsoleUI.IsShow())
    //    //    GameManager.Instance.ServerConsoleUI.Refresh();
    //}

    ///// <summary>
    ///// 协议处理：玩家断开与主机的连接
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleDisConnectToHost(MessagePackage package)
    //{
    //    //var data = S2CDisConnectToHost.GetRootAsS2CDisConnectToHost(GetMessagePackageByteBuffer(package));

    //    //标记断开连接主机成功
    //    GameWorld.IsLogin = false;

    //    ClientServer.Instance.UnRegisterAllPlayerData();

    //    Debug.Log("##收到消息：玩家断开与主机的连接！");
    //    //目前没有断线重传机制，所以采取断线清理场上战斗相关单元
    //    foreach (var cell in GameWorld.GlobalBattle.GetAllPlayers(true))
    //    {
    //        if (cell != GameWorld.GlobalBattle.ControlUnit)
    //            cell.Destroy();
    //    }
    //    //GameWorld.BattleReady();
    //}

    ///// <summary>
    ///// 协议处理：其它玩家离开
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandlePlayerQuitNotify(MessagePackage package)
    //{
    //    int objId = 0;

    //    var data1 = S2CPlayerQuitNotify.GetRootAsS2CPlayerQuitNotify(GetMessagePackageByteBuffer(package));
    //    objId = data1.DisConnectPlayerId;

    //    Debug.Log("##收到消息：其它玩家离开,id= " + objId);

    //    if (ClientServer.Instance.CheckPlayerData(objId))
    //        ClientServer.Instance.UnRegisterPlayerData(objId);

    //    WUnit unit = GameWorld.GlobalBattle.GetUnitById(objId);
    //    if (unit == null)
    //    {
    //        Debug.LogError("HandlePlayerQuitNotify ERROR, unit is null, objId=" + objId);
    //        return;
    //    }
    //    GameWorld.GlobalBattle.RemoveUnit(unit);

    //    //if (GameManager.Instance.ServerConsoleUI.IsShow())
    //    //    GameManager.Instance.ServerConsoleUI.Refresh();
    //}

    ///// <summary>
    ///// 协议处理：玩家信息同步
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleSyncPlayerGlobalInfo(MessagePackage package)
    //{
    //    var data = S2CSyncPlayerGlobalInfo.GetRootAsS2CSyncPlayerGlobalInfo(GetMessagePackageByteBuffer(package));
    //    WUnit unit = GameWorld.GlobalBattle.GetUnitById(data.ObjId);
    //    if (unit == null)
    //    {
    //        Debug.LogError("HandleSyncPlayerGlobalInfo ERROR, unit is null, objId=" + data.ObjId);
    //        return;
    //    }
    //    //Debug.Log("##收到消息：玩家信息同步,id= " + data.ObjId+ ",playerPos="+ UtilityTool.ConvertFlatToVector3(data.PlayerPos.Value));
    //    OtherPlayerNetSyncController syncController = UtilityTool.GetComponent<OtherPlayerNetSyncController>(unit.gameObject);
    //    if (syncController != null)
    //    {
    //        if (data.PlayerPos.HasValue)
    //        {
    //            syncController.SyncPlayer(UtilityTool.ConvertFlatToVector3(data.PlayerPos.Value), UtilityTool.ConvertFlatToQuaternion(data.PlayerRotate.Value));
    //        }

    //        if (data.GunPos.HasValue)
    //        {
    //            syncController.SyncGun(UtilityTool.ConvertFlatToVector3(data.GunPos.Value), UtilityTool.ConvertFlatToQuaternion(data.GunRotate.Value), UtilityTool.ConvertFlatToVector3(data.GunForward.Value));
    //        }

    //        if (data.VRHeadPos.HasValue)
    //        {
    //            syncController.SyncVRHead(UtilityTool.ConvertFlatToVector3(data.VRHeadPos.Value), UtilityTool.ConvertFlatToQuaternion(data.VRHeadRotate.Value));
    //        }
    //    }
    //}

    ///// <summary>
    ///// 协议处理 : 玩家复活
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandlePlayerReviveNotify(MessagePackage package)
    //{
    //    var data = S2CPlayerRevive.GetRootAsS2CPlayerRevive(GetMessagePackageByteBuffer(package));

    //    WUnit unit = GameWorld.GlobalBattle.GetUnitById(data.PlayerObjId);
    //    if (unit == null)
    //    {
    //        Debug.LogError("HandleMonsterInfoSyncNotify ERROR, unit is null, objId=" + data.PlayerObjId);
    //        return;
    //    }

    //    Debug.Log("收到 HandlePlayerReviveNotify 消息！！！，objid=" + unit.Id);

    //    //复活
    //    GameReviveManager.Instance.RevivePlayer(unit);
    //}
    //#endregion

    //#region 子弹相关
    ///// <summary>
    ///// 协议处理：子弹创建
    ///// </summary>
    //private void HandleFireBulletCreate(MessagePackage package)
    //{
    //    int id = 0;
    //    Vector3 pos = default(Vector3);
    //    Quaternion rot = default(Quaternion);

    //    var data1 = S2CFireBulletCreate.GetRootAsS2CFireBulletCreate(GetMessagePackageByteBuffer(package));
    //    id = data1.ObjId;
    //    pos = UtilityTool.ConvertFlatToVector3(data1.Pos.Value);
    //    rot = UtilityTool.ConvertFlatToQuaternion(data1.Rotate.Value);

    //    WUnit unit = GameWorld.GlobalBattle.GetUnitById(id);
    //    if (unit == null)
    //    {
    //        Debug.LogError("HandleFireBulletCreate ERROR, unit is null, objId=" + id);
    //        return;
    //    }

    //    //Debug.Log("FrameCounter = " + Time.frameCount + " 接收消息：HandleFireBulletCreate: pos=" + pos + ",rotate=" + rot + ",player=" + unit.Position);
    //    //Debug.LogWarning("~~~~~~~~~~~~~~~~~~~~Client HandleFireBulletCreate Id=" + id + ",MsgId=" + package.Head.MsgID); //测试输出
    //    unit.weaponAgent.SyncCreateBullte(pos, rot);
    //}

    ///// <summary>
    ///// 协议处理：重置弹药库
    ///// </summary>
    //private void HandleReloadMagazineNotify(MessagePackage package)
    //{
    //    var data = S2CReloadMagazineNotify.GetRootAsS2CReloadMagazineNotify(GetMessagePackageByteBuffer(package));
    //    int id = data.PlayerObjId;

    //    WUnit unit = GameWorld.GlobalBattle.GetUnitById(id);
    //    if (unit == null)
    //    {
    //        Debug.LogError("HandleFireBulletCreate ERROR, unit is null, objId=" + id);
    //        return;
    //    }

    //    unit.weaponAgent.SyncReloadMagazine();
    //}
    //#endregion

    //#region NPC相关
    ///// <summary>
    ///// 协议处理 : npc创建
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleNpcCreateNotify(MessagePackage package)
    //{
    //    var data = S2CNpcCreateNotify.GetRootAsS2CNpcCreateNotify(GetMessagePackageByteBuffer(package));
    //    if (data.NpcListLength == 0)
    //        return;
    //    for (int i = 0; i < data.NpcListLength; i++)
    //    {
    //        var cellData = data.NpcList(i);
    //        WUnit unit = GameWorld.GlobalBattle.GetUnitById(cellData.Value.ObjId);
    //        if (unit != null)
    //        {
    //            Debug.LogError("HandleMonsterCreateNotify ERROR, unit is existing, objId=" + unit.Id);
    //            continue;
    //        }
    //        NPCSpawnManager.Instance.SpawnNPC(
    //            cellData.Value.ObjId,
    //            cellData.Value.NpcId,
    //            cellData.Value.SpawnPointId,
    //            cellData.Value.GroupId);
    //    }
    //}
    //#endregion

    //#region 怪物相关
    ///// <summary>
    ///// 协议处理 : 怪物创建
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleMonsterCreateNotify(MessagePackage package)
    //{
    //    var data = S2CMonsterCreateNotify.GetRootAsS2CMonsterCreateNotify(GetMessagePackageByteBuffer(package));
    //    if (data.MonsterListLength == 0)
    //        return;
    //    for (int i = 0; i < data.MonsterListLength; i++)
    //    {
    //        var cellData = data.MonsterList(i);
    //        WUnit unit = GameWorld.GlobalBattle.GetUnitById(cellData.Value.Id);
    //        if (unit != null)
    //        {
    //            Debug.LogError("HandleMonsterCreateNotify ERROR, unit is existing, objId=" + unit.Id);
    //            continue;
    //        }
    //        if (cellData.Value.SpawnPoint == 0)
    //        {
    //            MonsterSpawnManager.Instance.SpawnMonster(
    //                cellData.Value.Id,
    //                cellData.Value.DatabaseId,
    //                UtilityTool.ConvertFlatToVector3(cellData.Value.SpawnPos.Value),
    //                UtilityTool.ConvertFlatToQuaternion(cellData.Value.SpawnRot.Value));
    //        }
    //        else
    //        {
    //            MonsterSpawnManager.Instance.SpawnMonster(cellData.Value.Id, cellData.Value.DatabaseId, cellData.Value.SpawnPoint);
    //        }

    //        MonsterSpawnManager.Instance.ClientTotalCount++;
    //    }
    //}

    ///// <summary>
    ///// 协议处理 : 怪物动画同步
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleMonsterAnimationSyncNotify(MessagePackage package)
    //{
    //    int objId = 0;
    //    string stateName = "";
    //    float transitionDuration = 0;
    //    int layerIndex = 0;

    //    var data = S2CMonsterAnimationSyncNotify.GetRootAsS2CMonsterAnimationSyncNotify(GetMessagePackageByteBuffer(package));
    //    objId = data.ObjId;
    //    stateName = data.StateName;
    //    transitionDuration = data.TransitionDuration;
    //    layerIndex = data.LayerIndex;

    //    WUnit unit = GameWorld.GlobalBattle.GetUnitById(objId);
    //    if (unit == null)
    //    {
    //        Debug.LogError("HandleMonsterInfoSyncNotify ERROR, unit is null, objId=" + objId);
    //        return;
    //    }

    //    OtherMonsterNetSyncController syncController = UtilityTool.GetComponent<OtherMonsterNetSyncController>(unit.gameObject);
    //    if (syncController == null)
    //    {
    //        Debug.LogError("HandleMonsterAnimationSyncNotify ERROR, OtherMonsterNetSyncController is null, objId=" + objId);
    //        return;
    //    }

    //    if (syncController.enabled)
    //        syncController.SyncAnimator(stateName, transitionDuration, layerIndex);
    //    //Debug.Log("收到 MonsterInfoSyncNotify 消息！！！");
    //}

    ///// <summary>
    ///// 协议处理 : 单元位置同步
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleUnitInfoSyncNotify(MessagePackage package)
    //{
    //    var data = S2CUnitInfoSyncNotify.GetRootAsS2CUnitInfoSyncNotify(GetMessagePackageByteBuffer(package));

    //    if (data.UnitInfoListLength == 0)
    //        return;

    //    for (int i = 0; i < data.UnitInfoListLength; i++)
    //    {
    //        var cellData = data.UnitInfoList(i);

    //        if (!cellData.HasValue)
    //            continue;

    //        WUnit unit = GameWorld.GlobalBattle.GetUnitById(cellData.Value.ObjId);
    //        if (unit == null)
    //        {
    //            Debug.LogError("HandleMonsterInfoSyncNotify ERROR, unit is null, objId=" + cellData.Value.ObjId);
    //            return;
    //        }

    //        OtherMonsterNetSyncController syncController = UtilityTool.GetComponent<OtherMonsterNetSyncController>(unit.gameObject);
    //        if (syncController == null)
    //        {
    //            Debug.LogError("HandleMonsterInfoSyncNotify ERROR, OtherMonsterNetSyncController is null, objId=" + cellData.Value.ObjId);
    //            return;
    //        }

    //        if (syncController.enabled)
    //        {
    //            syncController.SyncTransform(UtilityTool.ConvertFlatToVector3(cellData.Value.Pos.Value), UtilityTool.ConvertFlatToQuaternion(cellData.Value.Rotate.Value));
    //            if (unit.roleType == e_type_role.NPC)
    //                syncController.SyncBoneTransform(UtilityTool.ConvertFlatToQuaternion(cellData.Value.NpcBoneRot.Value));
    //        }
    //        else
    //            Debug.LogError("HandleMonsterInfoSyncNotify 同步位置失败，OtherMonsterNetSyncController没有被激活。");
    //    }
    //    //Debug.Log("收到 MonsterInfoSyncNotify 消息！！！");
    //}

    ///// <summary>
    ///// 怪物子弹创建
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleMonsterBulletCretateNotify(MessagePackage package)
    //{
    //    var data = S2CMonsterBulletCretateNotify.GetRootAsS2CMonsterBulletCretateNotify(GetMessagePackageByteBuffer(package));
    //    int uid = data.ObjId;

    //    WUnit unit = GameWorld.GlobalBattle.GetUnitById(uid);
    //    if (unit == null)
    //    {
    //        Debug.LogError("HandleMonsterBulletCretateNotify ERROR, unit is null, objId=" + uid);
    //        return;
    //    }

    //    //怪物子弹发射
    //    MonsterBulletManager.ClientMonsterFire(
    //        uid,
    //        UtilityTool.ConvertFlatToVector3(data.FirePos.Value),
    //        UtilityTool.ConvertFlatToQuaternion(data.FireRot.Value),
    //        data.AssetId,
    //        data.TargetId,
    //        UtilityTool.ConvertFlatToVector3(data.TargetOffset.Value),
    //        data.RandomSeed
    //        );

    //    //Debug.Log("收到 MonsterBulletCretateNotify 消息！！！");
    //}
    //#endregion

    //#region 击中相关
    ///// <summary>
    ///// 协议处理 : 击中伤害处理
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleHitDamage(MessagePackage package)
    //{
    //    var data = S2CHitDamage.GetRootAsS2CHitDamage(GetMessagePackageByteBuffer(package));

    //    //创建本地hitResult对象
    //    HitResultData hitData = HitResultData.CreateHitResultData(data);

    //    GameHitManager.HandleS2CHitDamageNotify(hitData);
    //}
    //#endregion

    //#region 剧情动画相关
    ///// <summary>
    ///// 协议处理 : 过场动画消息处理
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleCutScenesNotify(MessagePackage package)
    //{
    //    var data = S2CCutScenesNotify.GetRootAsS2CCutScenesNotify(GetMessagePackageByteBuffer(package));

    //    var type = (e_CutScenesType)data.CutScenesType;
    //    var status = System.Convert.ToBoolean(data.CutScenesStatus);

    //    if (TimeLineManager.Instance)
    //        TimeLineManager.Instance.Control(type, status);
    //    else
    //        Debug.LogError("HandleCutScenesNotify Error, TimeLineManager.Instance==null");
    //}
    //#endregion

    //#region 结算信息
    ///// <summary>
    ///// 结算信息处理
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleBattleStatisticalResultsNotify(MessagePackage package)
    //{
    //    var data = S2CBattleStatisticalResults.GetRootAsS2CBattleStatisticalResults(GetMessagePackageByteBuffer(package));

    //    if (string.IsNullOrEmpty(data.Results))
    //    {
    //        Debug.LogError("HandleBattleStatisticalResultsNotify Results==null，客户端同步失败。");
    //        return;
    //    }

    //    //转回results数据
    //    var results = StatisticalManager.StatisticalResults.ToResults(data.Results);
    //    StatisticalManager.Instance.statisticalResults = results;
    //}
    //#endregion

    //#region 大厅相关
    ///// <summary>
    ///// 游戏大厅操作信息
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleLobbyOperationNotify(MessagePackage package)
    //{
    //    int playerId = 0;
    //    var data = S2CLobbyOperationNotify.GetRootAsS2CLobbyOperationNotify(GetMessagePackageByteBuffer(package));

    //    //本地创建大厅操作数据
    //    LobbyOperationResultData resultData = LobbyOperationResultData.CreateResultData(data);

    //    playerId = data.OperationPlayerId;

    //    if (ClientServer.Instance.CheckPlayerData(playerId) == false)
    //    {
    //        Debug.LogError("HandleLobbyOperationNotify Chencge PlayerStyle Error! playerData==null playerId=" + playerId);
    //        return;
    //    }
    //    var playerData = ClientServer.Instance.GetPlayerData(playerId);
    //    if (playerData == null)
    //    {
    //        Debug.LogError("HandleLobbyOperationNotify Chencge PlayerStyle Error! playerData==null playerId=" + playerId);
    //        return;
    //    }

    //    if (resultData.isPlayerStyle)  //操作玩家样式
    //    {
    //        playerData.SetPlayerStyle(resultData.GetOperationPlayerStyle());

    //        if (playerData.JoinGameStatus)
    //        {
    //            WUnit oldUnit = GameWorld.GlobalBattle.GetUnitById(playerId);
    //            if (oldUnit == null)
    //            {
    //                Debug.LogError("HandleLobbyOperationNotify ERROR, unit is null, objId=" + playerId);
    //                return;
    //            }

    //            ////TODO:改变玩家样式...
    //            UnitManager.Instance.ChangePlayerStyle(oldUnit, playerData.PlayerStyle);
    //        }
    //    }
    //    else if (resultData.isWeaponType)  //操作武器类型
    //    {
    //        playerData.SetWeaponType(resultData.GetOperationWeaponType());

    //        if (playerData.JoinGameStatus)
    //        {
    //            WUnit oldUnit = GameWorld.GlobalBattle.GetUnitById(playerId);
    //            if (oldUnit == null)
    //            {
    //                Debug.LogError("HandleLobbyOperationNotify ERROR, unit is null, objId=" + playerId);
    //                return;
    //            }

    //            ////TODO:改变武器...
    //            UnitManager.Instance.ChangeWeaponStyle(oldUnit, playerData.WeaponType);
    //        }
    //    }
    //    else if (resultData.isJoinGame) //加入游戏
    //    {
    //        bool lastJoinStatus = playerData.JoinGameStatus;
    //        bool curJoinStatus = resultData.GetOperationJoinGameStatus();

    //        if (lastJoinStatus == curJoinStatus)
    //            return;

    //        playerData.SetJoinStatus(curJoinStatus);

    //        WUnit oldUnit = GameWorld.GlobalBattle.GetUnitById(playerId);

    //        if (playerData.JoinGameStatus)
    //        {
    //            if (oldUnit != null)
    //                GameWorld.GlobalBattle.RemoveUnit(oldUnit);

    //            WUnit player = null;
    //            //自己
    //            if (playerId == GameWorld.GlobalBattle.ControlUnit.Id)
    //            {
    //                player = UnitManager.Instance.CreateLocalPlayerUnit(playerId, playerData.GroupId, playerData.PlayerStyle, playerData.WeaponType);
    //                GameWorld.GlobalBattle.ControlUnit = player;
    //            }
    //            else
    //            {
    //                player = UnitManager.Instance.CreateOtherPlayer(playerId, playerData.GroupId, playerData.PlayerStyle, playerData.WeaponType);
    //            }

    //            //设置加入游戏状态
    //            player.JoinGame(true);

    //            //加入战场管理
    //            GameWorld.GlobalBattle.AddUnit(player);
    //        }
    //        else
    //        {
    //            if (oldUnit != null)
    //                GameWorld.GlobalBattle.RemoveUnit(oldUnit);
    //        }
    //    }
    //    else if (resultData.isGroupType) //阵营改变
    //    {
    //        playerData.SetGroupId(resultData.GetOperationGroupType());

    //        if (playerData.JoinGameStatus)
    //        {
    //            WUnit oldUnit = GameWorld.GlobalBattle.GetUnitById(playerId);
    //            if (oldUnit == null)
    //            {
    //                Debug.LogError("HandleLobbyOperationNotify ERROR, unit is null, objId=" + playerId);
    //                return;
    //            }
    //            UnitManager.Instance.ChangePlayerGroup(oldUnit, playerData.GroupId);
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("HandleLobbyOperationNotify 有未处理的大厅类型 OperationType=" + resultData.OperationType);
    //    }

    //    //if (GameManager.Instance.ServerConsoleUI.IsShow())
    //    //    GameManager.Instance.ServerConsoleUI.Refresh();
    //}
    //#endregion

    //#region 环境音
    ///// <summary>
    ///// 环境音
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandlePlayAmbientEffectNotify(MessagePackage package)
    //{
    //    var data = S2CPlayAmbientEffectNotify.GetRootAsS2CPlayAmbientEffectNotify(GetMessagePackageByteBuffer(package));

    //    WwiseAmbientEffectManager.Instance.PlayAmbientEffect(data.EventId, data.PointName);
    //}
    //#endregion

    //#region 战场相关
    ///// <summary>
    ///// 战场状态切换
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleBattleStateSwitchNotify(MessagePackage package)
    //{
    //    var data = S2CBattleStateSwitchNotify.GetRootAsS2CBattleStateSwitchNotify(GetMessagePackageByteBuffer(package));

    //    var state = data.StateType;

    //    var stateArg = data.StateArg;

    //    //Debug.LogError("HandleBattleStateSwitchNotify State = " + state);

    //    BattleRoomManager.Instance.BattleRoom.ChangeRoomState((e_RoomState_Type)state, stateArg);
    //}

    ///// <summary>
    ///// 游戏类型切换
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleGameLogicSwitchNotify(MessagePackage package)
    //{
    //    var data = S2CGameLogicSwitchNotify.GetRootAsS2CGameLogicSwitchNotify(GetMessagePackageByteBuffer(package));

    //    var logicType = (e_GameLogic_Type)data.GameLogicType;

    //    //Debug.LogError("HandleGameLogicSwitchNotify logicType = " + logicType);

    //    BattleRoomManager.Instance.CreateBattleRoom(GameWorld.GlobalBattle, (e_GameLogic_Type)data.GameLogicType);
    //}

    ///// <summary>
    ///// 游戏分数消息
    ///// </summary>
    ///// <param name="package"></param>
    //private void HandleGameScoreNotify(MessagePackage package)
    //{
    //    var data = S2CGameScoreNotify.GetRootAsS2CGameScoreNotify(GetMessagePackageByteBuffer(package));

    //    //Debug.LogError("HandleGameScoreNotify group = " + data.GroupId + ",Score = " + data.Score);

    //    GameStatusManager.Instance.SetGroupScore(data.GroupId, data.Score);
    //}
    //#endregion
}