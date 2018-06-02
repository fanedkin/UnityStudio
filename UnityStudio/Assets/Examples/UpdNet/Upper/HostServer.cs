using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using VRNetLibrary;

/// <summary>
/// 主机服务，主要保存一些主机相关的数据和接口，用于分离主机部分网络接口类跟数据相关
/// </summary>
public partial class HostServer : Singleton<HostServer>
{
    private Dictionary<IPEndPoint, Conn> mIPEndPointToConnDict;
    private Dictionary<int, Conn> mSessionIDToConnDict;
    /// <summary>
    /// Unit位置结构
    /// </summary>
    public struct UnitTransform
    {
        public int objId;
        public Vector3 pos;
        public Quaternion rot;
        public Quaternion npcBoneRot; //modify 2018.1.26 临时使用
    }

    public Conn GetConnByIPEndPoint(IPEndPoint iPEndPoint)
    {
        Conn conn;
        mIPEndPointToConnDict.TryGetValue(iPEndPoint, out conn);
        return conn;
    }
    public Conn GetConnBySessionID(int sessionID)
    {
        Conn conn;
        mSessionIDToConnDict.TryGetValue(sessionID, out conn);
        return conn;
    }

    public bool IsConnExist(IPEndPoint iPEndPoint)
    {
       return mIPEndPointToConnDict.ContainsKey(iPEndPoint);
    }
    public bool IsConnExist(int sessionID)
    {
        return mSessionIDToConnDict.ContainsKey(sessionID);
    }
    public void AddConnToDict(IPEndPoint iPEndPoint,Conn conn)
    {
        if (IsConnExist(iPEndPoint))
        {
            mIPEndPointToConnDict[iPEndPoint] = conn;
        }
        else
        {
            mIPEndPointToConnDict.Add(iPEndPoint, conn);
        }
   
    }
    public void AddConnToDict(int sessionID, Conn conn)
    {
        if (IsConnExist(sessionID))
        {
            mSessionIDToConnDict[sessionID] = conn;
        }
        else
        {
            mSessionIDToConnDict.Add(sessionID, conn);
        }

    }
    ///// <summary>
    ///// 玩家信息Session字典
    ///// </summary>
    //private Dictionary<int, PlayerData> playerSessionHash = new Dictionary<int, PlayerData>();

    ///// <summary>
    ///// 玩家信息id字典
    ///// </summary>
    //private Dictionary<int, PlayerData> playerIdHash = new Dictionary<int, PlayerData>();

    /// <summary>
    /// sessionId绑定playerid表
    /// </summary>
    private Dictionary<int, int> sessionIdToPlayerIdHash = new Dictionary<int, int>();

    /// <summary>
    /// Session表
    /// </summary>
    private HashSet<int> sessionHash = new HashSet<int>();

    /// <summary>
    /// 固定数组，减少发送GC。
    /// </summary>
    public int[] sessionArray { get; private set; }

    ///// <summary>
    ///// 获取玩家数据列表
    ///// </summary>
    ///// <returns></returns>
    //public PlayerData[] GetPlayerDatas(bool filterLocalServer = false)
    //{
    //    if (playerSessionHash.Count == 0)
    //        return new PlayerData[0];

    //    PlayerData[] datas = new PlayerData[filterLocalServer ? playerSessionHash.Count - 1 : playerSessionHash.Count];

    //    int i = 0;
    //    foreach (var pair in playerSessionHash)
    //    {
    //        if (filterLocalServer && pair.Key == MessageHead.LocalClientMessageSessionID)
    //            continue;

    //        datas[i++] = pair.Value;
    //    }

    //    return datas;
    //}

    ///// <summary>
    ///// 通过sessionId获得一个已经连接的玩家信息
    ///// </summary>
    ///// <param name="session"></param>
    ///// <param name="isLogError">是否打印错误</param>
    ///// <returns></returns>
    //public PlayerData GetPlayerDataBySessionId(int session, bool isLogError = true)
    //{
    //    PlayerData data;
    //    if (playerSessionHash.TryGetValue(session, out data) == false)
    //    {
    //        if (isLogError)
    //        {
    //            Debug.LogError("GetPlayerDataBySessionId() ERROR, 没有找到玩家，sessionId = " + session);
    //        }
    //        return null;
    //    }

    //    return data;
    //}

    ///// <summary>
    ///// 通过playerId获得一个已经连接的玩家信息
    ///// </summary>
    ///// <param name="session"></param>
    ///// <param name="isLogError">是否打印错误</param>
    ///// <returns></returns>
    //public PlayerData GetPlayerDataById(int id, bool isLogError = true)
    //{
    //    PlayerData data;
    //    if (playerIdHash.TryGetValue(id, out data) == false)
    //    {
    //        if (isLogError)
    //        {
    //            Debug.LogError("GetPlayerDataById() ERROR, 没有找到玩家，Id = " + id);
    //        }
    //        return null;
    //    }

    //    return data;
    //}

    ///// <summary>
    ///// 根据sessionId生成玩家id,并与sessionId绑定
    ///// </summary>
    ///// <returns></returns>
    //public int SpawnPlayerID(int sessionId)
    //{
    //    int playerid;
    //    if (sessionIdToPlayerIdHash.TryGetValue(sessionId, out playerid) == false)
    //    {
    //        playerid = ServerBattleManager.Instance.GetSpawnPlayerID();
    //        sessionIdToPlayerIdHash.Add(sessionId, playerid);
    //    }
    //    return playerid;
    //}

    ///// <summary>
    ///// 注册一个玩家Session关联
    ///// </summary>
    ///// <param name="sessionId"></param>
    ///// <param name="playerId"></param>
    //public void RegisterPlayerSession(int sessionId, int playerId, string mechineName, string mechineId, bool joinStatus, e_group_type groupId, e_Player_Style style, e_weapon_type weapon)
    //{
    //    if (playerSessionHash.ContainsKey(sessionId))
    //    {
    //        Debug.LogError("RegisterPlayerSession() ERROR, 为什么注册重复的玩家session! sessionId = " + sessionId + ",playerId =" + playerId);
    //        return;
    //    }

    //    PlayerData data = new PlayerData(sessionId, playerId, mechineName, mechineId, joinStatus, groupId, style, weapon);
    //    playerSessionHash.Add(data.SessionId, data);
    //    playerIdHash.Add(data.PlayerId, data);

    //    AddPlayerSessionList(sessionId);
    //}

    ///// <summary>
    ///// 删除一个玩家session关联
    ///// </summary>
    ///// <param name="session"></param>
    //public void UnRegisterPlayerSession(int session)
    //{
    //    PlayerData data;
    //    if (playerSessionHash.TryGetValue(session, out data) == false)
    //    {
    //        Debug.LogError("UnRegisterPlayerSession() ERROR, 没有找到玩家，sessionId = " + session);
    //        return;
    //    }

    //    playerSessionHash.Remove(session);
    //    playerIdHash.Remove(data.PlayerId);
    //    RemovePlayerSessionList(session);
    //}

    ///// <summary>
    ///// 添加进玩家列表
    ///// </summary>
    ///// <param name="session"></param>
    //private void AddPlayerSessionList(int session)
    //{
    //    if (!sessionHash.Contains(session))
    //    {
    //        sessionHash.Add(session);
    //        sessionArray = sessionHash.ToArray(); //缓存数组
    //    }
    //}

    ///// <summary>
    ///// 从玩家列表移除
    ///// </summary>
    ///// <param name="session"></param>
    //private void RemovePlayerSessionList(int session)
    //{
    //    if (sessionHash.Contains(session))
    //    {
    //        sessionHash.Remove(session);
    //        sessionArray = sessionHash.ToArray(); //缓存数组
    //    }
    //}

    ///// <summary>
    ///// 检查玩家是否存在
    ///// </summary>
    ///// <param name="playerid"></param>
    ///// <returns></returns>
    //private bool CheckPlayer(int playerid)
    //{
    //    return playerIdHash.ContainsKey(playerid);
    //}

    ///// <summary>
    ///// 通过playerid获取一个已经连接的玩家信息
    ///// </summary>
    ///// <param name="playerid"></param>
    ///// <param name="isLogError"></param>
    ///// <returns></returns>
    //private PlayerData GetPlayerDataByPlayerId(int playerid, bool isLogError = true)
    //{
    //    PlayerData data;
    //    if (playerIdHash.TryGetValue(playerid, out data) == false)
    //    {
    //        if (isLogError)
    //        {
    //            Debug.LogError("GetPlayerDataByPlayerId() ERROR, 没有找到玩家，playerid = " + playerid);
    //        }
    //        return null;
    //    }
    //    return data;
    //}

    ///// <summary>
    ///// 根据id获取玩家信息
    ///// </summary>
    ///// <param name="id"></param>
    ///// <returns></returns>
    //public bool GetPlayerInfoByID(int id, out SettingManager.PlayerInfo playerInfo)
    //{
    //    playerInfo = null;

    //    var playerData = GetPlayerDataByPlayerId(id);

    //    if (playerData == null)
    //        return false;

    //    return GetPlayerInfoBySID(playerData.SessionId, out playerInfo);
    //}

    ///// <summary>
    ///// 根据sid获取玩家信息
    ///// </summary>
    ///// <param name="id"></param>
    ///// <returns></returns>
    //public bool GetPlayerInfoBySID(int sid, out SettingManager.PlayerInfo playerInfo)
    //{
    //    playerInfo = null;

    //    var ipInfo = ServersManager.Instance.GetIp(sid);

    //    if (ipInfo != null)
    //    {
    //        playerInfo = SettingManager.Instance.GetPlayerInfoByIP(ipInfo.Address.ToString());
    //        return true;
    //    }
    //    return false;
    //}

    ///// <summary>
    ///// 获取是否可以加入游戏
    ///// </summary>
    ///// <returns></returns>
    //public bool GetCanjoinGame()
    //{
    //    return BattleRoomManager.Instance.BattleRoom.isStartFight == false;
    //}

    ///// <summary>
    ///// 获取可以加入的阵营id
    ///// </summary>
    ///// <returns></returns>
    //public e_group_type GetCanJoinGroup()
    //{
    //    return e_group_type.Unit_Group_Player_A;
    //}

    ///// <summary>
    ///// 获取开启的玩家数量
    ///// </summary>
    ///// <returns></returns>
    //public int GetJoinPlayerCount()
    //{
    //    int count = 0;
    //    foreach (var pair in playerIdHash)
    //    {
    //        if (pair.Value.JoinGameStatus)
    //            count++;
    //    }
    //    return count;
    //}

    ///// <summary>
    ///// 获取连接玩家的机器码
    ///// </summary>
    ///// <returns></returns>
    //public string[] GetAllRegisterMechineId(bool isFilterClosed = false)
    //{
    //    if (playerIdHash.Count == 0)
    //        return null;
    //    List<string> temps = new List<string>();
    //    foreach (var pair in playerIdHash)
    //    {
    //        if (isFilterClosed == false || pair.Value.JoinGameStatus)
    //            temps.Add(pair.Value.MechineId);
    //    }
    //    return temps.ToArray();
    //}

    ///// <summary>
    ///// 玩家加入状态
    ///// </summary>
    ///// <returns></returns>
    //public void SetJoinStatusByID(int playerId, bool status)
    //{
    //    var playerdata = GetPlayerDataByPlayerId(playerId);
    //    if (playerdata != null)
    //    {
    //        if (playerdata.JoinGameStatus == status)
    //            return;

    //        if (status == false)
    //        {

    //        }
    //        else
    //        {

    //        }

    //        playerdata.SetJoinStatus(status);
    //        LobbyOperationServer(playerId, LobbyOperationResultData.operation_JoinGame, System.Convert.ToInt32(status));
    //    }
    //}

    ///// <summary>
    ///// 设置武器类型
    ///// </summary>
    ///// <param name="playerId"></param>
    ///// <param name="type"></param>
    //public void SetWeaponType(int playerId, e_weapon_type type)
    //{
    //    var playerdata = GetPlayerDataByPlayerId(playerId);
    //    if (playerdata != null)
    //    {
    //        playerdata.SetWeaponType(type);
    //        LobbyOperationServer(playerId, LobbyOperationResultData.operation_WeaponType, (int)type);
    //    }
    //}

    ///// <summary>
    ///// 设置玩家类型
    ///// </summary>
    ///// <param name="playerId"></param>
    ///// <param name="type"></param>
    //public void SetPlayerStyle(int playerId, e_Player_Style type)
    //{
    //    var playerdata = GetPlayerDataByPlayerId(playerId);
    //    if (playerdata != null)
    //    {
    //        playerdata.SetPlayerStyle(type);
    //        LobbyOperationServer(playerId, LobbyOperationResultData.operation_PlayerStyle, (int)type);
    //    }
    //}

    ///// <summary>
    ///// 设置阵营
    ///// </summary>
    ///// <param name="playerId"></param>
    ///// <param name="type"></param>
    //public void SetGroupType(int playerId, e_group_type type)
    //{
    //    var playerdata = GetPlayerDataByPlayerId(playerId);
    //    if (playerdata != null)
    //    {
    //        playerdata.SetGroupId(type);
    //        LobbyOperationServer(playerId, LobbyOperationResultData.operation_GroupType, (int)type);
    //    }
    //}

    ///// <summary>
    ///// 大厅操作服务
    ///// </summary>
    ///// <param name="playerId"></param>
    ///// <param name="OperationType"></param>
    ///// <param name="OperationValue"></param>
    //public void LobbyOperationServer(int playerId, int OperationType, int OperationValue)
    //{
    //    LobbyOperationResultData data = new LobbyOperationResultData();
    //    data.OperationType = OperationType;
    //    data.OperationValue = OperationValue;
    //    HostSendMessage.BroadcastLobbyOperationNotify(playerId, data);

    //    if (GameManager.Instance.ServerConsoleUI.IsShow())
    //        GameManager.Instance.ServerConsoleUI.Refresh();
    //}

    //public bool IsCanFilterLocalServer(int sessionId)
    //{
    //    if (SettingManager.Instance.ServerType == e_Server_Type.Server && sessionId == MessageHead.LocalClientMessageSessionID)
    //        return true;
    //    return false;
    //}
}