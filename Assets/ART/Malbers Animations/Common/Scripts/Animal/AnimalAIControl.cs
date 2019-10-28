using UnityEngine;
using System.Collections;
using MalbersAnimations.Events;
using MalbersAnimations.Utilities;
using UnityEngine.Events;

#if UNITY_5_5_OR_NEWER
using UnityEngine.AI;
#endif

namespace MalbersAnimations
{
    /// <summary>
    /// Control for the Animal using Nav Mesh Agent
    /// </summary>
    [RequireComponent(typeof(Animal))]
    public class AnimalAIControl : MonoBehaviour
    {
        #region VARIABLES
        #region Components References
        private NavMeshAgent agent;                 //The NavMeshAgent
        protected Animal animal;                    //The Animal Script
        #endregion

        #region Target Verifications
        /// <summary>
        /// Is the Target an Animal?
        /// </summary>
       // protected Animal TisAnimal;          //To check if the target is an animal
        /// <summary>
        /// Is the Target an Action Zone?
        /// </summary>
        protected ActionZone isActionZone;
        /// <summary>
        /// Is the Target a WayPoint?
        /// </summary>
        protected MWayPoint isWayPoint;
        #endregion

        #region Internal Variables

        /// <summary>
        ///  The way to know if there no Target Position vector to go to
        /// </summary>
        protected static Vector3 NullVector = MalbersTools.NullVector;
        /// <summary>
        /// Desired Position to go to
        /// </summary>
        protected Vector3 targetPosition = NullVector;
        protected Vector3 TargetLastPosition = NullVector;

        /// <summary>Stores the Remainin distance to the Target's Position</summary>
        protected float RemainingDistance;
        protected float DefaultStopDistance;
        /// <summary>Used to Check if you enter once on a OffMeshLink</summary>
        protected bool EnterOFFMESH;

        /// <summary>Check if the animal is making an Action animation  </summary>
        protected bool DoingAnAction;
        protected bool EnterAction;

        /// <summary>Is the Animal stopped by an external source like Public Function Stop or Mount AI</summary>       
        protected bool Stopped = false;
        /// <summary>Sometimes OffMesh Links can be travelled by flying</summary>         
        private bool isFlyingOffMesh;

        internal IWayPoint NextWayPoint;
        /// <summary>
        /// True if the animal should be Flying
        /// </summary>
        protected bool flyPending;
        #endregion

        #region Public Variables
        [SerializeField]
        protected float stoppingDistance = 0.6f;
        [SerializeField]
        protected Transform target;                    //The Target
        public bool AutoSpeed = true;
        public float ToTrot = 6f;
        public float ToRun = 8f;
        public bool debug = false;                          //Debuging 


    
        #endregion

        #region Events
        [Space]
        public Vector3Event OnTargetPositionArrived = new Vector3Event();
        public TransformEvent OnTargetArrived = new TransformEvent();
        public UnityEvent OnActionStart = new UnityEvent();
        public UnityEvent OnActionEnd = new UnityEvent();
        public StringEvent OnDebug = new StringEvent();
        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// the navmeshAgent asociate to this GameObject
        /// </summary>
        public NavMeshAgent Agent
        {
            get
            {
                if (agent == null)
                {
                    agent = GetComponentInChildren<NavMeshAgent>();
                }
                return agent;
            }
        }

        /// <summary>
        /// Stopping Distance of the Next Waypoint
        /// </summary>
        public float StoppingDistance
        {
            get { return stoppingDistance; }

            set { Agent.stoppingDistance = stoppingDistance = value; }
        }


        protected bool targetisMoving;
        /// <summary>
        /// is the Target transform moving??
        /// </summary>
        public bool TargetisMoving
        {
            get
            {
                if (target != null)
                {
                    targetisMoving = (target.position - TargetLastPosition).magnitude > 0.001f;
                    return targetisMoving;
                }
                targetisMoving = false;
                return targetisMoving;
            }
        }

        /// <summary>
        /// The Agent is Active and we are on a NavMesh
        /// </summary>
        public bool AgentActive
        {
            get { return Agent.isOnNavMesh && Agent.enabled; }
        }

        /// <summary>
        /// Is the Animal waiting x time to go to the Next waypoint
        /// </summary>
        public bool IsWaiting { get; protected set; }

        #endregion

        void Start() { StartAgent(); }

        /// <summary>
        /// Initialize the Ai Animal Control Values
        /// </summary>
        protected virtual void StartAgent()
        {
            animal = GetComponent<Animal>();
            animal.OnAnimationChange.AddListener(OnAnimationChanged);           //Listen when the Animations changes..

            DoingAnAction = isFlyingOffMesh = false;

            Agent.updateRotation = false;                                       //The animator will control the rotation and postion.. NOT THE AGENT
            Agent.updatePosition = false;
            DefaultStopDistance = StoppingDistance;                             //Store the Started Stopping Distance
            Agent.stoppingDistance = StoppingDistance;
            SetTarget(target);                                                  //Set the first Target
            IsWaiting = false;
        }

        void Update()
        {
            Updating();

        }

        protected virtual void Updating()
        {
            if (isFlyingOffMesh) return;


            if (Stopped)
            {
                if (TargetisMoving)
                {
                    Stopped = false;
                    SetTarget(target);
                }
            }
            else if (animal.Fly || animal.Swim)                                                         //if the Animal is flying?
            {
                FreeMovement();
            }
            else if (AgentActive)                                               //if we are on a NAV MESH onGround
            {
                if (IsWaiting) return;                                          //If the Animal is Waiting no nothing . .... he is doing something else... wait until he's finish

                if (targetPosition == NullVector)                               //if there's no Position to go to.. Stop the Agent
                {
                    StopAnimal();
                }
                else
                    UpdateAgent();
            }

            if (target)
            {
                if (TargetisMoving) UpdateTargetTransform();

                TargetLastPosition = target.position;
            }

            Agent.nextPosition = agent.transform.position;                  //Update the Agent Position to the Transform position
        }

        /// <summary>
        /// Movement with no Agent at all
        /// </summary>
        private void FreeMovement()
        {
            if (IsWaiting) return;
            if (target ==null || targetPosition == NullVector) return; //If we have no were to go then Skip the code

            RemainingDistance = target ? Vector3.Distance(animal.transform.position, target.position) : 0;

            var Direction = (target.position - animal.transform.position);

            animal.Move(Direction);

            Debug.DrawRay(animal.transform.position, Direction.normalized, Color.white);

            if (RemainingDistance < StoppingDistance/* && !IsWaiting*/)   //We arrived to our destination
            {
                if (NextWayPoint != null && NextWayPoint.PointType != WayPointType.Air && animal.Fly) animal.SetFly(false); //If we arrive to our point and it was not an Air Point Disable Fly

                CheckNextTarget();
                // SetNextTarget();
            }
        }

        private void CheckNextTarget()
        {
            if (isActionZone && !DoingAnAction)                     //If the Target is an Action Zone Start the Action
            {
                animal.Action = true;                               //Activate the Action on the Animal (The ID is Given by the ACTION ZONE)
                animal.Stop();

                if (isActionZone.MoveToExitAction)
                {
                    float time = isActionZone.WaitTime;
                    Debuging(name + "is Waiting " + time + " seconds to finish a 'Move to Exit' Action");
                    animal.Invoke("WakeAnimal", time);
                }
            }
            else //if (isWayPoint)                                    //If the Next Target is a Waypoint
            {
                SetNextTarget();
            }
        }

        /// <summary>
        /// This will be Called everytime the animal changes an animation (Via Unity Event)
        /// </summary>
        protected virtual void OnAnimationChanged(int animTag)
        {
            var isInAction = (animTag == AnimTag.Action);                                 //Check if the Animal is making an Action

            if (isInAction != DoingAnAction)
            {
                DoingAnAction = isInAction;                                               //Update the Current Status of the Action Variable

                if (DoingAnAction)                  //If we started an Action ?
                {
                    OnActionStart.Invoke();
                    Debuging(name + " has started an ACTION");
                    IsWaiting = true;               //Set that the animal is doing something
                }
                else
                {
                    OnActionEnd.Invoke();
                    Debuging(name + " has ended an ACTION");

                    if (!EnterOFFMESH)      //if the action was not on an offmeshlink like eat drink..etc
                    {
                        SetNextTarget();
                    }
                    else
                    {
                        IsWaiting = false;
                    }
                }
            }

            if (animTag == AnimTag.Jump) animal.MovementRight = 0;                                       //Don't rotate if is in the middle of a jump

            if (animTag == AnimTag.Locomotion || animTag == AnimTag.Idle)                                //Activate the Agent when the animal is moving
            {
                if (animal.canFly)
                {
                    if (flyPending && !animal.Fly && NextWayPoint.PointType == WayPointType.Air)        //Fly is pending and the next waypoint is on the air so set ehe animal to fly
                    {
                        animal.SetFly(true);
                        flyPending = false;
                        return;  //Skip the rest of the code
                    }
                }

                if (!Agent.enabled)                     //If the Agent is disabled while on Idle Locomotion or Recovering Enable it 
                {
                    Agent.enabled = true;
                    Agent.ResetPath();
                    //Debuging("Enabling Agent for " + name + ".");
                    EnterOFFMESH = false;

                    if (targetPosition != NullVector)                                       //Resume the the path with the new Target Position in case there's one
                    {
                        Agent.SetDestination(targetPosition);
                        Agent.isStopped = false;
                    }
                }
            }
            else   //Disable the Agent whe is not on Locomotion or Idling (for when is falling or swimming)
            {
                if (Agent.enabled)
                {
                    Agent.enabled = false;
                    string a = "not on Locomotion or Idle";

                    if (animTag == AnimTag.Action) a = "doing an Action";
                    if (animTag == AnimTag.Jump) a = "Jumping";
                    if (animTag == AnimTag.Fall) a = "Falling";
                    if (animTag == AnimTag.Recover) a = "Recovering";


                    Debuging("Disable Agent. " + name + " is "+ a);
                }
            }
        }

        /// <summary>
        /// Set the next target and wait x time if the next waypoint has wait Time > 0
        /// </summary>
        private void SetNextTarget()
        {
            if (WaitToNextTargetC != null) StopCoroutine(WaitToNextTargetC);
            if (NextWayPoint != null)
            {
                WaitToNextTargetC = WaitToNextTarget(NextWayPoint.WaitTime, NextWayPoint.NextTarget);
                StartCoroutine(WaitToNextTargetC);
            }
        }

        /// <summary>
        /// Updates the Agents using he animation root motion
        /// </summary>
        protected virtual void UpdateAgent()
        {
            var Direction = Vector3.zero;                               //Reset the Direction (THIS IS THE DIRECTION VECTOR SENT TO THE ANIMAL)  

            RemainingDistance = Agent.remainingDistance;                    //Store the remaining distance -- but if navMeshAgent is still looking for a path Keep Moving
            //RemainingDistance = Agent.remainingDistance <=0 ? float.PositiveInfinity : Agent.remainingDistance;

            if (Agent.pathPending || Mathf.Abs(RemainingDistance) <= 0.1f)      //In Case the remaining Distance is wrong
            {
                RemainingDistance = float.PositiveInfinity;
                UpdateTargetTransform();
            }

            if (RemainingDistance > StoppingDistance)                   //if haven't arrived yet to our destination  
            {
                Direction = Agent.desiredVelocity;
                DoingAnAction = false;
            }
            else  //if we get to our destination                                                          
            {
                OnTargetPositionArrived.Invoke(targetPosition);         //Invoke the Event On Target Position Arrived
                if (target)
                {
                    OnTargetArrived.Invoke(target);                 //Invoke the Event On Target Arrived
                    if (isWayPoint) isWayPoint.TargetArrived(this); //Send that the Animal has Arrived
                }

                targetPosition = NullVector;                            //Reset the TargetPosition
                agent.isStopped = true;                                 //Stop the Agent

                CheckNextTarget();
            }

            animal.Move(Direction);                                     //Set the Movement to the Animal

            if (AutoSpeed) AutomaticSpeed();                            //Set Automatic Speeds
            CheckOffMeshLinks();                                        //Jump/Fall behaviour 
        }


        protected virtual void WakeAnimal()
        {
            animal.WakeAnimal();
            IsWaiting = false;
        }

        /// <summary>
        /// Manage all Off Mesh Links
        /// </summary>
        protected virtual void CheckOffMeshLinks()
        {
            if (Agent.isOnOffMeshLink && !EnterOFFMESH)                         //Check if the Agent is on a OFF MESH LINK
            {
                EnterOFFMESH = true;                                            //Just to avoid entering here again while we are on a OFF MESH LINK
                OffMeshLinkData OMLData = Agent.currentOffMeshLinkData;

                if (OMLData.linkType == OffMeshLinkType.LinkTypeManual)                 //Means that it has a OffMesh Link component
                {
                    OffMeshLink CurrentOML = OMLData.offMeshLink;                       //Check if the OffMeshLink is a Manually placed  Link

                    ActionZone Is_a_OffMeshZone =
                        CurrentOML.GetComponentInParent<ActionZone>();                  //Search if the OFFMESH IS An ACTION ZONE (EXAMPLE CRAWL)

                    if (Is_a_OffMeshZone && !DoingAnAction)                             //if the OffmeshLink is a zone and is not making an action
                    {
                        animal.Action = DoingAnAction = true;                           //Activate the Action on the Animal
                        return;
                    }


                    var DistanceEnd = (transform.position - CurrentOML.endTransform.position).sqrMagnitude;
                    var DistanceStart = (transform.position - CurrentOML.startTransform.position).sqrMagnitude;

                    var NearTransform = DistanceEnd < DistanceStart ? CurrentOML.endTransform : CurrentOML.startTransform;
                    var FarTransform = DistanceEnd > DistanceStart ? CurrentOML.endTransform : CurrentOML.startTransform;
                    StartCoroutine(MalbersTools.AlignTransform_Rotation(transform, NearTransform.rotation, 0.15f)); //Aling the Animal to the Link Position

                    if (animal.canFly && CurrentOML.CompareTag("Fly"))
                    {
                        Debuging(name + ": Fly OffMesh");
                        StartCoroutine(CFlyOffMesh(FarTransform));
                    }
                    else if (CurrentOML.area == 2)
                    {
                        animal.SetJump();                         //if the OffMesh Link is a Jump type
                    }
                }
                else if (OMLData.linkType == OffMeshLinkType.LinkTypeJumpAcross)             //Means that it has a OffMesh Link component
                {
                    animal.SetJump();
                }
            }
        }

        IEnumerator WaitToNextTargetC;

        protected virtual IEnumerator WaitToNextTarget(float time, Transform NextTarget)
        {
            if (isActionZone && isActionZone.MoveToExitAction) time = 0;    //Do not wait if the Action Zone was a 'Move to Exit" one
           

            if (time > 0)
            {
                IsWaiting = true;
                Debuging(name + " is waiting " + time.ToString("F2") + " seconds");
                animal.Move(Vector3.zero);  //Stop the animal
                yield return new WaitForSeconds(time);
            }

            IsWaiting = false;
            SetTarget(NextTarget);

            yield return null;
        }

        /// <summary>
        /// Change velocities
        /// </summary>
        protected virtual void AutomaticSpeed()
        {
            if (RemainingDistance < ToTrot)         //Set to Walk
            {
                animal.Speed1 = true;
            }
            else if (RemainingDistance < ToRun)     //Set to Trot
            {
                animal.Speed2 = true;
            }
            else if (RemainingDistance > ToRun)     //Set to Run
            {
                animal.Speed3 = true;
            }
        }

        /// <summary>
        /// Set the next Target
        /// </summary>
        public virtual void SetTarget(Transform target)
        {
            if (target == null)
            {
                StopAnimal();
                return;             //If there's no target Skip the code
            }

            this.target = target;
            targetPosition = target.position;       //Update the Target Position 

            isActionZone = target.GetComponent<ActionZone>();
            //TisAnimal = target.GetComponent<Animal>();                (WHAT HAPPENS IF THE TARGET IS AN ANIMAL ???? ))

            isWayPoint = target.GetComponent<MWayPoint>();
            NextWayPoint = target.GetComponent<IWayPoint>();            //Check if the Next Target has Next Waypoints

            Stopped = false;

            StoppingDistance = NextWayPoint != null ? NextWayPoint.StoppingDistance : DefaultStopDistance;  //Set the Next Stopping Distance

            CheckAirTarget();

            Debuging(name + " is travelling to : " + target.name);

            if (!Agent.isOnNavMesh) return;                             //No nothing if we are not on a Nav mesh or the Agent is disabled
            Agent.enabled = true;
            Agent.SetDestination(targetPosition);                       //If there's a position to go to set it as destination
            Agent.isStopped = false;                                    //Start the Agent again
        }

        private void CheckAirTarget()
        {
            if (NextWayPoint != null && NextWayPoint.PointType == WayPointType.Air && animal.canFly)    //If the animal can fly, there's a new wayPoint & is on the Air
            {
                var AnimState = animal.CurrentAnimState;

                if (AnimState == AnimTag.Locomotion
                 || AnimState == AnimTag.Idle)          //if we are on idle or moving then go to the fly
                {
                    animal.SetFly(true);
                    flyPending = false;
                }
                else                                    //if we are in any other state then wait to be on idle or moving to set the fly
                {
                    flyPending = true;
                }
            }
        }

        /// <summary>
        /// Use this for Targets that changes their position
        /// </summary>
        public virtual void UpdateTargetTransform()
        {
            if (!Agent.isOnNavMesh) return;         //No nothing if we are not on a Nav mesh or the Agent is disabled
            if (target == null) return;             //If there's no target Skip the code
            targetPosition = target.position;       //Update the Target Position 
            Agent.SetDestination(targetPosition);   //If there's a position to go to set it as destination
            if (Agent.isStopped) Agent.isStopped = false;
        }

        /// <summary>
        /// Stop the Agent on the Animal... also remove the Transform target and the Target Position and Stops the Animal
        /// </summary>
        public virtual void StopAnimal()
        {
            if (Agent && Agent.isOnNavMesh) Agent.isStopped = true;
            targetPosition = NullVector;
            StopAllCoroutines();
            //DoingAnAction = false;
            //animal.InterruptAction();
            if (animal)   animal.Stop();
            IsWaiting = isFlyingOffMesh = false;
            Stopped = true;
        }

        /// <summary>
        /// Set a Vector Position Destination
        /// </summary>
        public virtual void SetDestination(Vector3 point)
        {
            targetPosition = point;
            target = null;                                                  //Clean the Target
            StoppingDistance = DefaultStopDistance;                         //Reset the Stopping Distance

            if (!Agent.isOnNavMesh || !Agent.enabled) return;               //Do nothing if we are not on a Nav mesh or the Agent is disabled
            Agent.SetDestination(targetPosition);                           //If there's a position to go to set it as destination
            Agent.isStopped = false;
            Stopped = false;
            Debuging(name + " is travelling to : " + point);
        }

        protected void Debuging(string Log)
        {
            if (debug) Debug.Log(Log);

            OnDebug.Invoke(Log);
        }


        internal IEnumerator CFlyOffMesh(Transform target)
        {
            animal.SetFly(true);
            flyPending = false;
            isFlyingOffMesh = true;
            float distance = float.MaxValue;
            agent.enabled = false;
            while (distance > agent.stoppingDistance)
            {
                animal.Move((target.position - animal.transform.position));
                distance = Vector3.Distance(animal.transform.position, target.position);
                yield return null;
            }
            animal.Stop();      ///lets break him out a bit so he wont fall if the movement is too fast
            animal.SetFly(false);
            isFlyingOffMesh = false;
        }

        [HideInInspector] public bool showevents;
      


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!debug) return;

            if (Agent == null) { return; }
            if (Agent.path == null) { return; }

            Color lGUIColor = Gizmos.color;

            Gizmos.color = Color.green;
            for (int i = 1; i < Agent.path.corners.Length; i++)
            {
                Gizmos.DrawLine(Agent.path.corners[i - 1], Agent.path.corners[i]);
            }


            if (AutoSpeed)
            {
                Vector3 pos = Agent ? Agent.transform.position : transform.position;
                Pivots P = GetComponentInChildren<Pivots>();
                pos.y = P.transform.position.y;

                UnityEditor.Handles.color = Color.green;
                UnityEditor.Handles.DrawWireDisc(pos, Vector3.up, ToRun);

                UnityEditor.Handles.color = Color.yellow;
                UnityEditor.Handles.DrawWireDisc(pos, Vector3.up, ToTrot);

                if (Agent)
                {
                    UnityEditor.Handles.color = Color.red;
                    UnityEditor.Handles.DrawWireDisc(pos, Vector3.up, StoppingDistance);
                }
            }
        }
#endif
    }
}
