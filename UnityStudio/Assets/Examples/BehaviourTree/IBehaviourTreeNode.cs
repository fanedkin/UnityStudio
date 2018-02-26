using System.Collections;

public enum RunStatus
{
    Completed,
    Failure,
    Running,
}

public interface IBehaviourTreeNode
{
    RunStatus status { get; set; }
    string nodeName { get; set; }
    bool Enter(object input);
    bool Leave(object input);
    bool Tick(object input, object output);
    RenderableNode renderNode { get; set; }
    IBehaviourTreeNode parent { get; set; }
    IBehaviourTreeNode Clone();
}

/************************************************************************/
/* 组合结点                                                             */
/************************************************************************/
public interface ICompositeNode : IBehaviourTreeNode
{
    void AddNode(IBehaviourTreeNode node);
    void RemoveNode(IBehaviourTreeNode node);
    bool HasNode(IBehaviourTreeNode node);

    void AddCondition(IConditionNode node);
    void RemoveCondition(IConditionNode node);
    bool HasCondition(IConditionNode node);

    ArrayList nodeList { get; }
    ArrayList conditionList { get; }
}

/************************************************************************/
/* 选择节点                                                             */
/************************************************************************/
public interface ISelectorNode : ICompositeNode
{

}

/************************************************************************/
/*顺序节点                                                              */
/************************************************************************/
public interface ISequenceNode : ICompositeNode
{

}

/************************************************************************/
/* 平行(并列)节点                                                             */
/************************************************************************/
public interface IParallelNode : ICompositeNode
{

}

//////////////////////////////////////////////////////////////////////////  

/************************************************************************/
/* 装饰结点                                                             */
/************************************************************************/
public interface IDecoratorNode : IBehaviourTreeNode
{

}

/************************************************************************/
/* 条件节点                                                             */
/************************************************************************/
public interface IConditionNode
{
    string nodeName { get; set; }
    bool ExternalCondition();
}

/************************************************************************/
/* 行为节点                                                             */
/************************************************************************/
public interface IActionNode : IBehaviourTreeNode
{

}

public interface IBehaviourTree
{

}