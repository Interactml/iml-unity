using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public BehaviourState CurrentState;
    public float CurrentScore;
    public float LowScoreThreshold;
    public bool Talking;

    //these are from when the user stop talking
    public AgentGazeIK AgentGazeIKScript;
    public GameObject LookAtObjectWhenSilenced;
    public float LightIntensity;

    private void Start()
    {
        CurrentState.StateLogic();
        LightIntensity = 0.3f;
    }

    private void Update()
    {
        if(Talking) CurrentState.timer+= Time.deltaTime;
        if (CurrentState.timer>=CurrentState.maxTime)
        {
            //if is lower than the threshold and the agent will stop talking and will also look at something else.
            if (CurrentScore < LowScoreThreshold / 2)
            {
                CurrentState.StopTalking();
                AgentGazeIKScript.updateObjLookAt(LookAtObjectWhenSilenced.transform);
                Talking = false;
            }

            if (Talking)
            {
                if (CurrentScore < LowScoreThreshold) GoToLowScoreNextState();
                else
                {
                    GoToNextState();
                    LightIntensity += 0.08f;
                    if (LightIntensity > 1f)
                    {
                        LightIntensity = 1f;
                    }
                }

                CurrentState.StateLogic();

            }

            //if the agent was not talking and the score is higher or equal to the threshold, than continue talking from where it left and shifts the gaze to the player
            if (!Talking && CurrentScore >= LowScoreThreshold / 2)
            { CurrentState.ContinueTalking();
              AgentGazeIKScript.updateObjLookAt(Camera.main.transform);
              Talking = true;
            }
        }

    }

    public void GoToNextState()
    {
        if(CurrentState.NextState != null)
        {
            CurrentState = CurrentState.NextState;
        }
    }


    public void GoToLowScoreNextState()
    {
        if (CurrentState.LowScoreNextState != null)
        {
            CurrentState = CurrentState.LowScoreNextState;
        }
    }
}
