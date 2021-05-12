using UnityEngine;
using System.Collections;

public class NoCollidersBehavior : StateMachineBehaviour
{
    [Header("Deactivate Colliders on Enter")]
    public bool enter = true;  
    [Header("Activate Colliders on Exit")]
    public bool exit = true;
    Collider[] cap;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        cap = animator.GetComponentsInChildren<Collider>();
        if (enter)
        {
            foreach (Collider item in cap) item.enabled = false;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (exit)
        {
            foreach (Collider item in cap) item.enabled = true;
        }
    }
}