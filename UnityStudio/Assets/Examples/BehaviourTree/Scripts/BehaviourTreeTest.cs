using System.Collections;
using System.Collections.Generic;
using UnityEngine;


internal class WarriorInputData
{
    public WarriorActon action;
    public CharacterAttribute attribute;
}

class WarriorOutPutData
{
    public WarriorActon action;
}

enum WarriorActon
{
    ePatrol,
    eRunAway,
    eAttack,
    eCrazyAttack,
    eAlert,
    eIdle
}
public class BehaviourTreeTest : MonoBehaviour {

    private ICompositeNode rootNode = new SelectorNode();
    private WarriorInputData inputData = new WarriorInputData();
    private WarriorOutPutData outputData = new WarriorOutPutData();
    // Use this for initialization  
    public void Start()
    {
        inputData.attribute = GetComponent<CharacterAttribute>();

        rootNode.nodeName += "根";

        //条件  
        var hasNoTarget = new PreconditionNOT(() => { return inputData.attribute.hasTarget; });
        hasNoTarget.nodeName = "无目标";
        var hasTarget = new Precondition(hasNoTarget);
        hasTarget.nodeName = "发现目标";
        var isAnger = new Precondition(() => { return inputData.attribute.isAnger; });
        isAnger.nodeName = "愤怒状态";
        var isNotAnger = new PreconditionNOT(isAnger);
        isNotAnger.nodeName = "非愤怒状态";
        var HPLessThan500 = new Precondition(() => { return inputData.attribute.health < 500; });
        HPLessThan500.nodeName = "血少于500";
        var HPMoreThan500 = new PreconditionNOT(HPLessThan500);
        HPMoreThan500.nodeName = "血大于500";
        var isAlert = new Precondition(() => { return inputData.attribute.isAlert; });
        isAlert.nodeName = "警戒";
        var isNotAlert = new PreconditionNOT(isAlert);
        isNotAlert.nodeName = "非警戒";


        var patrolNode = new SequenceNode();
        patrolNode.nodeName += "巡逻";
        patrolNode.AddCondition(hasNoTarget);
        patrolNode.AddCondition(isNotAlert);
        patrolNode.AddNode(new PatrolAction());

        var alert = new SequenceNode();
        alert.nodeName += "警戒";
        alert.AddCondition(hasNoTarget);
        alert.AddCondition(isAlert);
        alert.AddNode(new AlertAction());

        var runaway = new SequenceNode();
        runaway.nodeName += "逃跑";
        runaway.AddCondition(hasTarget);
        runaway.AddCondition(HPLessThan500);
        runaway.AddNode(new RunAwayAction());

        var attack = new SelectorNode();
        attack.nodeName += "攻击";
        attack.AddCondition(hasTarget);
        attack.AddCondition(HPMoreThan500);

        var attackCrazy = new SequenceNode();
        attackCrazy.nodeName += "疯狂攻击";
        attackCrazy.AddCondition(isAnger);
        attackCrazy.AddNode(new CrazyAttackAction());
        attack.AddNode(attackCrazy);

        var attackNormal = new SequenceNode();
        attackNormal.nodeName += "普通攻击";
        attackNormal.AddCondition(isNotAnger);
        attackNormal.AddNode(new AttackAction());
        attack.AddNode(attackNormal);

        rootNode.AddNode(patrolNode);
        rootNode.AddNode(alert);
        rootNode.AddNode(runaway);
        rootNode.AddNode(attack);
        var ret = rootNode.Enter(inputData);
        if (!ret)
        {
            Debug.Log("无可执行节点！");
        }
    }

    // Update is called once per frame  
    void Update()
    {
        var ret = rootNode.Tick(inputData, outputData);

        if (!ret)
            rootNode.Leave(inputData);

        if (rootNode.status == RunStatus.Completed)
        {
            ret = rootNode.Enter(inputData);
            if (!ret)
                rootNode.Leave(inputData);
        }
        else if (rootNode.status == RunStatus.Failure)
        {
            Debug.Log("BT Failed");
            enabled = false;
        }

        if (outputData.action != inputData.action)
        {
            OnActionChange(outputData.action, inputData.action);
            inputData.action = outputData.action;
        }
    }

    void OnActionChange(WarriorActon action, WarriorActon lastAction)
    {
        //  print("OnActionChange "+action+" last:"+lastAction);  
        switch (lastAction)
        {
            case WarriorActon.ePatrol:
                //GetComponent<WarriorPatrol>().enabled = false;
                break;
            case WarriorActon.eAttack:
            case WarriorActon.eCrazyAttack:
                //GetComponent<WarriorAttack>().enabled = false;
                break;
            case WarriorActon.eRunAway:
                //GetComponent<WarriorRunAway>().enabled = false;
                break;
            case WarriorActon.eAlert:
                //GetComponent<WarriorAlert>().enabled = false;
                break;
        }

        switch (action)
        {
            case WarriorActon.ePatrol:
                //GetComponent<WarriorPatrol>().enabled = true;
                break;
            case WarriorActon.eAttack:
                //var attack = GetComponent<WarriorAttack>();
                //attack.revenge = false;
                //attack.enabled = true;
                break;
            case WarriorActon.eCrazyAttack:
                //var crazyAttack = GetComponent<WarriorAttack>();
                //crazyAttack.revenge = true;
                //crazyAttack.enabled = true;
                break;
            case WarriorActon.eRunAway:
                //GetComponent<WarriorRunAway>().enabled = true;
                break;
            case WarriorActon.eAlert:
                //GetComponent<WarriorAlert>().enabled = true;
                break;
            case WarriorActon.eIdle:
                //GetComponent<WarriorPatrol>().enabled = false;
                //GetComponent<WarriorAttack>().enabled = false;
                //GetComponent<WarriorRunAway>().enabled = false;
                break;
        }
    }  
}
