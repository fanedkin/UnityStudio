using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
public class AnimatorTest : MonoBehaviour {

    Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void OnGUI()
    {
        if (GUILayout.Button("Dying"))
        {
            animator.Play("Dying");
        }
        if (GUILayout.Button("HoldLog"))
        {
            animator.Play("HoldLog");
        }
        if (GUILayout.Button("Idle_Neutral"))
        {
            animator.Play("Idle_Neutral");
        }
        if (GUILayout.Button("DyingTurn"))
        {
            AnimatorHelper.Turn(animator, "Dying");
        }
    }
}
