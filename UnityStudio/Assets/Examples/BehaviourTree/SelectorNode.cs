public class SelectorNode : BaseCompositeNode, ISelectorNode
{
    public bool Enter(object input)
    {
        var checkOk = CheckNodeAndCondition();
        if (!checkOk) return false;

        do
        {
            var runningNode = nodeList_[runningNodeIndex] as IBehaviourTreeNode;
            checkOk = runningNode.Enter(input);
            if (checkOk) break;
            runningNodeIndex++;
            if (runningNodeIndex >= nodeList_.Count) return false;
        } while (!checkOk);

        status_ = RunStatus.Running;
        return true;
    }

    public bool Leave(object input)
    {
        var runningNode = nodeList_[runningNodeIndex] as IBehaviourTreeNode;
        runningNode.Leave(input);
        runningNodeIndex = 0;
        status_ = RunStatus.Completed;
        return true;
    }

    public bool Tick(object input, object output)
    {
        if (status_ == RunStatus.Failure) return false;
        if (status_ == RunStatus.Completed) return true;
        var checkOk1 = CheckCondition();
        if (!checkOk1) return false;
        var runningNode = nodeList_[runningNodeIndex] as IBehaviourTreeNode;
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
                if (runningNodeIndex >= nodeList_.Count) return false;

                bool checkOk = false;
                do
                {
                    var nextNode = nodeList_[runningNodeIndex] as IBehaviourTreeNode;
                    checkOk = nextNode.Enter(input);
                    if (checkOk) break;
                    runningNodeIndex++;
                    if (runningNodeIndex >= nodeList_.Count) return false;
                } while (!checkOk);
                break;
        }
        return true;
    }
}