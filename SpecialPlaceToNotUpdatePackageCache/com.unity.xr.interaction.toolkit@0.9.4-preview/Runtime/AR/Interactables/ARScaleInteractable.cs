//-----------------------------------------------------------------------
// <copyright file="ScaleManipulator.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

#if !AR_FOUNDATION_PRESENT

// Stub class definition used to fool version defines that this MonoScript exists (fixed in 19.3)
namespace UnityEngine.XR.Interaction.Toolkit.AR {  public class ARScaleInteractable {} }

#else

using UnityEngine;

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Controls the scale of an object via a Pinch gesture.
    /// If an object is selected, then doing a pinch/zoom modify the scale
    /// of the object.
    /// </summary>
    public class ARScaleInteractable : ARBaseGestureInteractable
    {

        [SerializeField, Tooltip("The minimum scale of the object.")]
        float m_MinScale = 0.75f;
        /// <summary>
        /// The minimum scale of the object.
        /// </summary>
        public float minScale {  get { return m_MinScale; } set { m_MinScale = value; } }

        [SerializeField, Tooltip("The maximum scale of the object.")]
        float m_MaxScale = 1.75f;
        /// <summary>
        /// The maximum scale of the object.
        /// </summary>
        public float maxScale {  get { return m_MaxScale; } set { m_MaxScale = value; } }

        [SerializeField, Tooltip("The elastic ratio used when scaling the object")]
        float m_ElasticRatioLimit = 0.0f;
        /// <summary>
        /// The limit of the elastic ratio.
        /// </summary>
        public float elasticRatioLimit { get { return m_ElasticRatioLimit; } set { m_ElasticRatioLimit = value; } }

        [SerializeField, Tooltip("Sensitivity to movement being translated into scale.")]
        float m_Sensitivity = 0.75f;
        /// <summary>
        /// Sensitivity to movement being translated into scale.
        /// </summary>
        public float sensitivity { get { return m_Sensitivity; } set { m_Sensitivity = value; } }

        [SerializeField, Tooltip("Amount that the scale bounces back after hitting min/max of range.")]
        float m_Elasticity = 0.15f;
        /// <summary>
        /// Amount that the scale bounces back after hitting min/max of range.
        /// </summary>
        public float elasticity { get { return m_Elasticity; } set { m_Elasticity = value; } }

        float m_CurrentScaleRatio;
        bool m_IsScaling;

        float m_ScaleDelta
        {
            get
            {
                if (minScale > maxScale)
                {
                    Debug.LogError("minScale must be smaller than maxScale.");
                    return 0.0f;
                }

                return maxScale - minScale;
            }
        }

        float m_ClampedScaleRatio
        {
            get
            {
                return Mathf.Clamp01(m_CurrentScaleRatio);
            }
        }

        float m_CurrentScale
        {
            get
            {
                float elasticScaleRatio = m_ClampedScaleRatio + ElasticDelta();
                float elasticScale = minScale + (elasticScaleRatio * m_ScaleDelta);
                return elasticScale;
            }
        }

        /// <summary>
        /// Enabled the scale controller.
        /// </summary>
        protected void OnEnable()
        {           
            m_CurrentScaleRatio = (transform.localScale.x - minScale) / m_ScaleDelta;
        }

        void OnValidate() 
        {
            minScale = Mathf.Max(0.0f, minScale);
            maxScale = Mathf.Max(Mathf.Max(0.0f, minScale), maxScale);
        }

        /// <summary>
        /// Returns true if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>True if the manipulation can be started.</returns>
        protected override bool CanStartManipulationForGesture(PinchGesture gesture)
        {
            if (!IsGameObjectSelected())
            {
                return false;
            }

            if (gesture.TargetObject != null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Recalculates the current scale ratio in case local scale, min or max scale were changed.
        /// </summary>
        /// <param name="gesture">The gesture that started this transformation.</param>
        protected override void OnStartManipulation(PinchGesture gesture)
        {
            m_IsScaling = true;
            m_CurrentScaleRatio = (transform.localScale.x - minScale) / m_ScaleDelta;
        }

        /// <summary>
        /// Continues the scaling of the object.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        protected override void OnContinueManipulation(PinchGesture gesture)
        {
            m_CurrentScaleRatio += sensitivity * GestureTouchesUtility.PixelsToInches(gesture.GapDelta);

            float currentScale = m_CurrentScale;
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            // If we've tried to scale too far beyond the limit, then cancel the gesture
            // to snap back within the scale range.
            if (m_CurrentScaleRatio < -elasticRatioLimit
                || m_CurrentScaleRatio > (1.0f + elasticRatioLimit))
            {
                gesture.Cancel();
            }
        }

        /// <summary>
        /// Finishes the scaling of the object.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        protected override void OnEndManipulation(PinchGesture gesture)
        {
            m_IsScaling = false;
        }

        float ElasticDelta()
        {
            float overRatio = 0.0f;
            if (m_CurrentScaleRatio > 1.0f)
            {
                overRatio = m_CurrentScaleRatio - 1.0f;
            }
            else if (m_CurrentScaleRatio < 0.0f)
            {
                overRatio = m_CurrentScaleRatio;
            }
            else
            {
                return 0.0f;
            }

            return (1.0f - (1.0f / ((Mathf.Abs(overRatio) * elasticity) + 1.0f)))
            * Mathf.Sign(overRatio);
        }

        void LateUpdate()
        {
            if (!m_IsScaling)
            {
                m_CurrentScaleRatio =
                    Mathf.Lerp(m_CurrentScaleRatio, m_ClampedScaleRatio, Time.deltaTime * 8.0f);
                float currentScale = m_CurrentScale;
                transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            }
        }
    }
}

#endif
