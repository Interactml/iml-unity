using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurpriseState : BehaviourState
{
    public string DebugMessage;
    //public AgentGazeIK AgentGazeIKScript;
    //public GameObject LookAtObject;

    public override void StateLogic()
    {
       
        Debug.Log(DebugMessage); 
        agentAnimator.SetTrigger("01Surprise");
        //AgentGazeIKScript.updateObjLookAt()
        base.StateLogic();
    }
}
