using UnityEngine;

namespace MalbersAnimations
{
    public class FlyDodgeBehaviour : StateMachineBehaviour
    {
        public bool InPlace;
        public Vector3 DodgeDirection = Vector3.zero;
        Vector3 momentum;                                //To Store the velocity that the animator had before entering this animation state
        Rigidbody rb;
        Animal animal;
        float time;

        float multiplier;
        AnimatorTransitionInfo transition;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rb = animator.GetComponent<Rigidbody>();
            animal = animator.GetComponent<Animal>();

            momentum = InPlace ? rb.velocity : animator.velocity;

        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            time = animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;     //Get the Time Right

            multiplier = 1;

            if (animator.IsInTransition(layerIndex))
            {
                transition = animator.GetAnimatorTransitionInfo(layerIndex);

                multiplier = stateInfo.normalizedTime <= 0.5f ? transition.normalizedTime : 1 - transition.normalizedTime;  //Smooh out the Movement

            }


            animal.DeltaPosition += momentum * time * multiplier;
            animal.DeltaPosition += animal.transform.TransformDirection(DodgeDirection) * time * multiplier;
        }
    }
}