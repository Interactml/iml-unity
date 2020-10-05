using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourState : MonoBehaviour
{
    public BehaviourState NextState;
    public BehaviourState LowScoreNextState;
    public string AnimationClipName;
    public AudioClip mainClip;
    public float timer;
    public float maxTime;

    private AudioSource MonologueAudioSource;
    protected Animator agentAnimator;

    private void Awake()
    {
        agentAnimator = GameObject.Find("AgentFemale").GetComponent<Animator>();
        maxTime = mainClip.length;//default is 10
        MonologueAudioSource = GameObject.Find("Monologue").GetComponent<AudioSource>();

    }
    public virtual void StateLogic()
    {
        //CheckEndOfState();
        timer = 0;
        MonologueAudioSource.clip = mainClip;
        MonologueAudioSource.Play();
        //int seconds = timer % 60;
    }

    public void StopTalking()
    {
        //Talking = false;
        //Debug.Log("Agent has stopped talking");
        agentAnimator.SetLayerWeight(1, 0.1f);

    }

    public void ContinueTalking()
    {
        //Talking = true;
        //Debug.Log("Agent has started talking again");
        agentAnimator.SetLayerWeight(1, 1f);

    }


}
