using UnityEngine;

namespace MalbersAnimations
{
    public class FlyBehavior : StateMachineBehaviour
    {
        #region Variables
        public float Drag = 5;

        public float DownAcceleration = 4;

        [Tooltip("If is changing from ")]
        public float DownInertia = 2;
        [Tooltip("If is changing from fall to fly this will smoothly ")]
        public float FallRecovery = 1.5f;
        [Tooltip("If Lock up is Enabled this apply to the dragon an extra Down Force")]
        public float LockUpDownForce = 4;

        float acceleration = 0;
        Rigidbody rb;
        Animal animal;
        float deltaTime;
        #endregion

        Vector3 FallVector;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rb = animator.GetComponent<Rigidbody>();            //Get the RB
            animal = animator.GetComponent<Animal>();           //Get the Animal
            acceleration = 0;
            animal.IsInAir = true;
            animal.RootMotion = true;

            FallVector = animal.CurrentAnimState == AnimTag.Fall ? rb.velocity : Vector3.zero;          //Just recover if your coming from the fall animations

            rb.constraints = RigidbodyConstraints.FreezeRotation;                                       //Release the Y Constraint
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);                                 //Clean the Y velocity
            rb.useGravity = false;
            rb.drag = Drag;

        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            deltaTime = Time.deltaTime;     //Get the Time Right

            if (FallVector != Vector3.zero)                         //if last animation was falling 
            {
                animal.DeltaPosition += FallVector * deltaTime;          //Add Recovery from falling
                FallVector = Vector3.Lerp(FallVector, Vector3.zero, deltaTime * FallRecovery);
            }

            //Add more speed when going Down
            if (animal.MovementAxis.y < -0.1)
            {
                acceleration = Mathf.Lerp(acceleration, acceleration + DownAcceleration, deltaTime);

            }
            else if (animal.MovementAxis.y > -0.1 || animal.MovementReleased)
            {
                float a = acceleration - DownInertia;
                if (a < 0) a = 0;

                acceleration = Mathf.Lerp(acceleration, a, deltaTime * 2);                  //Deacelerate slowly all the acceleration you earned..
            }

            animal.DeltaPosition += animator.velocity  * (acceleration/2) * deltaTime;

            if (animal.LockUp)
            {
                animal.DeltaPosition += Physics.gravity * LockUpDownForce * deltaTime * deltaTime;
            }
        }
    }
}