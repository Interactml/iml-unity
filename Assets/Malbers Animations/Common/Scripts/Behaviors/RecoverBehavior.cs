using UnityEngine;
using System.Collections;

namespace MalbersAnimations
{
    /// <summary>
    /// This Behavior Updates and resets all parameters to their original state
    /// </summary>
    public class RecoverBehavior : StateMachineBehaviour
    {
        public float smoothness = 10;
        public float MaxDrag = 3;
        public bool stillContraints = true;
        public bool Landing = true;
        public bool RigidY = true;
        Animal animal;
        Rigidbody rb;
        float deltatime;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal = animator.GetComponent<Animal>();                           //Get Reference for Animal
            rb = animator.GetComponent<Rigidbody>();                            //Get Reference for Rigid Body

            animal.RootMotion = false;

            if (RigidY) rb.constraints = Animal.Still_Constraints;

            //if we are landing on the Ground Set that is not longer on the air

            rb.drag = 0;

            if (Landing)
                animal.IsInAir = false;
            else
                rb.useGravity = false;

        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rb.drag = Mathf.Lerp(rb.drag, MaxDrag, Time.deltaTime * smoothness);



            //if (stateInfo.normalizedTime < 0.9f)   //Smooth Stop when RecoverFalls
            //{
            //    rb.drag = Mathf.Lerp(rb.drag, MaxDrag, deltatime * smoothness);
            //}
            //else
            //{

            //    animator.applyRootMotion = true;
            //}
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //int animTag = animator.GetCurrentAnimatorStateInfo(layerIndex).tagHash;

            //if (animTag != AnimTag.Fall && animTag != AnimTag.Underwater && animTag != AnimTag.Fly) //if the next animation is not fall then do the next code
            //                                                                                         if (animTag == AnimTag.Locomotion || animTag == AnimTag.Idle) //if the next animation is not fall then do the next code
            //{
            //    animator.applyRootMotion = true;
            //    rb.constraints = Animal.Still_Constraints;
            //}
            rb.drag = 0; //Reset the Drag
        }
    }
}