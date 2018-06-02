using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayer
{
    //id、连接、玩家数据
    public string ip;
    public Conn conn;
    public NetPlayerData data;
    public NetPlayerTempData tempData;

    //构造函数，给id和conn赋值
    public NetPlayer(string ip, Conn conn)
    {
        this.ip = ip;
        this.conn = conn;
        tempData = new NetPlayerTempData();
    }

    //发送
    public void Send()
    {
        //if (conn == null)
        //    return;
        //ServNet.instance.Send(conn, proto);
    }

    //踢下线
    //public static bool KickOff(string id)
    //{
    //    //Conn[] conns = ServNet.instance.conns;
    //    //for (int i = 0; i < conns.Length; i++)
    //    //{
    //    //    if (conns[i] == null)
    //    //        continue;
    //    //    if (!conns[i].isUse)
    //    //        continue;
    //    //    if (conns[i].player == null)
    //    //        continue;
    //    //    if (conns[i].player.id == id)
    //    //    {
    //    //        lock (conns[i].player)
    //    //        {
    //    //            if (proto != null)
    //    //                conns[i].player.Send(proto);

    //    //            return conns[i].player.Logout();
    //    //        }
    //    //    }
    //    //}
    //    //return true;
    //}

    //下线
    //public bool Logout()
    //{
    ////    ////事件处理，稍后实现
    ////    //ServNet.instance.handlePlayerEvent.OnLogout(this);
    ////    ////保存
    ////    //if (!DataMgr.instance.SavePlayer(this))
    ////    //    return false;
    ////    ////下线
    ////    //conn.player = null;
    ////    //conn.Close();
    ////    //return true;
    //}
}
