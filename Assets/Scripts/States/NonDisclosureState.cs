﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonDisclosureState : BehaviourState
{
    public string DebugMessage;
    public Animator BusAnimator;

    public override void StateLogic()
    {   
        Debug.Log(DebugMessage);
        agentAnimator.SetTrigger("05NonDisclosure_low");
        if (BusAnimator != null)
        {
            BusAnimator.SetTrigger("StartBus");
        }
        base.StateLogic();
    }
}