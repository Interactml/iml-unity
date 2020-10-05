using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionState : BehaviourState
{
    public string DebugMessage;
    public Animator PramAnimator;


    public override void StateLogic()
    {
        Debug.Log(DebugMessage);
        agentAnimator.SetTrigger("02Description");
        if (PramAnimator != null)
        {
            PramAnimator.SetTrigger("StartPram");
        }

        base.StateLogic();
    }
}
