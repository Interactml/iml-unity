using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnxiousState : BehaviourState
{
    public string DebugMessage;

    public override void StateLogic()
    {
        Debug.Log(DebugMessage);
        agentAnimator.SetTrigger("04Anxious_low");

        base.StateLogic();
    }
}
