using UnityEngine;

public class SequenceNode : BaseCompositeNode, ISequenceNode
{
    public SequenceNode(bool canContinue_ = false) { canContinue = canContinue_; }
    public bool canContinue = false;


    public bool Enter(object input)
    {
        var checkOk = CheckNodeAndCondition();
        if (!checkOk) return false;
        var runningNode = nodeList_[runningNodeIndex] as IBehaviourTreeNode;
        checkOk = runningNode.Enter(input);
        if (!checkOk) return false;
        status_ = RunStatus.Running;
        return true;
    }

    public bool Leave(object input)
    {
        if (nodeList_.Count == 0)
        {
            status_ = RunStatus.Failure;
            Debug.Log("SequenceNode has no node!");
            return false;
        }
        var runningNode = nodeList_[runningNodeIndex] as IBehaviourTreeNode;
        runningNode.Leave(input);
        if (canContinue)
        {
            runningNodeIndex++;
            runningNodeIndex %= nodeList_.Count;
        }
        status_ = RunStatus.Completed;
        return true;
    }

    public bool Tick(object input, object output)
    {
        if (status_ == RunStatus.Failure) return false;
        if (status_ == RunStatus.Completed) return true;

        var runningNode = nodeList_[runningNodeIndex] as IBehaviourTreeNode;
        var checkOk = CheckCondition();
        if (!checkOk)
        {
            return false;
        }

        switch (runningNode.status)
        {
            case RunStatus.Running:
                if (!runningNode.Tick(input, output))
                {
                    runningNode.Leave(input);
                    return false;
                }

                break;
            default:
                runningNode.Leave(input);
                runningNodeIndex++;
                if (runningNodeIndex >= nodeList_.Count) break;
                var nextNode = nodeList_[runningNodeIndex] as IBehaviourTreeNode;
                var check = nextNode.Enter(input);
                if (!check) return false;
                break;
        }
        return true;
    }

    public override IBehaviourTreeNode Clone()
    {
        var clone = base.Clone() as SequenceNode;
        clone.canContinue = canContinue;
        return clone;
    }
}