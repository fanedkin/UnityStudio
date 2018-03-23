using System;

class PatrolAction : BaseActionNode
{

    public PatrolAction() { nodeName_ += "巡逻行为"; }

    public override bool Tick(object input_, object output_)
    {
        // var input = input_ as WarriorInputData;   
        var output = output_ as WarriorOutPutData;
        output.action = WarriorActon.ePatrol;
        return true;
    }
}

class RunAwayAction : BaseActionNode
{
    public RunAwayAction() { nodeName_ += "逃跑行为"; }

    public override bool Tick(object input_, object output_)
    {
        // var input = input_ as WarriorInputData;  
        var output = output_ as WarriorOutPutData;
        output.action = WarriorActon.eRunAway;
        return true;
    }
}

class AttackAction : BaseActionNode
{
    public AttackAction() { nodeName_ += "攻击行为"; }

    public override bool Tick(object input_, object output_)
    {
        // var input = input_ as WarriorInputData;  
        var output = output_ as WarriorOutPutData;
        output.action = WarriorActon.eAttack;
        return true;
    }
}

class CrazyAttackAction : BaseActionNode
{
    public CrazyAttackAction() { nodeName_ += "疯狂攻击行为"; }

    public override bool Tick(object input_, object output_)
    {
        // var input = input_ as WarriorInputData;  
        var output = output_ as WarriorOutPutData;
        output.action = WarriorActon.eCrazyAttack;
        return true;
    }
}

class AlertAction : BaseActionNode
{
    public AlertAction() { nodeName_ += "警戒行为"; }
    public override bool Tick(object input_, object output_)
    {
        // var input = input_ as WarriorInputData;  
        var output = output_ as WarriorOutPutData;
        output.action = WarriorActon.eAlert;
        return true;
    }
}


