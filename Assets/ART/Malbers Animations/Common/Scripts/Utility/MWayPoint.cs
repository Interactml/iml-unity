using MalbersAnimations.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Utilities
{
    

    public class MWayPoint : MonoBehaviour, IWayPoint
    {
        public static List<MWayPoint> WayPoints;

        [SerializeField]
        private float stoppingDistance = 1;
        public float StoppingDistance
        {
            get { return stoppingDistance; }
            set { stoppingDistance = value; }
        }

        [MinMaxRange(0,60)]
        public RangedFloat waitTime = new RangedFloat(0,15);
        public WayPointType pointType = WayPointType.Ground;
        public float WaitTime
        {
            get { return waitTime.RandomValue; }
        }

        [SerializeField]
        private List<Transform> nextWayPoints;
        public List<Transform> NextTargets
        {
            get { return nextWayPoints; }
            set { nextWayPoints = value; }
        }

        public Transform NextTarget
        {
            get
            {
                if (NextTargets.Count > 0)
                {
                    return NextTargets[Random.Range(0, NextTargets.Count)];
                }
                return null;
            }
        }

        public WayPointType PointType
        {
            get { return pointType; }
        }

        [Space]
        [Space]
        public  ComponentEvent OnTargetArrived = new ComponentEvent();

        public bool debug = true;

        void OnEnable()
        {
            if (WayPoints == null)
            {
                WayPoints = new List<MWayPoint>();
            }

            WayPoints.Add(this);
        }

        void OnDisable()
        {
            WayPoints.Remove(this);
        }

        public void TargetArrived(Component target)
        {
            OnTargetArrived.Invoke(target);
        }

        /// <summary>
        /// Returns a Random Waypoint from the Global WaypointList
        /// </summary>
        public static Transform GetWaypoint()
        {
            if (WayPoints != null && WayPoints.Count > 1)
            {
                return WayPoints[Random.Range(0, WayPoints.Count)].transform;
            }
            return null;
        }

        /// <summary>
        /// Returns a Random Waypoint from the Global WaypointList by its type (Ground, Air, Water)
        /// </summary>
        public static Transform GetWaypoint(WayPointType pointType)
        {
            if (WayPoints != null && WayPoints.Count > 1)
            {
                var MWayPoint = WayPoints.Find(item => item.pointType == pointType);

                return MWayPoint ? MWayPoint.transform : null;
            }
            return null;
        }



#if UNITY_EDITOR
        /// <summary>
        /// DebugOptions
        /// </summary>
        void OnDrawGizmos()
        {
            if (debug)
            {
                UnityEditor.Handles.color = Color.red;
                Gizmos.color = Color.red;

                if (pointType == WayPointType.Air)
                {
                    Gizmos.DrawWireSphere(transform.position, StoppingDistance);
                }
                else
                {
                    UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, StoppingDistance);
                }

                UnityEditor.Handles.color = Color.white;
                UnityEditor.Handles.Label(transform.position, name);

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, 0.25f);

              
                Gizmos.color = new Color(1, 0, 0, 0.4f);
                Gizmos.DrawSphere(transform.position, 0.25f);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (debug)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 0.25f);

                Gizmos.color = Color.green;
                foreach (var item in nextWayPoints)
                {
                    if (item) Gizmos.DrawLine(transform.position, item.position);
                }
            }
        }
#endif
    }
}
