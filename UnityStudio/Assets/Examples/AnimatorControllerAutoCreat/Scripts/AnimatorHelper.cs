using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour {

    public static void Turn(Animator runtimeAnimator, string overrideClip)
    {
        AnimatorOverrideController overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = runtimeAnimator.runtimeAnimatorController;
        AnimationClip newClip = AssetDatabase.LoadAssetAtPath("Assets/Examples/AnimatorControllerAutoCreat/Resources/Character/DefaultAvatar@HoldLog.FBX", typeof(AnimationClip)) as AnimationClip;
        overrideController[overrideClip] = newClip;
        runtimeAnimator.runtimeAnimatorController = overrideController;

    }
}
