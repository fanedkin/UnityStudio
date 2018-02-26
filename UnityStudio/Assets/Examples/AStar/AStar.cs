using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    private int mapWidth = 8;
    private int mapHeight = 6;

    public AStarPoint[,] map=new AStarPoint[8,6];
	void Start ()
	{
	    InitMap();
	    AStarPoint startPoint = map[2, 3];
        AStarPoint endPoint = map[6, 3];
	    FindPath(startPoint, endPoint);
	    ShowPath(startPoint, endPoint);
	}

    private void InitMap()
    {
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                map[i,j]=new AStarPoint(i,j);
            }
        }
        map[4, 2].IsWall = true;
        map[4, 3].IsWall = true;
        map[4, 4].IsWall = true;
    }

    private void ShowPath(AStarPoint start, AStarPoint end)
    {
        AStarPoint temp = end;
        while (true)
        {
            //Debug.Log(temp.X + "," + temp.Y);
            Color c = Color.gray;
            if (temp == start)
            {
                c = Color.green;
            }
            else if (temp == end)
            {
                c = Color.red;
            }
            CreateCube(temp.X, temp.Y, c);

            if (temp.ParentPoint == null)
                break;
            temp = temp.ParentPoint;
        }
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (map[x, y].IsWall)
                {
                    CreateCube(x, y, Color.blue);
                }
            }
        }
    }

    private void CreateCube(int x, int y, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(x, y, 0);
        go.GetComponent<Renderer>().material.color = color;
    }

    private void FindPath(AStarPoint startPoint, AStarPoint endPoint)
    {
        List<AStarPoint> openList=new List<AStarPoint>();//开启列表
        List<AStarPoint> closeList = new List<AStarPoint>();//关闭列表
        openList.Add(startPoint);
        while (openList.Count > 0)
        {
            AStarPoint MinFPoint = FindMinFOfPoint(openList);//找到F值最小的节点，就是距离终点节点距离最短的节点
            openList.Remove(MinFPoint);//这个节点会进入关闭列表
            closeList.Add(MinFPoint);//从开启列表中移除
            List<AStarPoint> surroundPoints = GetSurroundPoints(MinFPoint);//找出周围的可移动点，不能是墙
            PointsFilter(surroundPoints, closeList);//过滤周围的这些节点，就是能不能移动过去的（如果是墙肯定是不能移动的，如果是斜线移动的节点就是看如果两个夹角是墙是否能斜线移动，看具体策划）
            foreach (AStarPoint surroundPoint in surroundPoints)
            {
                if (openList.Contains(surroundPoint))
                {
                    float nowG = CalcG(surroundPoint, MinFPoint);
                    if (surroundPoint.G > nowG)
                    {
                        surroundPoint.Update(MinFPoint, nowG);
                    }
                }
                else
                {
                    surroundPoint.ParentPoint = MinFPoint;
                    CalcF(surroundPoint, endPoint);
                    openList.Add(surroundPoint);
                }
            }
            if (openList.Contains(endPoint))
            {
                break;
            }
        }
    }

    private AStarPoint FindMinFOfPoint(List<AStarPoint> openList)
    {
        float f = float.MaxValue;
        AStarPoint temp = null;
        foreach (AStarPoint point in openList)
        {
            if (point.F < f)
            {
                temp = point;
                f = point.F;
            }
        }
        return temp;
    }

    private float CalcG(AStarPoint nowPoint, AStarPoint tempParentPoint)
    {
        return tempParentPoint.G + Vector2.Distance(new Vector2(nowPoint.X, nowPoint.Y),
                new Vector2(tempParentPoint.X, tempParentPoint.Y));
    }
    private void CalcF(AStarPoint nowPoint, AStarPoint endPoint)
    {
        float h = Mathf.Abs(endPoint.X - nowPoint.X) + Mathf.Abs(endPoint.Y - nowPoint.Y);
        float g = 0;
        if (nowPoint.ParentPoint == null)
        {
            g = 0;
        }
        else
        {
            g = Vector2.Distance(new Vector2(nowPoint.X, nowPoint.Y),
                new Vector2(nowPoint.ParentPoint.X, nowPoint.ParentPoint.Y));
        }
        float f = g + h;
        nowPoint.G = g;
        nowPoint.H = h;
        nowPoint.F = f;
    }
    private List<AStarPoint> GetSurroundPoints(AStarPoint point)
    {
        AStarPoint up = null, down = null, left = null, right = null;
        AStarPoint lu = null, ru = null, ld = null, rd = null;
        if (point.Y < mapHeight - 1)
        {
            up = map[point.X, point.Y + 1];
        }
        if (point.Y > 0)
        {
            down = map[point.X, point.Y - 1];
        }
        if (point.X > 0)
        {
            left = map[point.X - 1, point.Y];
        }
        if (point.X < mapWidth - 1)
        {
            right = map[point.X + 1, point.Y];
        }
        if (up != null && left != null)
        {
            lu = map[point.X - 1, point.Y + 1];
        }
        if (up != null && right != null)
        {
            ru = map[point.X + 1, point.Y + 1];
        }
        if (down != null && left != null)
        {
            ld = map[point.X - 1, point.Y - 1];
        }
        if (down != null && right != null)
        {
            rd = map[point.X + 1, point.Y - 1];
        }
        List<AStarPoint> list = new List<AStarPoint>();
        if (down != null && down.IsWall == false)
        {
            list.Add(down);
        }
        if (up != null && up.IsWall == false)
        {
            list.Add(up);
        }
        if (left != null && left.IsWall == false)
        {
            list.Add(left);
        }
        if (right != null && right.IsWall == false)
        {
            list.Add(right);
        }
        if (lu != null && lu.IsWall == false && left.IsWall == false && up.IsWall == false)
        {
            list.Add(lu);
        }
        if (ld != null && ld.IsWall == false && left.IsWall == false && down.IsWall == false)
        {
            list.Add(ld);
        }
        if (ru != null && ru.IsWall == false && right.IsWall == false && up.IsWall == false)
        {
            list.Add(ru);
        }
        if (rd != null && rd.IsWall == false && right.IsWall == false && down.IsWall == false)
        {
            list.Add(rd);
        }
        return list;
    }
    private void PointsFilter(List<AStarPoint> src, List<AStarPoint> closeList)
    {
        foreach (AStarPoint p in closeList)
        {
            if (src.IndexOf(p) > -1)
            {
                src.Remove(p);
            }
        }
    }
}
