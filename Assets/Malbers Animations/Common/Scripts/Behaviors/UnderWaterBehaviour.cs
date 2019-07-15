using UnityEngine;

namespace MalbersAnimations
{
    //Controls the movement while underwater
    public class UnderWaterBehaviour : StateMachineBehaviour
    {

        [Range(0, 90)]
        public float Bank;
        [Range(0, 90)]
        public float Ylimit = 87;

        [Space]
        public bool useShift = true;
        public float ShiftMultiplier = 2f;
        [Space]

        protected Rigidbody rb;
        protected Animal animal;
        protected Transform transform;
        protected float Shift;
        protected float deltaTime;

        Speeds Speed;

        int WaterLayer;
        private float Direction;
        private float forwardAceleration;

        public float PitchAngle { get; private set; }
        public bool Default_UseShift { get; private set; }


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ResetAllValues();

            rb = animator.GetComponent<Rigidbody>();
            animal = animator.GetComponent<Animal>();

            animal.RootMotion = true;

            transform = animator.transform;                                                             //Save the Transform on a local variable

            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.useGravity = false;


            Default_UseShift = animal.UseShift;
            animal.UseShift = false;

            WaterLayer = LayerMask.GetMask("Water");
            Speed = animal.underWaterSpeed;
        }

        void ResetAllValues()
        {
            Shift = 0;
            deltaTime = 0;
            Direction = 0;
            forwardAceleration = 0;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!animal.CanGoUnderWater || !animal.Underwater) return;              //Stop Moving if the animal is not going underwater anymore

            float TransitionNormalizedTime = 1;
            if (animator.IsInTransition(layerIndex) && stateInfo.normalizedTime < 0.5f)     //If is in the First Transition
                TransitionNormalizedTime = animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime;

            if (animator.IsInTransition(layerIndex) && stateInfo.normalizedTime > 0.5f)     //If is in the Second Transition
                TransitionNormalizedTime = 1 - animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime;

            deltaTime = Time.deltaTime;                                                     //Store the Delta Time

            if (useShift)
                Shift = Mathf.Lerp(Shift, animal.Shift ? ShiftMultiplier : 1, Speed.lerpPosition * deltaTime);   //Calculate the Shift

            if (animal.Up) animal.Down = false;       //Cannot press at the same time Down and UP(Jump)

            var CleanEuler = (transform.eulerAngles);
            CleanEuler.x = CleanEuler.z = 0;
            transform.eulerAngles = CleanEuler;                                             //Reset the Rotation before rotating... with just Yaw Rotation

            //animal.YAxisMovement(animal.upDownSmoothness, deltaTime);


            float isGoingForward = animal.MovementAxis.z >= 0 ? 1 : -1;                                           //Check if the animal is going Forward or Backwards
            Direction = Mathf.Lerp(Direction, Mathf.Clamp(animal.Direction, -1, 1), deltaTime * Speed.lerpRotation);          //Calculate the direction

            var RotationYaw = new Vector3(0, Direction * Speed.rotation * isGoingForward, 0);
            Quaternion Yaw = Quaternion.Euler(transform.InverseTransformDirection(RotationYaw));            //Get the Rotation on the Y Axis transformed to Quaternion

            // transform.rotation *= Yaw;                                                                   //Rotation ROLL USING ROTATE (BAD FOR PERFORMACE)
            animal.DeltaRotation *= Yaw;                                                                    //Rotation ROLL USING DeltaRotation (GOOD FOR PERFORMACE)

            float Up = animal.MovementUp;

            float forwardSpeed = Mathf.Clamp(animal.Speed, -1, 1);

            Vector3 DirectionVector = Vector3.zero;
            Vector3 movement = animal.T_Forward;                    //Set the Movement as the Stored Animal Forward Direction

            if (animal.DirectionalMovement)                         //If the Animal is using Directional Movement use the Raw Direction Vector
            {
                DirectionVector = animal.RawDirection;
                DirectionVector += (transform.up * Up);
            }
            else                                                                                         //If not is using Directional Movement Calculate New Direction Vector
            {
                DirectionVector = (transform.forward * forwardSpeed) + (transform.up * Up);              //Calculate the Direction to Move
                if (DirectionVector.magnitude > 1) DirectionVector.Normalize();                          //Remove extra Speed
                if (animal.MovementAxis.z < 0) Up = 0;                                                   //Remove UP DOWN MOVEMENT while going backwards
                movement = DirectionVector;
            }

            forwardAceleration = Mathf.Lerp(forwardAceleration, DirectionVector.magnitude, deltaTime * Speed.lerpPosition);

            var DeltaMovement =
                movement *                                      //Apply the movement to the Forward Direction of the Animal
                forwardAceleration *                            //This is to avoid going forward all the time even if no key is pressed
                Speed.position *                             //The Fly Velocity multiplier
                Shift *                                         //The Sprint mulitplier
                (animal.Speed < 0 ? 0.5f : 1) *                 //If the animal is going backwards go half speed;
                deltaTime;                                      //DeltaTime

            DeltaMovement = Vector3.Lerp(Vector3.zero, DeltaMovement, TransitionNormalizedTime);


            animal.DeltaPosition += DeltaMovement;


            if (DirectionVector.magnitude > 0.001)                                              //Rotation PITCH
            {
                float NewAngle = 90 - Vector3.Angle(Vector3.up, DirectionVector);

                float smooth = Mathf.Max(Mathf.Abs(animal.MovementAxis.y), Mathf.Abs(forwardSpeed));

                NewAngle = Mathf.Clamp(-NewAngle, -Ylimit, Ylimit);

                PitchAngle = Mathf.Lerp(PitchAngle, NewAngle, deltaTime * animal.upDownSmoothness*2);

                transform.Rotate(Mathf.Clamp(PitchAngle, -Ylimit, Ylimit) * smooth , 0, 0, Space.Self);         //Rotation PITCH USING ROTATE (BAD FOR PERFORMACE)
                //animal.DeltaRotation *= Quaternion.Euler(PitchAngle * smooth, 0, 0);                          //Rotation PITCH USING DeltaRotation (GOOD FOR PERFORMACE)
            }

            if (animal.debug) Debug.DrawRay(transform.position, DirectionVector * 2, Color.yellow);

            transform.Rotate(0, 0, -Bank * Mathf.Clamp(Direction, -1, 1), Space.Self);                //Rotation Bank USING ROTATE (BAD FOR PERFORMACE)
            //animal.DeltaRotation *= Quaternion.Euler(0, 0, -Bank * Mathf.Clamp(Direction, -1, 1));      //Rotation Bank USING DeltaRotation (GOOD FOR PERFORMACE)

            CheckExitUnderWater();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal.UseShift = Default_UseShift;
        }

        protected void CheckExitUnderWater()
        {
            //To Get Out of the Water---------------------------------
            RaycastHit UnderWaterHit;

            Vector3 origin = transform.position + new Vector3(0, (animal.height - animal.waterLine) * animal.ScaleFactor, 0);

            if (Physics.Raycast(origin, -Vector3.up, out UnderWaterHit, animal.ScaleFactor, WaterLayer))
            {
                if (!animal.Down)
                {
                    animal.Underwater = false;
                    animal.RootMotion = true;
                    rb.useGravity = true;
                    rb.drag = 0;

                    rb.constraints = Animal.Still_Constraints;

                    animal.MovementAxis = new Vector3(animal.MovementAxis.x, 0, animal.MovementAxis.z);
                }
            }
        }
    }
}