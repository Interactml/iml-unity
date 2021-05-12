using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using MalbersAnimations.Events;
using MalbersAnimations.Utilities;


namespace MalbersAnimations
{
    public partial class Animal
    {
        public enum Ground { walk = 1, trot = 2, run = 3 }

        /// <summary>List of all the animals on the scene </summary>
        public static List<Animal> Animals;

        #region Components 
        protected Animator anim;                    //Reference for the Animator
        protected Rigidbody _rigidbody;             //Reference for the RigidBody
        private Renderer animalMesh;                //Reference for the Mesh Renderer of this animal
        #endregion

        #region Animator Variables
        protected Vector3 movementAxis;             //In this variable will store Forward (Z) and horizontal (X) Movement
        protected Vector3 rawDirection;
       
        /// <summary>Transform.UP (Stored)</summary>
        internal Vector3 T_Up;
        /// <summary>Transform.Right (Stored)</summary>
        internal Vector3 T_Right;
        /// <summary>Transform.Forward (Stored) </summary>
        internal Vector3 T_Forward;

        /// <summary>Transform.Forward with no Y Value</summary>
        public Vector3 T_ForwardNoY
        { get { return new Vector3(T_Forward.x, 0, T_Forward.z).normalized; } }

        public static readonly float LowWaterLevel = -1000;

        protected bool
            speed1,                     //Walk (Set by Input)
            speed2,                     //Trot (Set by Input)
            speed3,                     //Run  (Set by Input)
            movementReleased = false,

            jump,                       //Jump (Set by Input)
            fly,                        //Fly  (Set by Input)
            shift,                      //Sprint or Speed Swap (Set by Input)
            down,                       //Crouch, fly Down or Swim Underwater (Set by Input)
            up,                         //Up for Fly and Swim underwater (Set by Input)
            dodge;                      //Dodge (Set by Input)
        internal bool fall, fallback;   //If is falling, (Automatic: Fall method)
        protected bool
            isInWater,                  //if is Entering Water(Trigger by WaterEnter Script or if the RayWater Hit the WaterLayer)
            isInAir,                    //if is Jumping or falling (Automatic: Fix Position Method)
            swim,                       //if is in Deep Water (Automatic: Swim method)
            underwater,                 //While Swimming Go underwater 

            stun,                       //Stunned (Set by Input or callit by a property)
            action,                     //Actions (Set by Input combined with Action ID)  
            stand = true,               //If Horizontal and Vertical are =0 (Automatic)
            backray,                    //Check if the Back feet are touching the ground 
            frontray;                   //Check if the Front feet are touching the ground

        private float waterLevel = -10;// LowWaterLevel;   //Temporal Water Level value


        /// <summary>
        /// Is the animal using a Direction Vector for moving?
        /// </summary>
        private bool directionalMovement;

        /// <summary>
        /// Value for the Vertical Parameter on the Animator (Calculated from the Movement.z multiplied by the Speeds (Walk Trot Run) and Speed.Lerps
        /// </summary>
        protected float vertical;
        protected float horizontal;             //Direction from the Horizontal input multiplied by the Shift for turning 180    

        /// <summary>
        /// Animator Normalized State Time for the Base Layer 
        /// </summary>
        private float stateTime;

        protected float                        //Direction from the Horizontal input multiplied by the Shift for turning 180    
           groundSpeed = 1f,                   //Current Ground Speed (Walk=1, Trot=2, Run =3) 
           slope,                              //Normalized Angle Slope from the MAX Angle Slope 
           idfloat,                            //Float values for the animator
           _Height;                            //Height from the ground to the hip 

        protected int
            idInt,                  //Integer values for the animator      
            actionID = -1,          //Every Actions has an ID this combined with the action bool will activate an action animation
            tired,                  //Counter to go to SleepMode (AFK) 
            loops = 1;              //Some aniamtions can have multiple loops like the wing attack from the dragon or eat on the animals
        #endregion

        public int animalTypeID;    //This parameter exist to Add Additive pose to the animal

        [SerializeField] private int stance = 0;

        internal Vector3 FixedDeltaPos = Vector3.zero;
        internal Vector3 DeltaPosition = Vector3.zero;               //All the Position Modifications 
        internal Vector3 LastPosition = Vector3.zero;
        internal Quaternion DeltaRotation = Quaternion.identity;     //All the Rotation Modifications 

        #region JumpControl
        public bool JumpPress = false;
        public float JumpHeightMultiplier = 0;
        public float AirForwardMultiplier = 0;
        public bool CanDoubleJump = false;
       
        /// <summary>States for double Jump... 0 = Can Double Jump (reseted when is on Locomotion)... 1 = has already Double Jumped</summary>
        internal int Double_Jump = 0;

        #endregion

        #region Ground
        public LayerMask GroundLayer = 1;
        public Ground StartSpeed = Ground.walk;

        public float height = 1f;                   //Distance from the Pivots to the ground

        internal Speeds currentSpeed;              //Current Speed Modification

        public Speeds walkSpeed = new Speeds(8, 4, 6);
        public Speeds trotSpeed = new Speeds(4, 4, 6);
        public Speeds runSpeed = new Speeds(2, 4, 6);

        protected float CurrentAnimatorSpeed = 1;

        private Transform platform;
        protected Vector3 platform_Pos;
        protected float platform_formAngle;

        #region Animator Parameters

        public string m_Vertical = "Vertical";
        public string m_Horizontal = "Horizontal";
        public string m_UpDown = "UpDown";

        public string m_Stand = "Stand";
        public string m_Jump = "_Jump";

        public string m_Fly = "Fly";
        public string m_Fall = "Fall";

        public string m_Attack1 = "Attack1";
        public string m_Attack2 = "Attack2";

        public string m_Stunned = "Stunned";
        public string m_Damaged = "Damaged";

        public string m_Shift = "Shift";
        public string m_Death = "Death";
        public string m_Dodge = "Dodge";

        public string m_Underwater = "Underwater";
        public string m_Swim = "Swim";

        public string m_Action = "Action";
        public string m_IDAction = "IDAction";

        public string m_IDFloat = "IDFloat";
        public string m_IDInt = "IDInt";
        public string m_Slope = "Slope";
        public string m_Type = "Type";
        public string m_SpeedMultiplier = "SpeedMultiplier";
        public string m_StateTime = "StateTime";
        public string m_Stance = "Stance";


        internal int hash_Vertical;
        internal int hash_Horizontal;
        internal int hash_UpDown;
        internal int hash_Stand;
        internal int hash_Jump;
        internal int hash_Dodge;
        internal int hash_Fall;
        internal int hash_Type;
        internal int hash_Slope;
        internal int hash_Shift;
        internal int hash_Fly;
        internal int hash_Attack1;
        internal int hash_Attack2;
        internal int hash_Death;
        internal int hash_Damaged;
        internal int hash_Stunned;
        internal int hash_IDInt;
        internal int hash_IDFloat;
        internal int hash_Swim;
        internal int hash_Underwater;
        internal int hash_IDAction;
        internal int hash_Action;
        internal int hash_StateTime;
        internal int hash_Stance;


        #region Optional Animator Parameters Activation
        [HideInInspector] bool hasFly;
        [HideInInspector] bool hasDodge;
        [HideInInspector] bool hasSlope;
        [HideInInspector] bool hasStun;
        [HideInInspector] bool hasAttack2;
        [HideInInspector] bool hasUpDown;
        [HideInInspector] bool hasUnderwater;
        [HideInInspector] bool hasSwim;
        [HideInInspector] bool hasStateTime;
        [HideInInspector] bool hasStance;
        #endregion

        #endregion


        #region AirControl
        /// <summary>Enables you to rotate the animal while jumping or falling</summary>
        public float airRotation = 100;
        /// <summary>Allows the Inputs to control the Fall and Jump movement on Air</summary>
        public bool AirControl = false;
        /// <summary> Maximum Horizontal Speed to move while on the air</summary>
     //   public float airMaxSpeed = 1f;
        /// <summary> Lerp between air stand and moving forward</summary>
        public float airSmoothness = 2;
        /// <summary>Acumulated Speed from the Air Control to tranfer it from jump to fall</summary>
        internal Vector3 AirControlDir;
        #endregion


        public float movementS1 = 1, movementS2 = 2, movementS3 = 3;        //IMPORTANT this are the values for the Animator Locomotion Blend Tree when the velocity is changed (Ex. Horse has 5 velocities)

        /// <summary>Maximun angle on the terrain the animal can walk </summary>
        [Range(0f, 90f)]
        public float maxAngleSlope = 45f;
        public bool SlowSlopes = true;

        [Range(0, 100)]
        public int GotoSleep;



        /// <summary>Smoothness value to Snap to ground </summary>
        public float SnapToGround = 20f;
        /// <summary>Smoothness value to aling to ground </summary>
        public float AlingToGround = 30f;
        public float FallRayDistance = 0.1f;
        public float BackFallRayDistance = 0.5f;
        public float FallRayMultiplier = 1f;

        /// <summary>
        /// Smooth transitions when Vertical axis changes (Set it to false for Keyboard, true for Gamepads/Touch Controllers)
        /// </summary>
        public bool SmoothVertical = true;
        /// <summary>
        /// When Using Directions or (CameraBasedInput) the Y value of the Direction Vector will be ignored
        /// </summary>
        public bool IgnoreYDir = false;
        public float TurnMultiplier = 100;




        #region Water Variables
        public float waterLine = 0f;                    //WaterLine

        public Speeds swimSpeed = new Speeds(8, 4, 6);  //SwimSpeed
        internal int WaterLayer;                        //Water Layer ID
        public bool canSwim = true;
        public bool CanGoUnderWater;

        [Range(0, 90)]
        public float bank = 0;
        public Speeds underWaterSpeed = new Speeds(8, 4, 6);         //SwimSpeed
                                                                     //float UnderWaterShift = 1;                           //Shift  
        #endregion

        #region Fly Variables
        public Speeds flySpeed = new Speeds();                  //Fly Speed Values
        /// <summary>On Start the nimal will be set to fly</summary>
        public bool StartFlying;                                
        /// <summary> Can the animal fly?</summary>
        public bool canFly;                                     
        /// <summary>When the animal is near to the ground it will land automatically</summary>
        public bool land = true;                                //if Land true means that when is close to the ground it will exit the Fly State
        protected float LastGroundSpeed;                        //To save the las ground speed before it start flying

        /// <summary> The animal cannot fly upwards... just fly forward or down...</summary>
        public bool LockUp = false;                    
        #endregion


        #region Attributes Variables (Attack, Damage)
        public float life = 100;
        public float defense = 0;
        public float damageDelay = 0.75f;            //Time before can aply damagage again
        public float damageInterrupt = 0.5f;       

        public int TotalAttacks = 3;
        public int activeAttack = -1;
        public float attackStrength = 10;
        public float attackDelay = 0.5f;

        public bool inmune;

        protected bool
            attack1, attack2,                   //Attacks (Set by Input) 
            isAttacking,                        //Set to true whenever an Attack Animations is played (Set by Animator) 
            isTakingDamage,                     //Prevent to take damage while this variable is true
            damaged,                            //GetHit (Set by OnTriggerEnter)
            death;                              //Death (Set by Life<0)

        protected List<AttackTrigger> Attack_Triggers;      //List of all the Damage Triggers on this Animal.
        #endregion
        #endregion
        /// <summary>Global Modification of the Animator Speed </summary>
        public float animatorSpeed = 1f;
        public float upDownSmoothness = 2f;
        public bool debug = true;

        //------------------------------------------------------------------------------
        #region Modify_the_Position_Variables
        internal RaycastHit hit_Hip; //Hip and Chest Ray Cast Information
        internal RaycastHit hit_Chest; //Hip and Chest Ray Cast Information
        internal RaycastHit WaterHitCenter; //Hip and Chest Ray Cast Information
        /// <summary>Raycast Information for Fall</summary>
        internal RaycastHit FallRayCast; //Hip and Chest Ray Cast Information



        protected Vector3
            fall_Point,
            _hitDirection,
            UpVector = Vector3.up;

        protected float scaleFactor = 1;


        protected List<Pivots> pivots = new List<Pivots>();
        protected Pivots pivot_Chest, pivot_Hip, pivot_Water;

        public int PivotsRayInterval = 1;
        public int FallRayInterval = 3;
        public int WaterRayInterval = 5;


        #endregion

   

        #region Events
        public UnityEvent OnJump;
        public UnityEvent OnAttack;
        public FloatEvent OnGetDamaged;
        public UnityEvent OnDeathE;
        public UnityEvent OnAction;
        public UnityEvent OnSwim;
        public BoolEvent OnFly;
        public UnityEvent OnUnderWater;
        public IntEvent OnAnimationChange;
        public IntEvent OnStanceChange;
        /// <summary>
        /// Is Invoked after the Parameters are sent to the animator Useful to link parameters with external scripts.. (Riding System)
        /// </summary>
        public UnityEvent OnSyncAnimator;     //Used for Sync Animators
        #endregion

        private static RaycastHit NULLRayCast = new RaycastHit();
        List<Collider> _col_ = new List<Collider>();                                                            //Colliders to disable with animator
        [HideInInspector]  public int FrameCounter;
      

        #region Properties
        /// <summary> Animal's RigidBody </summary>
        public Rigidbody _RigidBody
        {
            get
            {
                if (_rigidbody == null)
                {
                    _rigidbody = GetComponentInChildren<Rigidbody>();
                }
                return _rigidbody;
            }
        }

        /// <summary>
        /// Current Animal Speed in numbers, 1 = walk 2 = trot 3 = run 
        /// </summary>
        public virtual float GroundSpeed
        {
            set { groundSpeed = value; }
            get { return groundSpeed; }
        }


        /// <summary>
        /// Speed from the Vertical input multiplied by the speeds inputs(Walk Trot Run) this is the value thats goes to the Animator, is not the actual Speed of the animals
        /// </summary>
        public virtual float Speed
        {
            set { vertical = value; }
            get { return vertical; }
        }

        /// <summary>
        /// Gets the Normalized Slope of the terrain
        /// </summary>
        public float Slope
        {
            get
            {
                slope = 0;
                if (pivot_Chest && pivot_Hip)
                {
                    float AngleSlope = Vector3.Angle(SurfaceNormal, UpVector);                              //Calculate the Angle of the Terrain

                    float SlopeDirection = 1;
                    SlopeDirection = pivot_Chest.Y > pivot_Hip.Y ? 1 : -1;
                    slope = AngleSlope / maxAngleSlope * (SlopeDirection <= 0 ? -1 : 1);                   //Normalize the AngleSlop by the MAX Angle Slope and make it positive(HighHill) or negative(DownHill)

                    return slope;
                }

                return 0;
            }
        } 
        
        public BoolEvent OnMovementReleased = new BoolEvent();
        /// <summary>
        /// Checking if the input for forward or turn changed.
        /// </summary>
        public virtual bool MovementReleased
        {
            private set
            {
                if (movementReleased != value)
                {
                    movementReleased = value;
                    OnMovementReleased.Invoke(value);
                    //if (AnimState != Hash.Fall) SetFloatID(value ? -1 : 0); //Send to the animator that the Movement was released by setting it to (-2) or to (0) if it wasn't;
                }
            }
            get { return movementReleased; }
        }


        /// <summary>
        /// Is the Animal swimming?
        /// </summary>
        public virtual bool Swim
        {
            set
            {
                if (swim != value && Time.time - swimChanged >= 0.8f)      //All of this is for not changing back to false or true immediately; Wait 0.5 sec...
                {
                    swim = value;
                    swimChanged = Time.time;
                    currentSpeed = swimSpeed;

                    if (swim)
                    {
                        fall = isInAir = fly = false;  // Reset all other states Just Once
                        OnSwim.Invoke();
                        _RigidBody.constraints = Still_Constraints;
                        currentSpeed = swimSpeed;
                    }
                }
            }
            get { return swim; }
        }

        private float swimChanged;

        /// <summary>
        /// Direction from the Horizontal input multiplied by the speeds inputs(Walk Trot Run) this is the value thats goes to the Animator, is not the actual Speed of the animals
        /// </summary>
        public float Direction
        {
            get { return horizontal; }
        }

        /// <summary>
        /// Controls the Loops for some animations that can be played for an ammount of cycles.
        /// </summary>
        public int Loops
        {
            set { loops = value; }
            get { return loops; }
        }

        public int IDInt
        {
            set { idInt = value; }
            get { return idInt; }
        }

        public float IDFloat
        {
            set { idfloat = value; }
            get { return idfloat; }
        }

        /// <summary>
        /// Amount of Idle acumulated if the animals is not moving, if Tired is greater than GotoSleep the animal will go to the sleep state. </summary>/
        public int Tired
        {
            set { tired = value; }
            get { return tired; }
        }

        /// <summary>Is the animal on water? not necessarily swimming </summary>
        public bool IsInWater
        {
            get { return isInWater; }
        }

        /// <summary>Change the Speed Up</summary>
        public bool SpeedUp
        {
            set
            {
                if (value)
                {
                    if (groundSpeed == movementS1) Speed2 = true;
                    else if (groundSpeed == movementS2) Speed3 = true;
                }
            }
        }

        /// <summary>Changes the Speed Down</summary>
        public bool SpeedDown
        {
            set
            {
                if (value)
                {
                    if (groundSpeed == movementS3) Speed2 = true;
                    else if (groundSpeed == movementS2) Speed1 = true;
                }
            }
        }

        /// <summary>Set the Animal Speed to Speed1 </summary>
        public bool Speed1
        {
            get { return speed1; }

            set
            {
                if (value)
                {
                    speed1 = value;
                    speed2 = speed3 = false;
                    groundSpeed = movementS1;
                }
            }
        }
      
        /// <summary>
        /// Set the Animal Speed to Speed2
        /// </summary>
        public bool Speed2
        {
            get { return speed2; }

            set
            {
                if (value)
                {
                    speed2 = value;
                    speed1 = speed3 = false;
                    groundSpeed = movementS2;
                }
            }
        }

        /// <summary>
        /// Set the Animal Speed to Speed3
        /// </summary>
        public bool Speed3
        {
            get { return speed3; }
            set
            {
                if (value)
                {
                    speed3 = value;
                    speed2 = speed1 = false;
                    groundSpeed = movementS3;
                }
            }
        }

        public bool Jump
        {
            get { return jump; }
            set { jump = value; }
        }

        /// <summary> is the Animal UnderWater </summary>
        public bool Underwater
        {
            get { return underwater; }
            set
            {
                if (CanGoUnderWater)
                {
                    underwater = value;                 //Just change the Underwater Variable if can GoUnderWater is enabled
                   // currentSpeed = underWaterSpeed;
                }
            }
        }

        /// <summary> Allows to use Sprint </summary>
       [SerializeField] private bool useShift = true;

        public bool Shift
        {
            get { return shift; }
            set { shift = value; }
        }

        public bool Down
        {
            get { return down; }
            set { down = value; }
        }

        public bool Up
        {
            get { return up; }
            set { up = value; }
        }

        public bool Dodge
        {
            get { return dodge; }
            set { dodge = value; }
        }

        
        public bool Damaged
        {
            get { return damaged; }
            set { damaged = value; }
        }

        

        /// <summary>Toogle the Fly on and Off!!</summary>
        public bool Fly
        {
            get
            {
                if (!canFly) fly = false;                       //Set back the fly to dalse if canfly is false
                return fly;
            }
            set
            {
                if (!canFly) { return; }                        //Do nothing if canfly is false

                
                if (value)                                      //Only When true
                {
                    fly = !fly;                                 //Toogle Fly

                    if (fly)                                    //OnFly Enabled!
                    {
                        _RigidBody.useGravity = false;          //Deactivate gravity in case use gravity is off
                        LastGroundSpeed = Mathf.RoundToInt( groundSpeed);
                        groundSpeed = 1;                        //Change velocity to 1 (Flap Wings)
                        IsInAir = true;
                        currentSpeed = flySpeed;
                        Quaternion finalRot = Quaternion.FromToRotation(T_Up, UpVector) * _transform.rotation;
                        StartCoroutine(MalbersTools.AlignTransformsC(_transform, finalRot, 0.3f));    //Quick Align the Fly
                    }
                    else
                    {
                        groundSpeed = LastGroundSpeed;          //Restore the Ground Speed
                    }
                    OnFly.Invoke(fly);                          //Invoke the Event OnFly;
                }
            }
        }

        /// <summary>If set to true the animal will die </summary>
        public bool Death
        {
            get { return death; }
            set
            {
                death = value;
                if (death)
                {
                    Anim.SetTrigger(Hash.Death);                           //Triggers the death animation.

                    Anim.SetBool(Hash.Attack1, false);                     //Reset the Attack1 on the animator
                    if (hasAttack2) Anim.SetBool(Hash.Attack2, false);     //Reset the Attack2 on the animator
                
                    Anim.SetBool(Hash.Action , false);                     //Reset the Action on the animator

                    OnDeathE.Invoke();                                     //Invoke the animal is death
                    if (Animals.Count > 0) Animals.Remove(this);           //Remove this animal of the animal list because is dead
                }
            }
        }

        /// <summary>Enables the Attack to the Current Active Attack</summary>
        public bool Attack1
        {
            get { return attack1; }
            set
            {
                if (!value) attack1 = value;

                if (death) return;                                //Don't Attack while your death 
                if (AnimState == AnimTag.Action) return;          //Don't Attack when is making an action

                if (!isAttacking)                                       //Attack when is not attacking
                {
                    if (value)                                          //If Attack was set to true
                    {
                        attack1 = value;
                        IDInt = activeAttack;                           //Change the IntID to the Active attack ID

                        if (IDInt <= 0) SetIntIDRandom(TotalAttacks);   // if the Active Attack == -1 then Play Random Attacks
                        OnAttack.Invoke();
                    }
                }
            }
        }

        public bool Attack2
        {
            get { return attack2; }
            set
            {
                if (death) return;                                                      //If im death dont attack L:)

                if (value)                                                              //If Attack was set to true
                {
                    if (AnimState == AnimTag.Action) return;                             //Don't Attack when is making an action
                }

                attack2 = value;
            }
        }

        public bool Stun
        {
            get { return stun; }
            set { stun = value; }
        }

        public bool Action
        {
            get { return action; }
            set
            {
                if (ActionID == -1) return;                         //There's no Action Active
                if (death) return;                                  //if you're death no not play any action.    

                if (action != value)
                {
                    action = value;

                    if (action)
                    {
                        StartCoroutine(ToggleAction());
                    }
                }
            }
        }

        public int ActionID
        {
            get { return actionID; }
            set { actionID = value; }
        }

        /// <summary>Is the Animal Attacking (making a Attack Animation) </summary>
        public bool IsAttacking
        {
            get { return isAttacking; }
            set { isAttacking = value; }
        }

        /// <summary> Change the Animator rootMotion value</summary>
        public bool RootMotion
        {
            set
            {
                Anim.applyRootMotion = value;
                //if (!value)
                //{
                //    _RigidBody.velocity = (transform.position - LastPosition)/Time.deltaTime;
                //}
            }
            get { return Anim.applyRootMotion; }
        }
       

        /// <summary>Is the Animal on the Air, modifies the rigidbody constraints depending the IsInAir Value </summary>
        public bool IsInAir
        {
            get { return isInAir; }
            set
            {
                isInAir = value;
                _RigidBody.constraints = isInAir ? RigidbodyConstraints.FreezeRotation : Still_Constraints;
            }
        }

        public bool Stand { get { return stand; } }

        public Vector3 HitDirection
        {
            get { return _hitDirection; }
            set { _hitDirection = value; }
        }
        
        /// <summary>The Scale Factor of the Animal.. if the animal has being scaled this is the multiplier for the raycasting things</summary>
        public float ScaleFactor { get { return scaleFactor; } }
      
        public Pivots Pivot_Hip { get { return pivot_Hip; } }
       

        public Pivots Pivot_Chest   { get { return pivot_Chest; } }
     

        /// <summary>Returns the Current Animation State Tag of animal</summary> 
        public int CurrentAnimState;


        /// <summary>Returns the Current Animation State Tag of animal, if is in transition it will return the NextState Tag</summary>
        public int AnimState
        {
            get { return NextAnimState != 0 ? NextAnimState : CurrentAnimState; }
        }
        

        public int LastAnimationTag
        {
            private set
            {
                lastAnimTag = value;
                OnAnimationChange.Invoke(value);
            }
            get { return lastAnimTag; }
        }
        private int lastAnimTag;
        private Transform _transform;

        /// <summary>Returns the Next Animation State  Tag of animal 0 means that is not in transition</summary>
        public int NextAnimState;
       

        /// <summary> Returns the Animator Component of the Animal</summary>
        public Animator Anim
        {
            get
            {
                if (anim == null)
                {
                    anim = GetComponent<Animator>();
                }
                return anim;
            }
        }

        public Vector3 Pivot_fall
        {
            get {return fall_Point;}
        }

        public float Pivot_Multiplier
        {
            get
            {
                float multiplier = Pivot_Chest ? Pivot_Chest.multiplier : (Pivot_Hip ? Pivot_Hip.multiplier : 1);
                return multiplier * scaleFactor;
            }
        }

        public Vector3 Main_Pivot_Point
        {
            get
            {
                if (pivot_Chest) return pivot_Chest.GetPivot;
                if (pivot_Hip) return pivot_Hip.GetPivot;

                Vector3 Chest = _transform.position;
                Chest.y += height;
                return Chest;
            }
        }


        /// <summary>Locks Position Y and AllRotations on the Rigid Body</summary>
        public static RigidbodyConstraints Still_Constraints
        {
            get { return RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY; }
        }

        /// <summary>Direction Vector the animal to move </summary>
        public Vector3 MovementAxis
        {  
            get { return movementAxis; }
            set { movementAxis = value; }
        }

        /// <summary> Returns Movement Axis Z value </summary>
        public float MovementForward
        {
            get { return movementAxis.z; }
            set { movementAxis.z = value;
                MovementReleased = value == 0;
            }
        }

        public float MovementRight
        {
            get { return movementAxis.x; }
            set { movementAxis.x = value;
                MovementReleased = value == 0;
            }
        }

        public float MovementUp
        {
            get { return movementAxis.y; }
            set { movementAxis.y = value;
              
                MovementReleased = value == 0;
            }
        }

        public Vector3 SurfaceNormal
        {
            get
            {
                if (pivot_Hip && hit_Hip.transform != null)
                {
                    if (Pivot_Chest && hit_Chest.transform != null)
                    {
                        Vector3 direction = (hit_Chest.point - hit_Hip.point).normalized;
                        Vector3 Side = Vector3.Cross(UpVector, direction).normalized;
                        return Vector3.Cross(direction, Side).normalized;
                    }
                    else
                    {
                        return hit_Hip.normal;
                    }
                }
                return Vector3.up;
            }
        }


        public Renderer AnimalMesh
        {
            get { return animalMesh; }
            set { animalMesh = value; }
        }

        //Y value of the water Got it from the WaterRayCastHit
        public float Waterlevel
        {
            get {  return waterLevel; }
           set {   waterLevel = value; }
        }

        /// <summary> Is the animal using a Direction Vector for moving?</summary>
        public bool DirectionalMovement { get { return directionalMovement; } }
       
        /// <summary>RawDirection Vector if the Animal is using DirectionalMovement</summary>
        public Vector3 RawDirection
        {
            get { return rawDirection; }
            set { rawDirection = value; }
        }

        /// <summary> if the Animal can fly and you want to automatically land when the ground is found </summary>
        public bool Land
        {
            get { return land; }
            set { land = value; }
        }

        /// <summary>
        /// Animator Normalized State Time for the Base Layer 
        /// </summary>
        public float StateTime
        {
            get { return stateTime; }
            set { stateTime = value; }
        }

        /// <summary>
        /// Current Animal Stance 
        /// </summary>
        public int Stance
        {
            get { return stance; }
            set
            {
                if (stance != value)
                {
                    lastStance = stance;
                    stance = value;
                    OnStanceChange.Invoke(value);
                }
                if (hasStance) Anim.SetInteger(Hash.Stance, stance);
            }
        }
      

        /// <summary>
        /// Return the Last Animal Stance 
        /// </summary>
        public int LastStance
        {
            get { return lastStance; }
        }

        /// <summary>
        /// Can the Animal Use Sprint
        /// </summary>
        public bool UseShift
        {
            get { return useShift; }
            set { useShift = value; }
        }

        protected Transform Platform
        {
            get { return platform; }
            set { platform = value; }
        }

        private int lastStance;

        #endregion

        #region UnityEditor Variables
        [HideInInspector] public bool EditorGeneral = true;
        [HideInInspector] public bool EditorGround = true;
        [HideInInspector] public bool EditorWater = true;
        [HideInInspector] public bool EditorAir = true;
        [HideInInspector] public bool EditorAdvanced = true;
        [HideInInspector] public bool EditorAirControl = true;
        [HideInInspector] public bool EditorAttributes = true;
        [HideInInspector] public bool EditorEvents = false;
        [HideInInspector] public bool EditorAnimatorParameters = false;
        #endregion
    }
}
