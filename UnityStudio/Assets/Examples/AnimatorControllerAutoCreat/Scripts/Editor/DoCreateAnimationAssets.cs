using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;

public class DoCreateAnimatorWindow : EditorWindow
{
    [MenuItem("MyTools/CreateAnimator")]
    static void Open()
    {
        GetWindow<DoCreateAnimatorWindow>();
    }
    //序列化对象
    protected SerializedObject mSerializedObject;
    //序列化属性
    protected SerializedProperty mAnimationClipListProperty;

    AnimatorController mAnimatorController;
    [SerializeField]
    List<AnimationClip> mAnimationClipList=new List<AnimationClip>();
    protected void OnEnable()
    {
        //使用当前类初始化
        mSerializedObject = new SerializedObject(this);
        //获取当前类中可序列话的属性
        mAnimationClipListProperty = mSerializedObject.FindProperty("mAnimationClipList");
    }

    void OnGUI()
    {
        //更新
        mSerializedObject.Update();
        //开始检查是否有修改
        EditorGUI.BeginChangeCheck();

        mAnimatorController = EditorGUILayout.ObjectField(mAnimatorController, typeof(AnimatorController), false) as AnimatorController;

        EditorGUILayout.PropertyField(mAnimationClipListProperty, true);
        if (GUILayout.Button("CreateAnimator"))
        {
            CreateAnimator();
        }


        //结束检查是否有修改
        if (EditorGUI.EndChangeCheck())
        {
            mSerializedObject.ApplyModifiedProperties();//提交修改
        }

    }
    void CreateAnimator()
    {
        //得到它的Layer， 默认layer为base 你可以去拓展
        AnimatorControllerLayer layer = mAnimatorController.layers[0];

        for (int i = 0; i < mAnimationClipList.Count; i++)
        {
            AddStateTransition(mAnimationClipList[i], layer);
        }
    }

    private static void AddStateTransition(AnimationClip clip, AnimatorControllerLayer layer)
    {
        AnimatorStateMachine sm = layer.stateMachine;

        //取出动画名子 添加到state里面
        AnimatorState state = sm.AddState(clip.name);
        state.motion = clip;
        ////把state添加在layer里面
        AnimatorStateTransition trans = sm.AddAnyStateTransition(state);
        ////把默认的时间条件删除
    }
    //[MenuItem("MyTools/CreateAnimator2")]
    private static void CreateAnimator2()
    {
        // Creates the controller
        var controller =
            UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(
                "Assets/Mecanim/StateMachineTransitions.controller");

        // Add parameters
        controller.AddParameter("TransitionNow", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Reset", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("GotoB1", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("GotoC", AnimatorControllerParameterType.Trigger);

        // Add StateMachines
        var rootStateMachine = controller.layers[0].stateMachine;
        var stateMachineA = rootStateMachine.AddStateMachine("smA");
        var stateMachineB = rootStateMachine.AddStateMachine("smB");
        var stateMachineC = stateMachineB.AddStateMachine("smC");

        // Add States
        var stateA1 = stateMachineA.AddState("stateA1");
        var stateB1 = stateMachineB.AddState("stateB1");
        var stateB2 = stateMachineB.AddState("stateB2");
        stateMachineC.AddState("stateC1");
        var stateC2 = stateMachineC.AddState("stateC2");
        // don’t add an entry transition, should entry to state by default

        // Add Transitions
        var exitTransition = stateA1.AddExitTransition();
        exitTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "TransitionNow");
        exitTransition.duration = 0;

        var resetTransition = rootStateMachine.AddAnyStateTransition(stateA1);
        resetTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Reset");
        resetTransition.duration = 0;

        var transitionB1 = stateMachineB.AddEntryTransition(stateB1);
        transitionB1.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "GotoB1");
        stateMachineB.AddEntryTransition(stateB2);


        stateMachineC.defaultState = stateC2;
        var exitTransitionC2 = stateC2.AddExitTransition();
        exitTransitionC2.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "TransitionNow");
        exitTransitionC2.duration = 0;

        var stateMachineTransition = rootStateMachine.AddStateMachineTransition(stateMachineA, stateMachineC);
        stateMachineTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "GotoC");
        rootStateMachine.AddStateMachineTransition(stateMachineA, stateMachineB);
    }
}