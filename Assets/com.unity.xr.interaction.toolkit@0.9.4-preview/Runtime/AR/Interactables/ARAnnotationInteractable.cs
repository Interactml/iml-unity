#if !AR_FOUNDATION_PRESENT

// Stub class definition used to fool version defines that this MonoScript exists (fixed in 19.3)
namespace UnityEngine.XR.Interaction.Toolkit.AR {  public class ARAnnotationInteractable {} }

#else

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    [Serializable]
    public class ARAnnotation
    {
        [SerializeField]
        [Tooltip("The visualization game object that will become active when the object is hovered over.")]      
        GameObject m_AnnotationVisualization;
        /// <summary>
        /// The visualization game object that will become active when the object is hovered over.
        /// </summary>
        public GameObject annotationVisualization { get { return m_AnnotationVisualization; } set { m_AnnotationVisualization = value; } }


        [SerializeField]
        [Tooltip("Maximum angle (in radians) off of FOV horizontal center to show annotation.")]   
        float m_MaxFOVCenterOffsetAngle = 0.25f;
        /// <summary>
        /// Maximum angle (in radians) off of FOV horizontal center to show annotation. 
        /// </summary>
        public float maxFOVCenterOffsetAngle { get { return m_MaxFOVCenterOffsetAngle; } set { m_MaxFOVCenterOffsetAngle = value; } }

        [SerializeField]
        [Tooltip("Minimum range to show annotation at.")]       
        float m_MinAnnotationRange = 0.0f;
        /// <summary>
        /// Minimum range to show annotation at.
        /// </summary>
        public float minAnnotationRange { get { return m_MinAnnotationRange; } set { m_MinAnnotationRange = value; } }

        [SerializeField]
        [Tooltip("Maximum range to show annotation at.")]  
        float m_MaxAnnotationRange = 10.0f;
        /// <summary>
        /// Maximum range to show annotation at.
        /// </summary>
        public float maxAnnotationRange { get { return m_MaxAnnotationRange; } set { m_MaxAnnotationRange = value; } }
    }
    
    
    public class ARAnnotationInteractable : ARBaseGestureInteractable
    {
        [SerializeField]
        List<ARAnnotation> m_Annotations = new List<ARAnnotation>();
        
        ARAnnotation m_ViewingAnnotation;
        ARAnnotation m_ViewingAnnotationDotProduct;

        void Update()
        {
            // Disable all annotations if not hovered.
            if (!isHovered)
            {
                foreach (var annotation in m_Annotations)
                {
                    annotation.annotationVisualization.SetActive(false);
                }
            }
            else
            {
                var fromCamera = transform.position - Camera.main.transform.position;
                float distSquare = fromCamera.sqrMagnitude;
                fromCamera.y = 0.0f;
                fromCamera.Normalize();
                float dotProd = Vector3.Dot(fromCamera, Camera.main.transform.forward);

                foreach (var annotation in m_Annotations)
                {
                    bool enableThisFrame = 
                        (Mathf.Acos(dotProd) < annotation.maxFOVCenterOffsetAngle &&
                        distSquare >= Mathf.Pow(annotation.minAnnotationRange, 2.0f) && 
                        distSquare < Mathf.Pow(annotation.maxAnnotationRange, 2.0f));
                    if (annotation.annotationVisualization != null)
                    {
                        if (enableThisFrame && !annotation.annotationVisualization.activeSelf)
                            annotation.annotationVisualization.SetActive(true);
                        else if (!enableThisFrame && annotation.annotationVisualization.activeSelf)
                            annotation.annotationVisualization.SetActive(false);

                        // If enabled, align to camera
                        if (annotation.annotationVisualization.activeSelf)
                        {
                            annotation.annotationVisualization.transform.rotation =
                                Quaternion.LookRotation(fromCamera, transform.up);
                        }
                    }
                }
            }
        }
    }
}

#endif