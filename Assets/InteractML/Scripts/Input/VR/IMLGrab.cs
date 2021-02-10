using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace InteractML
{
    public class IMLGrab : XRBaseInteractable
    {
        public IMLComponent graph;
        public Color selectedHighlight;
        public Texture2D trainedColor;
        
        public Texture2D baseColour;
        public Texture2D recordingColour;
        public Texture2D runningColour;
        [HideInInspector]
        public Texture2D current;

        public void Start()
        {
            IMLEventDispatcher.deselectGraph += DeactivateVRInterface;
            Deselected();
            SetBody(baseColour);
        }

        public void Update()
        {
            if (interactionManager == null)
            {
                interactionManager = FindObjectOfType<XRInteractionManager>();
            }
        }
        public void Selected()
        {
            Debug.Log("selected");
            this.GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 0.01f);
            //cubeRenderer.material.SetColor("_OutlineColor", selectedHighlight);
            Debug.Log("selected");
            IMLEventDispatcher.selectGraph?.Invoke(graph);

        }


        public void SetColourHighlight(Color colour)
        {
            this.GetComponent<Renderer>().sharedMaterial.SetColor("_OutlineColor", colour);
        }
        
        public void SetBody(Texture2D texture)
        {
            this.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
            current = texture;
        }
        
        public void Deselected()
        {
            this.GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 0.00f);

        }
       

        public void Recording()
        {

        }

        public void ActivateVRInterface()
        {
            graph.universalInputActive = true;
        }
        
        public void DeactivateVRInterface(IMLComponent deselectedGraph)
        {
            if (graph = deselectedGraph)
            {
                Deselected();
                graph.universalInputActive = false;
            }
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

