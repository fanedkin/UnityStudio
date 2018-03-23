using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPoint
{
    public int X;
    public int Y;
    public float F;
    /// <summary>
    /// 开始节点到自身节点的距离
    /// </summary>
    public float G;
    /// <summary>
    /// 自身节点到结束节点的距离
    /// </summary>
    public float H;
    public AStarPoint ParentPoint;
    public bool IsWall;

    public AStarPoint(int x, int y,AStarPoint point=null,bool isWall=false)
    {
        X = x;
        Y = y;
        ParentPoint = point;
        IsWall = isWall;
    }

    public void Update(AStarPoint parentPoint,float g)
    {
        ParentPoint = parentPoint;
        G = g;
        F = G + H;
    }
}
