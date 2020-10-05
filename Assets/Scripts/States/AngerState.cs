using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngerState : BehaviourState
{
    public string DebugMessage;


    public override void StateLogic()
    {
        Debug.Log(DebugMessage);
        agentAnimator.SetTrigger("04Angry_high");

        base.StateLogic();
    }
}
