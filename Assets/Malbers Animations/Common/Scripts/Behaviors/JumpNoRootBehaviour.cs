using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MalbersAnimations
{
    public class JumpNoRootBehaviour : StateMachineBehaviour
    {
        public float JumpMultiplier = 1;
        public float ForwardMultiplier = 0;

        Animal animal;
        Rigidbody rb;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal = animator.GetComponent<Animal>();
            rb = animator.GetComponent<Rigidbody>();

            animal.InAir(true);
            animal.SetIntID(0);
            animal.OnJump.Invoke();     //Invoke that the Animal is Jumping

            animal.RootMotion = false;

            Vector3 JumpVector = (Vector3.up * JumpMultiplier * animal.JumpHeightMultiplier) + (animator.transform.forward * ForwardMultiplier * animal.AirForwardMultiplier);

            rb.AddForce(JumpVector, ForceMode.VelocityChange); //Jump Up
        }

        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal.SetIntID(0);

            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(layerIndex);

            //if the next animation is fly then clean the rigidbody velocity on the Y axis
            if (rb && currentState.tagHash == AnimTag.Fly)
            {
                Vector3 cleanY = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.velocity = cleanY;
            }

            if (currentState.tagHash != AnimTag.Fall && currentState.tagHash != AnimTag.Fly)
            {
                animal.IsInAir = false;
            }
        }
    }
}