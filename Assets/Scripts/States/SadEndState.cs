using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SadEndState : BehaviourState
{
    public string DebugMessage;


    public override void StateLogic()
    {
        Debug.Log(DebugMessage);
        agentAnimator.SetTrigger("06SadEnd_low");

        base.StateLogic();
    }
}
