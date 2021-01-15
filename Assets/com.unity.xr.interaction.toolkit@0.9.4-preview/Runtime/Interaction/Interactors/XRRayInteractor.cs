using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interactor used for interacting with interactables at a distance.  This is handled via raycasts
    /// that update the current set of valid targets for this interactor.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("XR/XR Ray Interactor")]
	public class XRRayInteractor : XRBaseControllerInteractor, IUIInteractable
    {
        private class RaycastHitComparer : IComparer<RaycastHit>
        {
            public int Compare(RaycastHit a, RaycastHit b)
            {
                float aDistance = a.collider != null ? a.distance : float.MaxValue;
                float bDistance = b.collider != null ? b.distance : float.MaxValue;
                int result = aDistance.CompareTo(bDistance);
                return result;
            }
        }

        const float k_DefaultMaxRaycastDistance     = 30f;
        const float k_DefaultHoverTimeToSelect      = 0.5f;
        const int   kMaxRaycastHits                 = 10;

        const int k_MinLineSamples = 2;
        const int k_MaxLineSamples = 100;

        /// <summary>Options of line types.</summary>
        public enum LineType 
        {
            StraightLine = 0,  
            ProjectileCurve = 1,
            BezierCurve  = 2,            
        }

        public enum HitDetectionType
        {
            Raycast = 0,
            SphereCast = 1,
        }
       
        [SerializeField]
        LineType m_LineType = LineType.StraightLine;
        /// <summary>Gets or sets the type of ray cast.</summary>
        public LineType lineType { get { return m_LineType; } set { m_LineType = value; RebuildSamplePoints(); } }

        [SerializeField]
        float m_MaxRaycastDistance = k_DefaultMaxRaycastDistance;
        /// <summary>Gets or sets the max distance of ray cast. Increase this value will let you reach further.</summary>
        public float maxRaycastDistance { get { return m_MaxRaycastDistance; } set { m_MaxRaycastDistance = value; } }

        [SerializeField]
        Transform m_ReferenceFrame;
        /// <summary>Gets or sets the reference frame of the projectile.
        /// If not set it will try to find the XRRig object, if the XRRig does not exist it will use self. /// </summary>
        public Transform referenceFrame { get { return m_ReferenceFrame; } set { m_ReferenceFrame = value; } }

        [SerializeField]
        float m_Velocity = 16f;
        /// <summary>Initial velocity of the projectile. Increase this value will make the curve reach further.</summary>
        public float Velocity { get { return m_Velocity; } set { m_Velocity = value; } }

        [SerializeField]
        float m_Acceleration = 9.8f;
        /// <summary>Gravity of the projectile in the reference frame.</summary>
        public float Acceleration { get { return m_Acceleration; } set { m_Acceleration = value; } }

        [SerializeField]
        float m_AdditionalFlightTime = 0.5f;
        /// <summary>Additional flight time after the projectile lands at the same height of the start point in the tracking space.
        ///Increase this value will make the end point drop lower in height.</summary>
        public float AdditionalFlightTime { get { return m_AdditionalFlightTime; } set { m_AdditionalFlightTime = value; } }
                
        [SerializeField]
        float m_EndPointDistance = 30f;
        /// <summary>Increase this value distance will make the end of curve further from the start point.</summary>
        public float endPointDistance { get { return m_EndPointDistance; } set { m_EndPointDistance = value; } }
        
        [SerializeField]
        float m_EndPointHeight = -10f;
        /// <summary>Decrease this value will make the end of the curve drop lower relative to the start point.</summary>
        public float endPointHeight { get { return m_EndPointHeight; } set { m_EndPointHeight = value; } }

        [SerializeField]
        float m_ControlPointDistance = 10f;
        /// <summary>Increase this value will make the peak of the curve further from the start point.</summary>
        public float controlPointDistance { get { return m_ControlPointDistance; } set { m_ControlPointDistance = value; } }

        [SerializeField]
        float m_ControlPointHeight = 5f;
        /// <summary>Increase this value will make the peak of the curve higher relative to the start point.</summary>
        public float controlPointHeight { get { return m_ControlPointHeight; } set { m_ControlPointHeight = value; } }

        [SerializeField]
        [Range(k_MinLineSamples, k_MaxLineSamples)] int m_SampleFrequency = 20;
        /// <summary>Gets or sets the number of sample points of the curve, should be at least 3, 
        /// the higher the better quality. </summary>
        public int sampleFrequency { get { return m_SampleFrequency; } set { m_SampleFrequency = value; RebuildSamplePoints(); } }

        [SerializeField]
        HitDetectionType m_HitDetectionType = HitDetectionType.Raycast;
        /// <summary>Sets which type of hit detection to use for the raycast. </summary>
        public HitDetectionType hitDetectionType { get { return m_HitDetectionType; } set { m_HitDetectionType = value; } }

        [SerializeField]
        float m_SphereCastRadius;
        /// <summary>Gets or sets radius used for spherecasting. Will use regular raycasting if set to 0.0f or less.</summary>
        public float sphereCastRadius { get { return m_SphereCastRadius; } set { m_SphereCastRadius = value; } }

        [SerializeField]
        LayerMask m_RaycastMask = -1;
        /// <summary>Gets or sets layer mask used for limiting raycast targets.</summary>
        public LayerMask raycastMask { get { return m_RaycastMask; } set { m_RaycastMask = value; } }

        [SerializeField]
        QueryTriggerInteraction m_RaycastTriggerInteraction = QueryTriggerInteraction.Ignore;
        /// <summary>Gets or sets type of interaction with trigger volumes via raycast.</summary>
        public QueryTriggerInteraction raycastTriggerInteraction { get { return m_RaycastTriggerInteraction; } set { m_RaycastTriggerInteraction = value; } }

        [SerializeField]
        bool m_HoverToSelect;
        /// <summary>Gets or sets whether this uses hovering for a time period to select the interactable being hovered.</summary>
        public bool hoverToSelect { get { return m_HoverToSelect; } set { m_HoverToSelect = value; } }

        [SerializeField]
        float m_HoverTimeToSelect = k_DefaultHoverTimeToSelect;
        /// <summary>Gets or sets the number of seconds for which this interactor must hover over an object to select it if Hover To Select is enabled.</summary>
        public float hoverTimeToSelect { get { return m_HoverTimeToSelect; } set { m_HoverTimeToSelect = value; } }

        [SerializeField]
        bool m_EnableUIInteraction = true;
        /// <summary>Gets or sets whether this interactor is able to affect UI.</summary>
        public bool enableUIInteraction
        {
            get
            {
                return m_EnableUIInteraction;
            }
            set
            {
                if(m_EnableUIInteraction != value)
                {
                    m_EnableUIInteraction = value;
                    if (enabled)
                    {
                        if(m_EnableUIInteraction)
                        {
                            m_InputModule.RegisterInteractable(this);
                        }
                        else
                        {
                            m_InputModule.UnregisterInteractable(this);
                        }
                    }
                }     
            }
        }

        /// <summary>Gets the signed angle between the controller's forward direction and the tracking space.</summary>
        public float Angle
        {
            get
            {
                Vector3 projectedForward = Vector3.ProjectOnPlane(m_StartTransform.forward, m_ReferenceFrame.up);
                if (Vector3.Angle(m_StartTransform.forward, projectedForward) == 0)
                    return 0;
                else
                    return Vector3.SignedAngle(m_StartTransform.forward, projectedForward, Vector3.Cross(m_StartTransform.forward, projectedForward));
            }
        }

        protected override List<XRBaseInteractable> ValidTargets { get { return m_ValidTargets; } }

        // reusable array of raycast hits
        RaycastHit[]    m_RaycastHits           = new RaycastHit[kMaxRaycastHits];
        RaycastHitComparer m_RaycastHitComparer = new RaycastHitComparer();

        // reusable list of valid targets
        List<XRBaseInteractable> m_ValidTargets = new List<XRBaseInteractable>();

        // reusable list of sample points
        Vector3[] m_SamplePoints = null;
        int m_NoSamplePoints = -1;

        // state to manage hover selection
        XRBaseInteractable m_CurrentNearestObject;
        float m_LastTimeHoveredObjectChanged;
        bool m_PassedHoverTimeToSelect = false;

        int m_HitCount = 0;
        int m_HitPositionInLine = -1;

        // Control points to calculate the bezier curve
        Vector3[] m_ControlPoints = new Vector3[3];

        /// <summary>The starting transform of any Raycasts.  Default value is the controller transform, or the attachTransform if it is not null.</summary>
        Transform m_StartTransform { get { return attachTransform ?? transform; } }

        // Input Module for fast access to UI systems.
        XRUIInputModule m_InputModule;

        // Used by UpdateUIModel to retrieve the line points to pass along to Unity UI.
        static Vector3[] s_CachedLinePoints;

        void RebuildSamplePoints()
        {
            if (m_LineType == LineType.StraightLine)
            {
                if(m_SamplePoints == null || m_SamplePoints.Length != 2)
                    m_SamplePoints = new Vector3[2];
            }
            else
            {
                if(m_SamplePoints == null || m_SamplePoints.Length != m_SampleFrequency)
                    m_SamplePoints = new Vector3[m_SampleFrequency];
            }
            m_NoSamplePoints = 0;
        }

        void FindOrCreateXRUIInputModule()
        {
            var eventSystem = Object.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
                eventSystem = new GameObject("Event System", typeof(EventSystem)).GetComponent<EventSystem>();

            m_InputModule = eventSystem.GetComponent<XRUIInputModule>();
            if (m_InputModule == null)
                m_InputModule = eventSystem.gameObject.AddComponent<XRUIInputModule>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RebuildSamplePoints();
            FindReferenceFrame();

            if(m_EnableUIInteraction)
            {
                FindOrCreateXRUIInputModule();
                m_InputModule.RegisterInteractable(this);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // clear lines
            m_NoSamplePoints = -1;

            if(m_EnableUIInteraction)
            {
                m_InputModule.UnregisterInteractable(this);
            }
            m_InputModule = null;
        }

        /// <summary> This function implements the ILineRenderable interface and returns the sample points of the line. </summary>
        public bool GetLinePoints(ref Vector3[] linePoints, ref int noPoints)
        {
    
            if (m_SamplePoints == null || m_SamplePoints.Length < 2 || m_NoSamplePoints < 2)
            {
                return false;
            }
            else
            {
                if (linePoints == null)
                {
                    linePoints = new Vector3[m_NoSamplePoints];
                }

                if (linePoints.Length < m_NoSamplePoints)
                {
                    linePoints = new Vector3[m_NoSamplePoints];
                }
                Array.Copy(m_SamplePoints, linePoints, m_NoSamplePoints);
                noPoints = m_NoSamplePoints;
                return true;
            }
        }         

        /// <summary> This function implements the ILineRenderable interface, 
        /// if there is a raycast hit, it will return the world position and the normal vector
        /// of the hit point, and its position in linePoints. </summary>
        public bool TryGetHitInfo(ref Vector3 position, ref Vector3 normal, ref int positionInLine, ref bool isValidTarget)
        {
            float distance = float.MaxValue;
            int rayIndex = int.MaxValue;

            RaycastHit raycastHit;
            if (GetCurrentRaycastHit(out raycastHit))  // if the raycast hits any collider
            {
                position = raycastHit.point;
                normal = raycastHit.normal;
                positionInLine = rayIndex = m_HitPositionInLine;
                distance = raycastHit.distance;
                // if the collider is registered as an interactable and the interactable is being hovered
                var interactable = interactionManager.TryGetInteractableForCollider(raycastHit.collider);
                
                isValidTarget = interactable && m_HoverTargets.Contains(interactable);
            }

            RaycastResult result;
            int raycastPointIndex;
            if(GetCurrentUIRaycastResult(out result, out raycastPointIndex))
            {
                if(raycastPointIndex >= 0)
                {
                    if (raycastPointIndex < rayIndex || ((raycastPointIndex == rayIndex) && (result.distance <= distance)))
                    {
                        position = result.worldPosition;
                        normal = result.worldNormal;
                        positionInLine = raycastPointIndex;

                        isValidTarget = result.gameObject != null;
                    }
                }                
            }

            return isValidTarget;
        }

        /// <summary>
        /// Updates the current UI Model to match the state of the Interactor
        /// </summary>
        /// <param name="model">The model that will match this Interactor</param>
        public void UpdateUIModel(ref TrackedDeviceModel model)
        {
            model.position = m_StartTransform.position;
            model.orientation = m_StartTransform.rotation;
            model.select = isUISelectActive;

            int numPoints = 0;
            GetLinePoints(ref s_CachedLinePoints, ref numPoints);

            List<Vector3> raycastPoints = model.raycastPoints;
            raycastPoints.Clear();
            if (numPoints > 0 && s_CachedLinePoints != null)
            {
                raycastPoints.Capacity = raycastPoints.Count + numPoints;
                for (int i = 0; i < numPoints; i++)
                    raycastPoints.Add(s_CachedLinePoints[i]);
            }
            model.raycastLayerMask = raycastMask;
        }

        /// <summary>
        /// Attempts to retrieve the current UI Model.  Returns false if not available.
        /// </summary>
        /// <param name="model"> The UI Model that matches that Interactor.</param>
        /// <returns></returns>
        public bool TryGetUIModel(out TrackedDeviceModel model)
        {
            if(m_InputModule != null)
            {
                if (m_InputModule.GetTrackedDeviceModel(this, out model))
                    return true;
            }

            model = new TrackedDeviceModel(-1);
            return false;
        }

        /// <summary>
        /// This function will return the first raycast result, if any raycast results are available.
        /// </summary>
        /// <param name="raycastHit">the raycastHit result that will be filled in by this function</param>
        /// <returns>true if the raycastHit parameter contains a valid raycast result</returns>
        public bool GetCurrentRaycastHit(out RaycastHit raycastHit)
        {
            if (m_HitCount > 0 && m_RaycastHits.Length > 0)
            {
                raycastHit = m_RaycastHits[0];
                return true;
            }
            raycastHit = new RaycastHit();
            return false;
        }

        bool GetCurrentUIRaycastResult(out RaycastResult result, out int raycastPointIndex)
        {
            TrackedDeviceModel model;
            if(TryGetUIModel(out model))
            {
                result = model.implementationData.lastFrameRaycast;
                raycastPointIndex = model.implementationData.lastFrameRaycastResultPositionInLine;
                return result.isValid;
            }

            result = new RaycastResult();
            raycastPointIndex = -1;
            return false;
        }
                       
        void UpdateControlPoints()
        {
            m_ControlPoints[0] = m_StartTransform.position;
            m_ControlPoints[1] = m_ControlPoints[0] + m_StartTransform.forward * m_ControlPointDistance + m_StartTransform.up * m_ControlPointHeight;
            m_ControlPoints[2] = m_ControlPoints[0] + m_StartTransform.forward * m_EndPointDistance + m_StartTransform.up * m_EndPointHeight;
        }

        Vector3 CalculateBezierPoint(float t, Vector3 start, Vector3 control, Vector3 end)
        {
            return Mathf.Pow((1f - t), 2) * start + 2f * (1f - t) * t * control + Mathf.Pow(t, 2) * end;
        }        

        Vector3 CalculateProjectilePoint(float t, Vector3 start, Vector3 velocity, Vector3 acceleration)
        {
            return start + velocity * t + 0.5f * acceleration * t * t;
        }

        void FindReferenceFrame()
        {
            if (m_ReferenceFrame != null)
                return;

            var frame = GetComponentInParent<XRRig>();
            if (frame != null)
            {
                m_ReferenceFrame = frame.transform;
            }
            else
            {
                m_ReferenceFrame = transform;
                Debug.Log("Reference frame of the projectile curve not set and XRRig is not found, using self as default", this);
            }                
        }

        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                // check to see if we have a new hover object
                GetValidTargets(m_ValidTargets);
                var nearestObject = m_ValidTargets.FirstOrDefault();
                if (nearestObject != m_CurrentNearestObject)
                {
                    m_CurrentNearestObject = nearestObject;
                    m_LastTimeHoveredObjectChanged = Time.time;
                    m_PassedHoverTimeToSelect = false;
                }
                else if (nearestObject && !m_PassedHoverTimeToSelect)
                {
                    float progressToHoverSelect = Mathf.Clamp01((Time.time - m_LastTimeHoveredObjectChanged) / m_HoverTimeToSelect);
                    if (progressToHoverSelect >= 1.0f)
                        m_PassedHoverTimeToSelect = true;
                }
            }
        }

        int CheckCollidersBetweenPoints(Vector3 from, Vector3 to)
        {
            Array.Clear(m_RaycastHits, 0, kMaxRaycastHits);

            if (m_HitDetectionType == HitDetectionType.SphereCast && m_SphereCastRadius > 0.0f)
            {
                // casts a sphere along a ray from last point to next point to check if there are hits in between  
                return Physics.SphereCastNonAlloc(from, m_SphereCastRadius, to - from,
                    m_RaycastHits, Vector3.Distance(to, from), raycastMask, raycastTriggerInteraction);
            }
            else
            {
                // raycast from last point to next point to check if there are hits in between                                       
               return Physics.RaycastNonAlloc(from, to - from,
                    m_RaycastHits, Vector3.Distance(to, from), raycastMask, raycastTriggerInteraction);
            }
        }

        /// <summary>
        /// Retrieve the list of interactables that this interactor could possibly interact with this frame.
        /// This list is sorted by priority (in this case distance).
        /// </summary>
        /// <param name="validTargets">Populated List of interactables that are valid for selection or hover.</param>
        public override void GetValidTargets(List<XRBaseInteractable> validTargets)
        {
            // pointers used to sample the curve and check colliders between points
            Vector3 m_PreviousPoint;
            Vector3 m_NextPoint;
            
            validTargets.Clear();

            // we havent init'd cleanly. bail out.
            if(m_SamplePoints == null || m_SamplePoints.Length < 2)
            {
                return;
            }

            m_NoSamplePoints = 1;
            m_SamplePoints[0] = m_StartTransform.position;
                        
            m_PreviousPoint = m_StartTransform.position;
            m_HitCount = 0;
            m_HitPositionInLine = 0;
            int accumulatedHits = 0;

            int maxSamplePoints;
            switch (m_LineType)
            {
                case LineType.StraightLine:
                    m_NextPoint =  m_PreviousPoint + m_StartTransform.forward * maxRaycastDistance;
                    m_HitCount = CheckCollidersBetweenPoints(m_PreviousPoint, m_NextPoint);

                    if (m_HitCount != 0)
                        m_HitPositionInLine = 1; // hit position is between point 0 and point 1

                    // save the "virtual" end point of the line
                    m_SamplePoints[m_NoSamplePoints] = m_NextPoint;
                    m_NoSamplePoints++;
                    break;

                case LineType.ProjectileCurve:
                  
                    float flightTime = 2.0f * m_Velocity * Mathf.Sin(Mathf.Abs(Angle) * Mathf.Deg2Rad) / m_Acceleration + m_AdditionalFlightTime;
                    Vector3 velocityVector = m_StartTransform.forward * m_Velocity;
                    Vector3 accelerationVector = m_ReferenceFrame.up * -1.0f * m_Acceleration;

                    maxSamplePoints = m_SamplePoints.Length;
                    accumulatedHits = 0;
                    for (int i = 1; i < m_SampleFrequency && m_NoSamplePoints < maxSamplePoints; i++)
                    {                    
                        float t = (float)i / (float)(m_SampleFrequency - 1) * flightTime;

                        m_NextPoint = CalculateProjectilePoint(t, m_StartTransform.position, velocityVector, accelerationVector);

                        // check collider only when there has not been a hit point
                        if (accumulatedHits == 0)
                        {
                            accumulatedHits += CheckCollidersBetweenPoints(m_PreviousPoint, m_NextPoint);                            
                            if (accumulatedHits != 0)
                                m_HitPositionInLine = i;
                        }

                        // keep sampling
                        m_SamplePoints[m_NoSamplePoints] = m_NextPoint;
                        m_NoSamplePoints++;
                        m_PreviousPoint = m_NextPoint;
                    }
                    m_HitCount = accumulatedHits;

                    break;

                case LineType.BezierCurve:

                    // update control points for bezier curve
                    UpdateControlPoints();

                    maxSamplePoints = m_SamplePoints.Length;

                    for (int i = 1; i < m_SampleFrequency && m_NoSamplePoints < maxSamplePoints; i++)
                    {
                        float t = (float)i / (float)(m_SampleFrequency - 1);
                        m_NextPoint = CalculateBezierPoint(t, m_ControlPoints[0], m_ControlPoints[1], m_ControlPoints[2]);

                        // check collider only when there has not been a hit point
                        if (accumulatedHits == 0)
                        {
                            accumulatedHits += CheckCollidersBetweenPoints(m_PreviousPoint, m_NextPoint);
                            if (accumulatedHits != 0)
                                m_HitPositionInLine = i;
                        }

                        // keep sampling
                        m_SamplePoints[m_NoSamplePoints] = m_NextPoint;
                        m_NoSamplePoints++;
                        m_PreviousPoint = m_NextPoint;
                    }

                    m_HitCount = accumulatedHits;
                    break;

            }

            // sort all the hits by distance to the controller
            if (m_HitCount > 0)
			{
                SortingHelpers.Sort(m_RaycastHits, m_RaycastHitComparer);
                for (var i = 0; i < Math.Min(m_HitCount, kMaxRaycastHits); ++i)  
			    {
                    var interactable = interactionManager.TryGetInteractableForCollider(m_RaycastHits[i].collider);
			        if (interactable && !validTargets.Contains(interactable))
                        validTargets.Add(interactable);
                    else
                        break; // blocker
			    }
			}                        
		}

        /// <summary>Determines if the interactable is valid for hover this frame.</summary>
        /// <param name="interactable">Interactable to check.</param>
        /// <returns><c>true</c> if the interactable can be hovered over this frame.</returns>
        public override bool CanHover(XRBaseInteractable interactable)
        {
            return base.CanHover(interactable) && (selectTarget == null || selectTarget == interactable);
        }

        /// <summary>Determines if the interactable is valid for selection this frame.</summary>
        /// <param name="interactable">Interactable to check.</param>
        /// <returns><c>true</c> if the interactable can be selected this frame.</returns>
        public override bool CanSelect(XRBaseInteractable interactable)
        {
            bool selectActivated = (hoverToSelect && (m_CurrentNearestObject == interactable) && m_PassedHoverTimeToSelect) || base.CanSelect(interactable);
            return selectActivated && (selectTarget == null || selectTarget == interactable);
        }
    }
}