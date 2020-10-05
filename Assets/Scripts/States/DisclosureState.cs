using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisclosureState : BehaviourState
{
    public string DebugMessage;


    public override void StateLogic()
    {
        Debug.Log(DebugMessage);
        agentAnimator.SetTrigger("05Disclosure_high");

        base.StateLogic();
    }
}
