using System.Collections.Generic;
using UnityEngine;

public class RenderableNode
{
    public RenderableNode parent;
    public IBehaviourTreeNode targetNode;
    public Rect posRect = new Rect();
    public string name;
    public int layer;
    public RunStatus staus;
    public override string ToString()
    {
        return name + "\n" + staus.ToString();
    }
    public virtual void Render()
    {
        bool running = staus == RunStatus.Running;
        var rect = posRect;
        rect.y -= (posRect.height / 2);

        var oldColor = GUI.color;
        if (running)
        {
            GUI.color = Color.green;
        }
        GUI.Box(rect, ToString());
        GUI.color = oldColor;

        if (parent == null && targetNode != null && targetNode.parent != null)
        {
            parent = targetNode.parent.renderNode;
        }
        if (parent != null)
        {
            Vector2 parentPos = new Vector2();
            parentPos.x = parent.posRect.x + parent.posRect.width;
            parentPos.y = parent.posRect.y;
            //GUIHelper.DrawLine(new Vector2(rect.x, rect.y + rect.height / 2), parentPos, running ? Color.green : Color.yellow);
        }

    }
}

public class RenderableCondictionNode : RenderableNode
{
    public IConditionNode targetCondictionNode;
    public override string ToString() { parent = null; return name; }
    public override void Render()
    {
        var rect = posRect;
        rect.y -= (posRect.height / 2);

        var oldColor = GUI.color;
        if (targetCondictionNode.ExternalCondition())
            GUI.color = Color.green;
        else
            GUI.color = Color.blue;
        GUI.Box(rect, ToString());
        GUI.color = oldColor;
    }
}

public class EmptyNode : RenderableNode { public override void Render() { } }

public class NodeBox
{
    public Rect posRect = new Rect();
    public List<RenderableNode> nodeList = new List<RenderableNode>();
    public void AddNode(RenderableNode node)
    {
        nodeList.Add(node);
    }
    public void Render()
    {
        posRect.y = Screen.height / 2;
        Rect rect = new Rect();

        foreach (var node in nodeList)
        {
            var n = node;
            rect.height += (n.posRect.height + 1);
            rect.width = n.posRect.width + 10;
        }
        rect.height += 10;
        rect.x = posRect.x - rect.width / 2;
        rect.y = posRect.y - rect.height / 2;
        //GUI.Box(rect, "");  
        posRect.width = rect.width;
        posRect.height = rect.height;
        float height = 0;
        for (var i = 0; i < nodeList.Count; i++)
        {
            var n = nodeList[i];
            n.posRect.y = rect.y + height + n.posRect.height / 2 + 5;
            n.posRect.x = rect.x + 5;
            n.Render();
            height += n.posRect.height + 1;
        }
    }
}
