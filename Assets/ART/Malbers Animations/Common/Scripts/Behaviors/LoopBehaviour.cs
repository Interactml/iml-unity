using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MalbersAnimations
{
    /// <summary>
    /// This Animator behaviour is used to make loops on animal animations depending the Loop parameter...(Loop parameter is accessible by the Loops Property)
    /// </summary>
    public class LoopBehaviour : StateMachineBehaviour {

        [Header("This behaviour requires a transition to itself")]
        [Header("With the contidion 'IntID' != -1")]
        public int IntIDExitValue =-1;
        [Header("")]
        protected int CurrentLoop = 0;
        protected int loop = 0;
        bool hasEntered = false;
        Animal animal;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal = animator.GetComponent<Animal>();               //Get the reference for the animator

            if (animal == null) return;

            if (!hasEntered)
            {
                hasEntered = true;
                CurrentLoop = 1;
                animal.SetIntID(0);            //Set the exit value
            }
            else
            {
                CurrentLoop++;
            }   

            if (CurrentLoop >= animal.Loops)
            {
                hasEntered = false;
                animal.SetIntID(IntIDExitValue);            //Set the exit value
            }
        }
    }
}