using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace InteractML
{
    public class IMLGrab : XRBaseInteractable
    {
        // Interactor
        XRBaseInteractor m_SelectingInteractor;
        /// <summary>Get the current selecting interactor for this interactable.
        public XRBaseInteractor selectingInteractor { get { return m_SelectingInteractor; } }
        public IMLComponent graph;
        
        public Color selectedHighlight;
        public Color trainedHighlight;
        
        public Texture2D baseColour;
        public Texture2D recordingColour;
        public Texture2D runningColour;
        [HideInInspector]
        public Texture2D current;

        public void Selected()
        {
            this.GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 0.01f);
            //cubeRenderer.material.SetColor("_OutlineColor", selectedHighlight);

        }

        public void SetColourHighlight(Color colour)
        {
            this.GetComponent<Renderer>().material.SetColor("_OutlineColor", colour);
        }
        
        public void SetBody(Texture2D texture)
        {
            this.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
            current = texture;
        }
       

        public void Recording()
        {

        }

        public void ActivateVRInterface()
        {
            graph.universalInputActive = true;
        }
        
        public void DeactivateVRInterface()
        {
            graph.universalInputActive = false;
        }
        void Detach()
        {
            /*if (m_ThrowOnDetach)
            {
                m_RigidBody.velocity = m_DetachVelocity;
                m_RigidBody.angularVelocity = m_DetachAngularVelocity;
            }*/
        }
    }
}

