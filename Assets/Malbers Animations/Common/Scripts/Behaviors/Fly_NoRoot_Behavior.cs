using UnityEngine;
using System.Collections;
using System;

namespace MalbersAnimations
{
    //Fly Animations are inPlace
    public class Fly_NoRoot_Behavior : StateMachineBehaviour
    {
        [Range(0, 90)]
        [Tooltip("Adds Banking to the Fly animation when turning")]
        public float Bank = 30;
        [Range(0, 90)]
        [Tooltip("Top Angle the Animal Can go UP or Down ")]
        public float Ylimit = 80;
        public float Drag = 5;

        [Space]
        public bool UseDownAcceleration = true;               //Accelerates if going Down
        public float DownAcceleration = 3;
        public float FallRecovery = 1.5f;

        [Space]
        public bool CanNotSwim = false;

        protected float acceleration = 0;
        protected Rigidbody rb;
        protected Animal animal;
        protected Transform transform;
        //protected Quaternion DeltaRotation;
        protected float Shift;
        protected float Direction;
        protected float deltaTime;

        Vector3 FallVector;
        protected float forwardAceleration;

        protected Speeds BehaviourSpeed;
        private float PitchAngle;
        private float vertical;
        private bool foundWater;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ResetAllValues();

            rb = animator.GetComponent<Rigidbody>();
            animal = animator.GetComponent<Animal>();
            BehaviourSpeed = animal.flySpeed;

            animal.RootMotion = true;

            transform = animator.transform;                                                             //Save the Transform on a local variable
                                                                                                        // DeltaRotation = transform.rotation;

            acceleration = 0;
            vertical = animal.Speed;

            FallVector =
                animal.CurrentAnimState == AnimTag.Fall || animal.CurrentAnimState == AnimTag.Jump ?
                rb.velocity : Vector3.zero;          //Just recover if your coming from the fall animations

            rb.constraints = RigidbodyConstraints.FreezeRotation;                                       //Release the Y Constraint
                                                                                                        // rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);                                 //Clean the Y velocity
            rb.useGravity = false;
            rb.drag = Drag;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!animal.Fly) return;

            float TransitionNormalizedTime = 1;

            if (animator.IsInTransition(layerIndex) && stateInfo.normalizedTime < 0.5f)     //If is in the First Transition
                TransitionNormalizedTime = animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime;

            if (animator.IsInTransition(layerIndex) && stateInfo.normalizedTime > 0.5f)     //If is in the Last Transition
                TransitionNormalizedTime = 1 - animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime;

            deltaTime = Time.deltaTime; //Store the Delta Time

            var CleanEuler = (transform.eulerAngles);
            CleanEuler.x = CleanEuler.z = 0;

            transform.eulerAngles = CleanEuler;                                             //Reset the Rotation before rotating... with just Yaw Rotation

            float isGoingForward = animal.MovementAxis.z >= 0 ? 1 : -1;                     //Check if the animal is going Forward or Backwards

            Direction = Mathf.Lerp(Direction, Mathf.Clamp(animal.Direction, -1, 1), deltaTime * BehaviourSpeed.lerpRotation);          //Calculate the direction

            var RotationYaw = new Vector3(0, Direction * BehaviourSpeed.rotation * isGoingForward, 0);

            Quaternion Yaw = Quaternion.Euler(transform.InverseTransformDirection(RotationYaw));            //Get the Rotation on the Y Axis transformed to Quaternion


            //***THE ANIMAL ALREADY TAKE CARE FO THE YAW ROTATIONs***/
            animal.DeltaRotation *= Yaw;                                                                      //Rotation ROLL USING DeltaRotation (GOOD FOR PERFORMACE)
            // transform.rotation *= Yaw;                                                                       //Rotation ROLL USING ROTATE (BAD FOR PERFORMACE)

            //DeltaRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * rb.rotation;            //Restore the rotation to the Default after turning USING ROTATE ...             
            //DeltaRotation = Quaternion.FromToRotation(transform.up, Vector3.up) * rb.rotation * Yaw;        //Restore the rotation to the Default after turning USING DeltaRotation ...

            float UpDown = animal.MovementUp;

            vertical = Mathf.Lerp(vertical, Mathf.Clamp(animal.Speed, -1, 1), deltaTime * 6);

            Vector3 DirectionVector = Vector3.zero;
            Vector3 movement = animal.T_Forward;                    //Set the Movement as the Stored Animal Forward Direction



            if (animal.DirectionalMovement)                         //If the Animal is using Directional Movement use the Raw Direction Vector
            {
                DirectionVector = animal.RawDirection;
                if (animal.IgnoreYDir) DirectionVector.y = 0;
                DirectionVector.Normalize();

                DirectionVector += (transform.up * UpDown);
                if (DirectionVector.magnitude > 1) DirectionVector.Normalize();                          //Remove extra Speed
            }
            else                                                                                         //If not is using Directional Movement Calculate New Direction Vector
            {
                DirectionVector = (transform.forward * vertical) + (transform.up * UpDown);              //Calculate the Direction to Move
                if (DirectionVector.magnitude > 1) DirectionVector.Normalize();                          //Remove extra Speed
                if (animal.MovementAxis.z < 0) UpDown = 0;                                               //Remove UP DOWN MOVEMENT while going backwards
                movement = DirectionVector;
            }

            forwardAceleration = Mathf.Lerp(forwardAceleration, DirectionVector.magnitude, deltaTime * BehaviourSpeed.lerpPosition);

            var DeltaMovement =
                movement *                                      //Apply the movement to the Forward Direction of the Animal
                forwardAceleration *                            //This is to avoid going forward all the time even if no key is pressed
                BehaviourSpeed.position *                       //The Fly Velocity multiplier
                (animal.Speed < 0 ? 0.5f : 1) *                 //If the animal is going backwards go half speed;
                deltaTime;                                      //DeltaTime

            DeltaMovement = Vector3.Lerp(Vector3.zero, DeltaMovement, TransitionNormalizedTime);

            if (CanNotSwim)
            {
                RaycastHit WaterHitCenter;
                // Debug.DrawRay(animal.Main_Pivot_Point, -Vector3.up * animal.Pivot_Multiplier * animal.ScaleFactor * animal.FallRayMultiplier, Color.green);

                if (Physics.Raycast(animal.Main_Pivot_Point, -Vector2.up, out WaterHitCenter, animal.Pivot_Multiplier * animal.ScaleFactor * animal.FallRayMultiplier, 16)) //16 Water Layer
                {
                    foundWater = true;
                }
                else
                {
                    foundWater = false;
                }
            }

            if (foundWater && DeltaMovement.y < 0)
            {
                DeltaMovement.y = 0.001f;
                animal.DeltaPosition.y = 0f;
                animal.MovementUp = 0;
            }

            animal.DeltaPosition += DeltaMovement;

            if (animal.debug) Debug.DrawRay(transform.position, DirectionVector * 2, Color.yellow);


            if (DirectionVector.magnitude > 0.001)                                              //Rotation PITCH
            {
                float NewAngle = 90 - Vector3.Angle(Vector3.up, DirectionVector);

                float smooth = Mathf.Max(Mathf.Abs(animal.MovementAxis.y), Mathf.Abs(vertical));

                NewAngle = Mathf.Clamp(-NewAngle, -Ylimit, Ylimit);

                PitchAngle = Mathf.Lerp(PitchAngle, NewAngle, deltaTime * animal.upDownSmoothness * 2);

                //transform.Rotate(Mathf.Clamp(PitchAngle, -Ylimit, Ylimit) * smooth , 0, 0, Space.Self);           //Rotation PITCH USING ROTATE (BAD FOR PERFORMACE)
                animal.DeltaRotation *= Quaternion.Euler(PitchAngle * smooth * TransitionNormalizedTime, 0, 0);      //Rotation PITCH USING DeltaRotation (GOOD FOR PERFORMACE)
            }

            //transform.Rotate(0, 0, -Bank * Mathf.Clamp(Direction, -1, 1), Space.Self);                //Rotation Bank USING ROTATE (BAD FOR PERFORMACE)
            animal.DeltaRotation *= Quaternion.Euler(0, 0, -Bank * Direction);                           //Rotation Bank USING DeltaRotation (GOOD FOR PERFORMACE)


            if (foundWater) return;

            if (FallVector != Vector3.zero)                              //if last animation was falling 
            {
                animal.DeltaPosition += FallVector * deltaTime;          //Add Recovery from falling
                FallVector = Vector3.Lerp(FallVector, Vector3.zero, deltaTime * FallRecovery);
            }

            if (UseDownAcceleration) GravityAcceleration(DirectionVector);
        }
        private void GravityAcceleration(Vector3 DirectionVector)
        {
            //Add more speed when going Down
            if (animal.MovementAxis.y < -0.1)
            {
                acceleration = Mathf.Lerp(acceleration, acceleration + DownAcceleration, deltaTime);
            }
            else
            {
                float a = acceleration - DownAcceleration;
                if (a < 0) a = 0;

                acceleration = Mathf.Lerp(acceleration, a, deltaTime);  //Deacelerate slowly all the acceleration you earned..
            }

            animal.DeltaPosition += DirectionVector * (acceleration * deltaTime);
        }

        private void ResetAllValues()
        {
            deltaTime = acceleration = forwardAceleration = PitchAngle = Direction = 0;
        }
    }
}