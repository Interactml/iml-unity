using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.GroundFitter
{
    /// <summary>
    /// FM: Base class for ground fitter components, it contains variables needed for all ground fitter component in shared way
    /// </summary>
    public abstract class FGroundFitter_Base : MonoBehaviour
    {

        #region < Main Variables >

        [Header("> Main Variables <", order = 0)]
        [Space(4f, order = 1)]

        [Tooltip("How quick rotation should be corrected to target")]
        [Range(1f, 30f)]
        public float FittingSpeed = 6f;

        [Tooltip("Smoothing whole rotation motion")]
        [Range(0f, 1f)]
        public float TotalSmoother = 0f;

        [Space(3f)]
        [Tooltip("Transform which will be rotated by script, usually it can be the same transform as component's")]
        [HideInInspector]
        public Transform TransformToRotate;

        [Space(3f)]
        [Tooltip("If you want this script only to change your object's rotation and do nothing with position, untoggle this")]
        public bool GlueToGround = false;

        #endregion


        #region < Tweaking Variables >

        [Header("> Tweaking Settings <", order = 0)]
        [Space(4f, order = 1)]
        [Range(0f, 1f)]
        [Tooltip("If forward/pitch rotation value should go in lighter value than real normal hit direction")]
        public float MildForwardValue = 0f;

        [Tooltip("Maximum rotation angle in rotation of x/pitch axis, so rotating forward - degrees value of maximum rotation")]
        [Range(0f, 90f)]
        public float MaxForwardRotation = 90f;

        [Space(5f)]

        [Range(0f, 1f)]
        [Tooltip("If side rotation value/roll should go in lighter value than real normal hit direction")]
        public float MildHorizontalValue = 0f;

        [Tooltip("Max roll rotation. If rotation should work on also on x axis - good for spiders, can look wrong on quadropeds etc.")]
        [Range(0f, 90f)]
        public float MaxHorizontalRotation = 90f;

        #endregion


        #region < Advanced Variables >

        [Header("> Advanced settings <", order = 0)]
        [Space(4f, order = 1)]
        [Tooltip("We should cast raycast from position little higher than foots of your game object")]
        public float RaycastHeightOffset = 0.5f;
        [Tooltip("How far ray should cast to check if ground is under feet")]
        public float RaycastCheckRange = 5f;

        [Tooltip("If value is not equal 0 there will be casted second ray in front or back of gameObject")]
        public float LookAheadRaycast = 0f;
        [Tooltip("Blending with predicted forward raycast rotation")]
        public float AheadBlend = 0.5f;

        [Tooltip("Offset over ground")]
        [HideInInspector]
        public float UpOffset = 0f;

        [Space(8f)]
        [Tooltip("What collision layers should be included by algorithm")]
        public LayerMask GroundLayerMask = 1 << 0;
        [Tooltip("When casting down vector should adjust with transform's rotation")]
        public bool RelativeLookUp = false;
        [Range(0f, 1f)]
        public float RelativeLookUpBias = 0.0f;

        internal Vector3 WorldUp = Vector3.up;

        #endregion


        #region < Zone cast Variables >

        [Space(8f)]
        [Tooltip("Casting more raycsts under object to detect ground more precisely, then we use average from all casts to set new rotation")]
        public bool ZoneCast = false;
        public Vector2 ZoneCastDimensions = new Vector2(0.3f, 0.5f);
        public Vector3 ZoneCastOffset = Vector3.zero;
        [Range(0f, 10f)]
        public float ZoneCastBias = 0f;
        [Range(0f, 1f)]
        [Tooltip("More precision = more raycasts = lower performance")]
        public float ZoneCastPrecision = 0.25f;

        #endregion


        #region < Rest of the variables >

        /// <summary> For other classes to have access </summary>
        public RaycastHit LastRaycast { get; protected set; }
        public Vector3 LastRaycastOrigin { get; protected set; }
        public RaycastHit LastTransformRaycast { get; protected set; }
        public Quaternion LastRotation { get; protected set; }

        /// <summary> With this variable you define in which direction model should look - use it fro other scripts, check demo script for patrolling")] </summary>
        internal float UpAxisRotation = 0f;

        /// <summary> Quaternion to help controll over transform's rotation when object is placed and rotated to ground </summary>
        protected Quaternion helperRotation = Quaternion.identity;

        protected Collider selfCollider;
        protected Vector3 castOffset = Vector3.zero;
        protected float deltaTime;

        #endregion


        protected virtual void Start()
        {
            selfCollider = GetComponent<Collider>();
            if (TransformToRotate == null) TransformToRotate = transform;
            UpAxisRotation = transform.localEulerAngles.y;
        }


        protected virtual void Reset()
        {
            // Called when comonent is added to game object
            TransformToRotate = transform;
        }


        #region Rotation & Physical Calculations



        protected virtual void FitToGround()
        {
            if (selfCollider) selfCollider.enabled = false;

            // If we want we casting second ray to prevent object from clipping at big slopes
            RaycastHit aheadHit = new RaycastHit();

            if (LookAheadRaycast != 0f)
            {
                Physics.Raycast(TransformToRotate.position + GetUpVector(RaycastHeightOffset) + TransformToRotate.forward * LookAheadRaycast, -GetUpVector(), out aheadHit, RaycastCheckRange, GroundLayerMask, QueryTriggerInteraction.Ignore);
            }

            RefreshLastRaycast();

            // If cast catch ground
            if (LastRaycast.transform)
            {
                // We rotate helper transform to snap ground with smoothness
                Quaternion fromTo = Quaternion.FromToRotation(Vector3.up, LastRaycast.normal);

                // We mix both casts for average rotation
                if (aheadHit.transform)
                {
                    Quaternion aheadFromTo = Quaternion.FromToRotation(Vector3.up, aheadHit.normal);
                    fromTo = Quaternion.Lerp(fromTo, aheadFromTo, AheadBlend);
                }

                helperRotation = Quaternion.Slerp(helperRotation, fromTo, deltaTime * FittingSpeed);
            }
            else // If nothing is under our legs we rotate object smoothly to zero rotations
            {
                helperRotation = Quaternion.Slerp(helperRotation, Quaternion.identity, deltaTime * FittingSpeed);
            }

            RotationCalculations();

            if (GlueToGround)
                if (LastRaycast.transform)
                    TransformToRotate.position = LastRaycast.point + Vector3.up * UpOffset;

            if (selfCollider) selfCollider.enabled = true;
        }



        internal virtual void RotationCalculations()
        {
            // Then we can rotate object to target look in y axis
            Quaternion targetRotation = helperRotation;

            targetRotation = Quaternion.Euler(Mathf.Clamp(FLogicMethods.WrapAngle(targetRotation.eulerAngles.x), -MaxForwardRotation, MaxForwardRotation) * (1 - MildForwardValue), targetRotation.eulerAngles.y, Mathf.Clamp(FLogicMethods.WrapAngle(targetRotation.eulerAngles.z), -MaxHorizontalRotation, MaxHorizontalRotation) * (1 - MildHorizontalValue));

            if (TotalSmoother == 0f)
            {
                TransformToRotate.rotation = targetRotation * Quaternion.AngleAxis(UpAxisRotation, Vector3.up);
            }
            else
            {
                Quaternion yRotation = Quaternion.AngleAxis(UpAxisRotation, Vector3.up);
                TransformToRotate.rotation *= Quaternion.Inverse(yRotation);
                TransformToRotate.rotation = Quaternion.Slerp(TransformToRotate.rotation, targetRotation, deltaTime * Mathf.Lerp(50f, 1f, TotalSmoother));
                TransformToRotate.rotation *= yRotation;
            }

            LastRotation = TransformToRotate.rotation;
        }



        internal virtual RaycastHit CastRay()
        {
            // Cast ground to get data about surface hit
            RaycastHit outHit;
            LastRaycastOrigin = GetRaycastOrigin() + castOffset;
            Physics.Raycast(LastRaycastOrigin, -GetUpVector(), out outHit, RaycastCheckRange + Mathf.Abs(UpOffset), GroundLayerMask, QueryTriggerInteraction.Ignore);

            // Making zone cast calculations to create average ray hit
            if (ZoneCast)
            {
                Vector3 p = TransformToRotate.position + GetRotation() * ZoneCastOffset + GetUpVector(RaycastHeightOffset);
                Vector3 x = TransformToRotate.right * ZoneCastDimensions.x;
                Vector3 z = TransformToRotate.forward * ZoneCastDimensions.y;

                List<RaycastHit> hits = new List<RaycastHit>();
                hits.Add(outHit);

                RaycastHit newHit;

                int c = 0;
                float part = 1f;
                for (int i = 0; i < Mathf.Lerp(4, 24, ZoneCastPrecision); i++)
                {
                    Vector3 sum = Vector3.zero;

                    if (c == 0) sum = x - z;
                    else if (c == 1) sum = x + z;
                    else if (c == 2) sum = -x + z;
                    else if (c == 3)
                    {
                        sum = -x - z;
                        part += 0.75f;
                        c = -1;
                    }

                    Physics.Raycast(p + sum / part, -GetUpVector() + sum * ZoneCastBias + castOffset, out newHit, RaycastCheckRange + Mathf.Abs(UpOffset), GroundLayerMask, QueryTriggerInteraction.Ignore);
                    if (newHit.transform) hits.Add(newHit);

                    c++;
                }

                Vector3 avPos = Vector3.zero;
                Vector3 avNormal = Vector3.zero;

                for (int i = 0; i < hits.Count; i++)
                {
                    avNormal += hits[i].normal;
                    avPos += hits[i].point;
                }

                avPos /= hits.Count;
                avNormal /= hits.Count;

                outHit.normal = avNormal;
                if (!outHit.transform)
                {
                    outHit.point = new Vector3(avPos.x, TransformToRotate.position.y, avPos.z);
                }
            }

            return outHit;
        }




        #endregion


        #region Utilities

        internal Vector3 GetRaycastOrigin()
        {
            return TransformToRotate.position + GetUpVector() * RaycastHeightOffset;
        }

        //protected virtual Vector3 GetUpVector()
        //{
        //    if (RelativeLookUp)
        //        return Vector3.Lerp(WorldUp, TransformToRotate.TransformDirection(Vector3.up).normalized, RelativeLookUpBias);
        //    else
        //        return WorldUp;
        //}


        protected virtual Quaternion GetRotation()
        {
            return TransformToRotate.rotation;
        }

        protected virtual Vector3 GetUpVector(float mulRange = 1f)
        {
            if (RelativeLookUp)
                return Vector3.Lerp(WorldUp, TransformToRotate.TransformDirection(Vector3.up).normalized, RelativeLookUpBias) * mulRange;
            else
                return WorldUp * mulRange;
        }

        internal void RotateBack(float speed = 5f)
        {
            if (speed <= 0f) return;
            helperRotation = Quaternion.Slerp(helperRotation, Quaternion.identity, deltaTime * speed);
        }

        internal void RefreshLastRaycast()
        {
            LastRaycast = CastRay();
            if (LastRaycast.transform) LastTransformRaycast = LastRaycast;
        }

        internal void BackRaycast()
        {
            LastRaycast = LastTransformRaycast;
        }


        #endregion


        #region Gizmos Stuff


#if UNITY_EDITOR

        [Space(2f, order = 0)]
        [Header("> Debugs <", order = 1)]
        [Space(4f, order = 2)]
        public bool drawDebug = true;

        // Set it to false if you don't want show clickable gizmo
        [Space(1f)]
        public bool drawGizmo = true;

        protected virtual void OnDrawGizmos()
        {
            if (TransformToRotate == null) TransformToRotate = transform;

            Gizmos.color = new Color(1f, 0.2f, 0.4f, 0.8f);

            // Drawing fat main cast ray to be more visible
            float scale = RaycastCheckRange;
            Gizmos.DrawRay(TransformToRotate.position + GetUpVector(RaycastHeightOffset), GetUpVector() * -RaycastCheckRange);
            Gizmos.DrawRay(TransformToRotate.position + TransformToRotate.TransformVector(Vector3.one * 0.001f * scale) + GetUpVector(RaycastHeightOffset), GetUpVector() * -RaycastCheckRange);
            Gizmos.DrawRay(TransformToRotate.position + TransformToRotate.TransformVector(-Vector3.one * 0.001f * scale) + GetUpVector(RaycastHeightOffset), GetUpVector() * -RaycastCheckRange);
            Gizmos.DrawRay(TransformToRotate.position + TransformToRotate.TransformVector(Vector3.one * 0.002f * scale) + GetUpVector(RaycastHeightOffset), GetUpVector() * -RaycastCheckRange);
            Gizmos.DrawRay(TransformToRotate.position + TransformToRotate.TransformVector(-Vector3.one * 0.002f * scale) + GetUpVector(RaycastHeightOffset), GetUpVector() * -RaycastCheckRange);
            Gizmos.DrawRay(TransformToRotate.position + TransformToRotate.TransformVector(-Vector3.one * 0.005f * scale) + GetUpVector(RaycastHeightOffset), GetUpVector() * -RaycastCheckRange);
            Gizmos.DrawRay(TransformToRotate.position + TransformToRotate.TransformVector(-Vector3.one * 0.005f * scale) + GetUpVector(RaycastHeightOffset), GetUpVector() * -RaycastCheckRange);

            if (LookAheadRaycast != 0f)
            {
                Gizmos.color = new Color(0.2f, 0.4f, 1f, 0.8f);
                Gizmos.DrawRay(TransformToRotate.position + GetUpVector(RaycastHeightOffset) + TransformToRotate.forward * LookAheadRaycast, -GetUpVector() * (RaycastCheckRange /*+ YOffset*/));
            }

            if (ZoneCast)
            {
                Vector3 p = TransformToRotate.position + GetRotation() * ZoneCastOffset + GetUpVector(RaycastHeightOffset);
                Vector3 x = TransformToRotate.right * ZoneCastDimensions.x;
                Vector3 z = TransformToRotate.forward * ZoneCastDimensions.y;

                Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.8f);

                // Drawing rectangle for guide how zone rays will go
                Gizmos.DrawLine(p + x - z, p + x + z);
                Gizmos.DrawLine(p + x + z, p - x + z);
                Gizmos.DrawLine(p - x + z, p - x - z);
                Gizmos.DrawLine(p - x - z, p + x - z);

                Vector3 downDir = -GetUpVector() * RaycastCheckRange;

                int c = 0;
                float part = 1f;
                for (int i = 0; i < Mathf.Lerp(4, 24, ZoneCastPrecision); i++)
                {
                    Vector3 sum = Vector3.zero;

                    if (c == 0) sum = x - z;
                    else if (c == 1) sum = x + z;
                    else if (c == 2) sum = -x + z;
                    else if (c == 3)
                    {
                        sum = -x - z;
                        part += 0.75f;
                        if (part > 2.5f) part += 1f;
                        if (part > 5.5f) part += 3f;
                        if (part > 10f) part += 7f;
                        c = -1;
                    }

                    Gizmos.DrawRay(p + sum / part, downDir + sum * ZoneCastBias);

                    c++;
                }
            }

            if (!drawGizmo) return;
            if (TransformToRotate) Gizmos.DrawIcon(TransformToRotate.position, "FIMSpace/GroundFitter/SPR_GroundFitter Gizmo.png", true);
        }

#endif

        #endregion

    }


    #region Custom Inspector Window Stuff

#if UNITY_EDITOR
    /// <summary>
    /// FM: Editor class component to enchance controll over component from inspector window
    /// </summary>
    [UnityEditor.CustomEditor(typeof(FGroundFitter_Base))]
    [UnityEditor.CanEditMultipleObjects]
    public class FGroundFitter_BaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            FGroundFitter_Base targetScript = (FGroundFitter_Base)target;
            List<string> exclude = new List<string>();

            if (targetScript.LookAheadRaycast == 0f)
            {
                exclude.Add("AheadBlend");
            }

            if (!targetScript.RelativeLookUp)
            {
                exclude.Add("RelativeLookUpBias");
            }

            if (!targetScript.ZoneCast)
            {
                exclude.Add("ZoneCastDimensions");
                exclude.Add("ZoneCastOffset");
                exclude.Add("ZoneCastBias");
                exclude.Add("ZoneCastPrecision");
            }

            DrawPropertiesExcluding(serializedObject, exclude.ToArray());
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    #endregion

}