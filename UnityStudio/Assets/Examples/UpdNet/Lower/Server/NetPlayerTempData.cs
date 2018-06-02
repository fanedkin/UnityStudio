using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayerTempData  {

    public NetPlayerTempData()
    {
        status = Status.None;
    }
    //状态
    public enum Status
    {
        None,
        Room,
        Fight,
    }
    public Status status;
    //room状态
    public NetRoom room;
    public int team = 1;
    public bool isOwner = false;
    //战场相关
    public long lastUpdateTime;
    public float posX;
    public float posY;
    public float posZ;
}
