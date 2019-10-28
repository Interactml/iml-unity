///TO DO
///Set on which State the action can enabled  (locomotion, jumping, flying.. etc)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MalbersAnimations.Events;
using UnityEngine.Events;
using System;
using MalbersAnimations.Utilities;

namespace MalbersAnimations
{
    [RequireComponent(typeof(BoxCollider))]
    public class ActionZone : MonoBehaviour, IWayPoint
    {
        #region Variables

        static Keyframe[] K = { new Keyframe(0, 0), new Keyframe(1, 1) };

        public Action ID;                    //Action to Use

        public bool automatic;                          //Set the Action Zone to Automatic
        public int index;                               //Index of the Action Zone (List index)
        public float AutomaticDisabled = 10f;           //is Automatic is set to true this will be the time to disable temporarly the Trigger
        public bool HeadOnly;                           //Use the Trigger for heads only
        public bool ActiveOnJump = false;
        /// <summary>
        /// The Action Requires the Animal to Wake Up/Move
        /// </summary>
        public bool MoveToExitAction = false;


        public bool Align;                              //Align the Animal entering to the Aling Point
        /// <summary>
        /// Enable this to align only with the Lenght of the Zone not just a point
        /// </summary>
        public bool AlignWithWidth;
        public Transform AlingPoint;
        public Transform AlingPoint2;
        public float AlignTime = 0.5f;
        public AnimationCurve AlignCurve = new AnimationCurve(K);

        public bool AlignPos = true, AlignRot = true, AlignLookAt = false;

        protected List<Collider> animal_Colliders  = new List<Collider>();
        protected Animal oldAnimal;
        public float ActionDelay = 0;
        //public AnimalProxy animalProxy; //This is used to Get all the funtions of any animal that gets to the zone..

        public AnimalEvent OnEnter = new AnimalEvent();
        public AnimalEvent OnExit = new AnimalEvent();
        public AnimalEvent OnAction = new AnimalEvent();

        [MinMaxRange(0, 60)]
        [SerializeField]
        private RangedFloat waitTime = new RangedFloat(0, 5);
        public WayPointType pointType = WayPointType.Ground;
        public static List<ActionZone> ActionZones;     //Keep a Track of all the Zones on the Scene

        #region AI
        [SerializeField]
        private List<Transform> nextTargets;
        public List<Transform> NextTargets
        {
            get { return nextTargets; }
            set { nextTargets = value; }
        }

        public Transform NextTarget
        {
            get
            {
                if (NextTargets.Count > 0)
                {
                    return NextTargets[UnityEngine.Random.Range(0, NextTargets.Count)];
                }
                return null;
            }
        }

        public WayPointType PointType
        {
            get { return pointType; }
        }


        public float WaitTime
        {
            get { return waitTime.RandomValue; }
        }

        [SerializeField]
        private float stoppingDistance = 0.5f;
        public float StoppingDistance
        {
            get { return stoppingDistance; }
            set { stoppingDistance = value; }
        }
        #endregion


        Collider ZoneCollider;

        #endregion

        void OnTriggerEnter(Collider other)
        {
            if (!MalbersTools.CollidersLayer(other, LayerMask.GetMask("Animal"))) return; //Just accept animal layer only

            if (HeadOnly && !other.name.ToLower().Contains("head")) return;         //If is Head Only and no head was found Skip

            Animal newAnimal = other.GetComponentInParent<Animal>();                //Get the animal on the entering collider

            if (!newAnimal) return;                                                 //If there's no animal do nothing

            newAnimal.ActionID = ID;                                                //Set the ID on the ANIMAL that entered the Action


            if (animal_Colliders.Find(coll => coll == other) == null)               //if the entering collider is not already on the list add it
            {
                animal_Colliders.Add(other);
            }


            if (newAnimal == oldAnimal) return;                                     //if the animal is the same do nothing (when entering two animals on the same Zone)
            else
            {
                if (oldAnimal)
                {
                    oldAnimal.ActionID = -1;                            //Remove the old animal and remove the Action ID
                    animal_Colliders = new List<Collider>();                           //Clean the colliders
                }

                oldAnimal = newAnimal;                                             //Set a new Animal
               
               // if (animalProxy) animalProxy.SetAnimal(newAnimal);                 //Set to the Proxy the current animal entering the zone
            }

            newAnimal.OnAction.AddListener(OnActionListener);                      //Listen when the animal activate the Action Input

            OnEnter.Invoke(newAnimal);


            if (automatic)       //Just activate when is on the Locomotion State if this is automatic
            {
                if (newAnimal.AnimState == AnimTag.Jump && !ActiveOnJump) return;   //Dont start an automatic action if is jumping and active on jump is disabled
               // newAnimal.OnAction.RemoveListener(OnActionListener);
                newAnimal.SetAction(ID);
                StartCoroutine(ReEnable(newAnimal));
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (HeadOnly && !other.name.ToLower().Contains("head")) return;         //if is only set to head and there's no head SKIP

            Animal exiting_animal = other.GetComponentInParent<Animal>();
            if (!exiting_animal) return;                                            //If there's no animal script found skip all
            if (exiting_animal != oldAnimal) return;                                //If is another animal exiting the zone SKIP

            if (animal_Colliders.Find(item => item == other))                       //Remove the collider from the list that is exiting the zone.
            {
                animal_Colliders.Remove(other);
            }

            if (animal_Colliders.Count == 0)                                        //When all the collides are removed from the list..
            {
                OnExit.Invoke(oldAnimal);                                           //Invoke On Exit when all animal's colliders has exited the Zone

                if (oldAnimal.ActionID == ID)
                {
                    oldAnimal.ActionID = -1;              //Reset the Action ID if we have the same
                }
                oldAnimal = null;

                //if (animalProxy) animalProxy.SetAnimal(null);                        //Set to the Proxy the current animal entering the zone
            }
        }

        /// <summary>
        /// This will disable the Collider on the action zone
        /// </summary>
        IEnumerator ReEnable(Animal animal) //For Automatic only 
        {
            if (AutomaticDisabled > 0)
            {
                ZoneCollider.enabled = false;
                yield return null;
                yield return null;
                animal.ActionID = -1;
                yield return new WaitForSeconds(AutomaticDisabled);
                ZoneCollider.enabled = true;
            }
            oldAnimal = null;       //clean animal
            animal_Colliders = new List<Collider>();      //Reset Colliders
            yield return null;
        }

        public virtual void _DestroyActionZone(float time)
        {
            Destroy(gameObject, time);
        }

        /// <summary>
        /// Used for checking if the animal enables the action
        /// </summary>
        private void OnActionListener()
        {
            if (!oldAnimal) return;                             //Skip if there's no animal

            StartCoroutine(OnActionDelay(ActionDelay, oldAnimal));              //Invoke the Event OnAction

            if (Align && AlingPoint)
            {
                IEnumerator ICo = null;

                Vector3 AlingPosition = AlingPoint.position;

                if (AlingPoint2)                //In case there's 
                {
                    AlingPosition = MalbersTools.ClosestPointOnLine(AlingPoint.position, AlingPoint2.position, oldAnimal.transform.position);
                }

                if (AlignLookAt)
                {
                    ICo = MalbersTools.AlignLookAtTransform(oldAnimal.transform, AlingPoint, AlignTime, AlignCurve);                //Align Look At the Zone
                    StartCoroutine(ICo);
                }
                else
                {
                    if (AlignPos) StartCoroutine(MalbersTools.AlignTransform_Position(oldAnimal.transform, AlingPosition, AlignTime, AlignCurve));
                    if (AlignRot) StartCoroutine(MalbersTools.AlignTransform_Rotation(oldAnimal.transform, AlingPoint.rotation, AlignTime, AlignCurve));
                }
            }
            StartCoroutine(CheckForCollidersOff());
        }

        IEnumerator OnActionDelay(float time, Animal animal)
        {
            if (time > 0)
            {
                yield return new WaitForSeconds(time);
            }

            OnAction.Invoke(animal);

            yield return null;
        }

        IEnumerator CheckForCollidersOff() // Used on  when the animal disable its colliders
        {
            yield return null;
            yield return null;          //Wait 2 frames

            if (oldAnimal && !oldAnimal.ActiveColliders)
            {
                oldAnimal.OnAction.RemoveListener(OnActionListener);
                oldAnimal.ActionID = -1;
                oldAnimal = null;
                animal_Colliders = new List<Collider>();      //Reset Colliders
            }
        }


        /// <summary>
        /// Used to wakeUp the animal after entering an action like Seat, Sleep, Lie
        /// </summary>
        public virtual void _WakeAnimal(Animal animal)
        {
            if (animal)
                animal.MovementAxis = (Vector3.forward)*3;
        }

        void OnEnable()
        {
            if (ActionZones == null) ActionZones = new List<ActionZone>();
            ZoneCollider = GetComponent<Collider>();                                   //Get the reference for the collider
            ActionZones.Add(this);                                                  //Save the the Action Zones on the global Action Zone list
        }
        void OnDisable()
        {
            ActionZones.Remove(this);                                              //Remove the the Action Zones on the global Action Zone list
            if (oldAnimal)
            {
                oldAnimal.OnAction.RemoveListener(OnActionListener);
                oldAnimal.ActionID = -1;
            }
        }


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (AlingPoint && AlingPoint2)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(AlingPoint.position, AlingPoint2.position);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (EditorAI)
            {
                UnityEditor.Handles.color = Color.red;
                UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, StoppingDistance);

                Gizmos.color = Color.green;
                if (nextTargets != null)
                    foreach (var item in nextTargets)
                    {
                        if (item) Gizmos.DrawLine(transform.position, item.position);
                    }
            }
        }
#endif

        [HideInInspector] public bool EditorShowEvents = true;
        [HideInInspector] public bool EditorAI = true;

 
    }
}