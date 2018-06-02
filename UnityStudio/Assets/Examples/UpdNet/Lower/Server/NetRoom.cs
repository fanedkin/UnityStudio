using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRNetLibrary;
public class NetRoom
{
    //状态
    public enum Status
    {
        Prepare = 1,
        Fight = 2,
    }
    public Status status = Status.Prepare;
    //玩家
    public int maxPlayers = 6;
    public Dictionary<string, NetPlayer> list = new Dictionary<string, NetPlayer>();

    public bool isInit;
    /// <summary>
    /// 处理后的消息队列，用于消息回调
    /// </summary>
    private SwitchQueue<MessagePackage> mMessageQueue;
    void Update()
    {
        if (this.isInit)
        {
            this.MessageProcess();
        }
    }
    /// <summary>
    /// 消息处理
    /// </summary>
    private void MessageProcess()
    {
        this.mMessageQueue.Switch();
        if (!this.mMessageQueue.Empty())
        {
            while (!this.mMessageQueue.Empty())
            {
                MessagePackage package = this.mMessageQueue.Dequeue();
            }
        }
    }

    /// <summary>
    /// 消息处理，加入回调队列
    /// </summary>
    /// <param name="buffer"></param>
    public void OnMessage(MessagePackage package)
    {
        if (!this.isInit)
        {
            Debug.LogError(this + " 没有初始化");
        }
        else
        {
            this.mMessageQueue.Enqueue(package);
        }
    }
    //添加玩家
    public bool AddPlayer(NetPlayer player)
    {
        lock (list)
        {
            if (list.Count >= maxPlayers)
                return false;
            NetPlayerTempData tempData = player.tempData;
            //tempData.team = SwichTeam();
            tempData.status = NetPlayerTempData.Status.Room;

            if (list.Count == 0)
                tempData.isOwner = true;
            string id = player.ip;
            list.Add(id, player);
        }
        return true;
    }



    //删除玩家
    public void DelPlayer(string id)
    {
        lock (list)
        {
            if (!list.ContainsKey(id))
                return;
            bool isOwner = list[id].tempData.isOwner;
            list[id].tempData.status = NetPlayerTempData.Status.None;
            list.Remove(id);
            if (isOwner)
                UpdateOwner();
        }
    }

    //更换房主
    public void UpdateOwner()
    {
        lock (list)
        {
            if (list.Count <= 0)
                return;
            Dictionary<string, NetPlayer>.Enumerator en = list.GetEnumerator();
            for (int i = 0; i < list.Count; i++)
            {
                if (en.MoveNext())
                {
                    NetPlayer p= en.Current.Value;
                    if (i == 0)
                    {
                        p.tempData.isOwner = true;
                    }
                    else
                    {
                        p.tempData.isOwner = false;
                    }
                }
            }
        }
    }

    //广播
    public void Broadcast()
    {
        foreach (NetPlayer player in list.Values)
        {
            player.Send();
        }
    }

 
}
