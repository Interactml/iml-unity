using MalbersAnimations.Utilities;
using UnityEngine;


namespace MalbersAnimations
{
    /// <summary>
    /// Controls the Logic for RootMotion Jumps
    /// </summary>
    public class JumpBehaviour : StateMachineBehaviour
    {

        #region Var
        /// <summary>Ray Length to check if the ground is at the same level all the time</summary>
        [Header("Checking Fall")]
        [Tooltip("Ray Length to check if the ground is at the same level all the time")]
        public float fallRay = 1.7f;

        [Tooltip("Terrain difference to be sure the animal will fall ")]
        public float stepHeight = 0.1f;

        [Tooltip("Min Distance to land and End the Jump")]
        public float MinJumpLand = 0f;

        [Tooltip("Animation normalized time to change to fall animation if the ray checks if the animal is falling ")]
        [Range(0,1)]
        public float willFall = 0.7f;

        [Header("Jump on Higher Ground")]
        [Tooltip("Range to Calcultate if we can land on Higher ground")]
        [MinMaxRange(0, 1)]
        public RangedFloat Cliff = new RangedFloat(0.5f, 0.65f);
        public float CliffRay = 0.6f;
       

        [Space]
        [Header("Add more Height and Distance to the Jump")]
        public float JumpMultiplier = 1;
        public float ForwardMultiplier = 1;

        [Space]
        [Header("Double Jump")]
        [Tooltip("Enable the Double Jump after x normalized time of the animation")]
        [Range(0, 1)]
        public float DoubleJumpTime = 0.33f;

        private Animal animal;
        private Rigidbody rb;
        private Transform transform;

        /// <summary>Height multipliers are > 0 and Foward Multipliers >0 </summary>
        private bool Can_Add_ExtraJump;
        private Vector3 ExtraJump;

        private bool JumpPressed;
        private float jumpPoint;
        private float Rb_Y_Speed = 0;
        private RaycastHit JumpRay;

        private float JumpSmoothPressed = 1;
        private bool JumpEnd;
        private bool cast_WillFall_Ray;
        #endregion

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal = animator.GetComponent<Animal>();
            rb = animator.GetComponent<Rigidbody>();
            transform = animator.transform;

            animal.RootMotion = true;

            animal.IsInAir = true;

            jumpPoint = transform.position.y;                           //Store the Heigh of the jump

            animal.InAir(true);
            animal.SetIntID(0);                                         //Reset the INT_ID

            animal.OnJump.Invoke();                                     //Invoke that the Animal is Jumping

            Rb_Y_Speed = 0;                                             //For Flying

            cast_WillFall_Ray = false;                                            //Reset Values IMPROTANT
            var PlanarRawDirection = animal.RawDirection;
            PlanarRawDirection.y = 0;
            animal.AirControlDir = PlanarRawDirection;
           

           //----------------------------------------------------------------------------------------
            #region Jump Multiplier Start

            Can_Add_ExtraJump = (JumpMultiplier > 0 && animal.JumpHeightMultiplier > 0) || (ForwardMultiplier > 0 && animal.AirForwardMultiplier > 0);
            ExtraJump = ((Vector3.up * JumpMultiplier * animal.JumpHeightMultiplier) + (animal.T_ForwardNoY * ForwardMultiplier * animal.AirForwardMultiplier));

            JumpSmoothPressed = 1;
            JumpPressed = true;

            if (animal.JumpPress)
            {
                Can_Add_ExtraJump = JumpPressed = animal.Jump;  //if you release the Jump Input you cannot add more extra jump
            }
            #endregion
            JumpEnd = false;
            animator.SetFloat(Hash.IDFloat, 1);
        }


        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            bool isInTransition = animator.IsInTransition(layerIndex);
            bool isInLastTransition = isInTransition && stateInfo.normalizedTime > 0.5f;

            if (animal.AnimState != AnimTag.Jump ) return;       //Do this while is on the Jump State ... else Ignore it


            #region if is transitioning to flying
            //If the next animation is FLY smoothly remove the Y rigidbody speed
            if (rb && isInLastTransition && animator.GetNextAnimatorStateInfo(layerIndex).tagHash == AnimTag.Fly)
            {
                float transitionTime = animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime;
                Vector3 cleanY = rb.velocity;

                if (Rb_Y_Speed < cleanY.y) Rb_Y_Speed = cleanY.y;                       //Get the max Y SPEED

                cleanY.y = Mathf.Lerp(Rb_Y_Speed, 0, transitionTime);

                rb.velocity = cleanY;
            }
            #endregion

            if (isInLastTransition) return;   //Ignore if is in the first transition
         

            if (JumpPressed) JumpPressed = animal.Jump;

            #region Double Jump

            if (animal.CanDoubleJump)           //If the Animal can double Jump
            {
                if (stateInfo.normalizedTime >= DoubleJumpTime && animal.Double_Jump != 1)
                {
                    if (animal.Jump)
                    {
                        animal.Double_Jump++;
                        animal.SetIntID(112);
                        return;
                    }
                }
            }
            #endregion

            //Since the speed is constantly changed while is jumping (with rootMotion) we need to add speed constantly WITH DELTAPOSITION trough out the whole jump
            if (!isInTransition && Can_Add_ExtraJump && !JumpEnd)
            {
                if (animal.JumpPress)
                {
                    var range = JumpPressed ? 1 : 0;
                    JumpSmoothPressed = Mathf.Lerp(JumpSmoothPressed, range, Time.deltaTime * 5);
                }

                animal.DeltaPosition += (ExtraJump * Time.deltaTime * JumpSmoothPressed);
            }

            if (stateInfo.normalizedTime >= willFall && cast_WillFall_Ray == false)
            {
                Can_Fall(stateInfo.normalizedTime);
                cast_WillFall_Ray = true;
            }

            if (animal.FrameCounter % animal.FallRayInterval == 0)        //Skip to reduce aditional raycasting
            {
                Can_Jump_on_Cliff(stateInfo.normalizedTime);
            }


         
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animal.AirControl) AirControl();
        }

        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var currentState = animator.GetCurrentAnimatorStateInfo(layerIndex);

            if (currentState.tagHash == AnimTag.Fly)                                 //if the next animation is fly then clean the rigidbody velocity on the Y axis
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
            else if (currentState.tagHash != AnimTag.Fall && currentState.tagHash != AnimTag.Jump)                          //if the next state is NOT Fall or Fly set the Ground_Constraints
            {
                animal.IsInAir = false;
            }

            if (currentState.tagHash == AnimTag.JumpEnd)  JumpEnd = true;
           
        }

        /// <summary>Check if the animal can change to fall state if there's no future ground to land on</summary>
        void Can_Fall(float normalizedTime)
        {
            Debug.DrawRay(animal.Pivot_fall, -transform.up * animal.Pivot_Multiplier * fallRay, Color.red);

            if (MinJumpLand > 0)
            {
              
                if (Physics.Raycast(animal.Pivot_fall, -transform.up, out JumpRay, animal.Pivot_Multiplier * fallRay, animal.GroundLayer))
                {
                    float distance = Vector3.Distance(animal.Pivot_fall, JumpRay.point);
                    float Angle = Vector3.Angle(Vector3.up, JumpRay.normal);

                  if (animal.debug)  Debug.Log("Min Distance to complete the Jump: "+ distance);

                    if ((MinJumpLand * animal.ScaleFactor) < distance || Angle > animal.maxAngleSlope)
                    {
                        animal.SetIntID(111);
                        MalbersTools.DebugTriangle(JumpRay.point, 0.1f, Color.yellow);
                    }
                }
                else
                { animal.SetIntID(111); }
            }
            else if (Physics.Raycast(animal.Pivot_fall, -transform.up, out JumpRay, animal.Pivot_Multiplier * fallRay, animal.GroundLayer))
            {
                if ((jumpPoint - JumpRay.point.y) <= (stepHeight * animal.ScaleFactor)
                    && (Vector3.Angle(JumpRay.normal, Vector3.up) < animal.maxAngleSlope))      //If if finding a lower jump point;
                {
                    MalbersTools.DebugTriangle(JumpRay.point, 0.1f, Color.red);
                }
                else
                {
                    animal.SetIntID(111);           //Set INTID to 111 to activate the FALL transition
                    MalbersTools.DebugTriangle(JumpRay.point, 0.1f, Color.yellow);
                }
            }
            else
            {
                animal.SetIntID(111);                //Set INTID to 111 to activate the FALL transition
                MalbersTools.DebugPlane(animal.Pivot_fall - (transform.up * animal.Pivot_Multiplier * fallRay), 0.1f, Color.red);
            }
        }

        /// <summary>─Get jumping on a cliff</summary>
        void Can_Jump_on_Cliff(float normalizedTime)
        {
          if (normalizedTime >= Cliff.minValue && normalizedTime <= Cliff.maxValue)
            {
                if (Physics.Raycast(animal.Main_Pivot_Point, -transform.up, out JumpRay, CliffRay * animal.ScaleFactor, animal.GroundLayer))
                {
                    if (Vector3.Angle(JumpRay.normal, Vector3.up) < animal.maxAngleSlope)       //Jump to a jumpable cliff not an inclined one
                    {
                        if (animal.debug)
                        {
                            Debug.DrawLine(animal.Main_Pivot_Point, JumpRay.point, Color.black);
                            MalbersTools.DebugTriangle(JumpRay.point, 0.1f, Color.black);
                        }
                        animal.SetIntID(110);
                    }
                }
                else
                {
                    if (animal.debug)
                    {
                        Debug.DrawRay(animal.Main_Pivot_Point, - transform.up * CliffRay * animal.ScaleFactor, Color.black);
                        MalbersTools.DebugPlane(animal.Main_Pivot_Point - ( transform.up * CliffRay * animal.ScaleFactor), 0.1f, Color.black);
                    }
                }
            }
        }

        /// <summary>/If the jump can be controlled on air
        void AirControl()
        {
            RaycastHit hit_AirControl = animal.FallRayCast;
            float Angle = Vector3.Angle(Vector3.up, hit_AirControl.normal);
            if (Angle > animal.maxAngleSlope) return;


            float deltaTime = Time.deltaTime;
            var VerticalSpeed = rb.velocity.y;
            var PlanarRawDirection = animal.RawDirection;
            PlanarRawDirection.y = 0;

            animal.AirControlDir = Vector3.Lerp(animal.AirControlDir, PlanarRawDirection * ForwardMultiplier, deltaTime * animal.airSmoothness);

            Debug.DrawRay(transform.position, transform.TransformDirection(animal.AirControlDir), Color.yellow);

            Vector3 RB_Velocity = animal.AirControlDir * animal.AirForwardMultiplier;

            if (!animal.DirectionalMovement)
            {
                RB_Velocity = transform.TransformDirection(RB_Velocity);
            }

            RB_Velocity.y = VerticalSpeed;

            rb.velocity = RB_Velocity;
        }
    }
}



