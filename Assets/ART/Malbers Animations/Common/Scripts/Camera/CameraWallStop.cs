using MalbersAnimations.Utilities;
using System;
using System.Collections;
using UnityEngine;
namespace MalbersAnimations
{
    public class CameraWallStop : MonoBehaviour
    {
        public float clipMoveTime = 0.05f;              // time taken to move when avoiding cliping (low value = fast, which it should be)
        public float returnTime = 0.4f;                 // time taken to move back towards desired position, when not clipping (typically should be a higher value than clipMoveTime)
        public float sphereCastRadius = 0.15f;           // the radius of the sphere used to test for object between camera and target
        public bool visualiseInEditor;                  // toggle for visualising the algorithm through lines for the raycast in the editor
        public float closestDistance = 0.5f;            // the closest distance the camera can be from the target
        public bool protecting { get; private set; }    // used for determining if there is an object between the target and the camera
        public LayerMask dontClip = 1 << 20;            //Dont clip animal

        private Transform m_Cam;                        // the transform of the camera
        private Transform m_Pivot;                      // the point at which the camera pivots around
        private float m_OriginalDist;                   // the original distance to the camera before any modification are made
        private float m_MoveVelocity;                   // the velocity at which the camera moved
        private float m_CurrentDist;                    // the current distance from the camera to the target
        private Ray m_Ray = new Ray();                  // the ray used in the lateupdate for casting between the camera and the target
        private RaycastHit[] hits;                      // the hits between the camera and the target
        private RayHitComparer m_RayHitComparer;        // variable to compare raycast hit distances


        private void Start()
        {
            
            m_Cam = GetComponentInChildren<Camera>().transform;     //find the camera in the object hierarchy
            m_Pivot = m_Cam.parent;
            m_OriginalDist = m_Cam.localPosition.magnitude;
            m_CurrentDist = m_OriginalDist;

            m_RayHitComparer = new RayHitComparer();         // create a new RayHitComparer
        }


        private void LateUpdate()
        {
            float targetDist = m_OriginalDist;                                  // initially set the target distance

            m_Ray.origin = m_Pivot.position + m_Pivot.forward * sphereCastRadius;
            m_Ray.direction = -m_Pivot.forward;

            var cols = Physics.OverlapSphere(m_Ray.origin, sphereCastRadius);   // initial check to see if start of spherecast intersects anything

            bool initialIntersect = false;
            bool hitSomething = false;

            for (int i = 0; i < cols.Length; i++)                   // loop through all the collisions to check if something we care about
            {
                if ((!cols[i].isTrigger) && !(MalbersTools.CollidersLayer(cols[i],dontClip)))   //is on a layer we don't want to clip
                {
                    initialIntersect = true;
                    break;
                }
            }
           
            if (initialIntersect)                                                               // if there is a collision
            {
                m_Ray.origin += m_Pivot.forward * sphereCastRadius;
                hits = Physics.RaycastAll(m_Ray, m_OriginalDist - sphereCastRadius);            // do a raycast and gather all the intersections
            }
            else        // if there was no collision do a sphere cast to see if there were any other collisions
            {
                hits = Physics.SphereCastAll(m_Ray, sphereCastRadius, m_OriginalDist + sphereCastRadius);
            }

           
            Array.Sort(hits, m_RayHitComparer);                         // sort the collisions by distance

           
            float nearest = Mathf.Infinity;                             // set the variable used for storing the closest to be as far as possible

           
            for (int i = 0; i < hits.Length; i++)                       // loop through all the collisions
            {
                // only deal with the collision if it was closer than the previous one, not a trigger, not in the Layer Mask
                if (hits[i].distance < nearest && (!hits[i].collider.isTrigger) &&  !MalbersTools.CollidersLayer(hits[i].collider,dontClip))
                {
                    nearest = hits[i].distance;                                         // change the nearest collision to latest
                    targetDist = -m_Pivot.InverseTransformPoint(hits[i].point).z;
                    hitSomething = true;
                }
            }

                
            if (hitSomething)           // visualise the cam clip effect in the editor
            {
                Debug.DrawRay(m_Ray.origin, -m_Pivot.forward * (targetDist + sphereCastRadius), Color.red);
            }

            // hit something so move the camera to a better position
            protecting = hitSomething;
            m_CurrentDist = Mathf.SmoothDamp(m_CurrentDist, targetDist, ref m_MoveVelocity,
                                           m_CurrentDist > targetDist ? clipMoveTime : returnTime);
            m_CurrentDist = Mathf.Clamp(m_CurrentDist, closestDistance, m_OriginalDist);
            m_Cam.localPosition = -Vector3.forward * m_CurrentDist;
        }


        // comparer for check distances in ray cast hits
        public class RayHitComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
            }
        }
    }
}

