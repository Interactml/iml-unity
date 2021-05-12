﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractML
{
    public class IMLGrab : MonoBehaviour
    {
        // reference to IML graph this object represents
        public IMLComponent graph;
        // colour for outline when selected
        public Color selectedHighlight;
        // texture for circle when the model is trained
        public Texture2D trainedColor;
        // base texture when the object is not selected
        public Texture2D baseColour;
        // texture for when graph recording
        public Texture2D recordingColour;
        // texture for when the graph is running
        public Texture2D runningColour;
        [HideInInspector]
        // hold current texture - used bu imlcomponent
        public Texture2D current;
        private Texture2D currentOld;

        public void OnValidate()
        {
            //Debug.Log("validate grab");

            IMLEventDispatcher.UniversalControlChange += DestroyMe;
            IMLEventDispatcher.DestroyIMLGrab += DestroyIt;
        }
        public void Start()
        {

            IMLEventDispatcher.deselectGraph += DeactivateInterface;
            Deselected();
            SetBody(baseColour);
        }
        public void Update()
        {
            if (currentOld != current)
            {
                this.GetComponent<Renderer>().material.SetTexture("_MainTex", current);
                Debug.Log("here");
                currentOld = current;
            }
                
        }

        private void DestroyMe(bool destroy)
        {
            if (!destroy)
            {
                Debug.Log("destroy");
                //DestroyImmediate(this);
            }
            
        }

        private void DestroyIt()
        {
            Debug.Log("destroy");
            DestroyImmediate(this.gameObject);

        }
        /// <summary>
        /// method for when the object is selected change texture and send event for activation
        /// </summary>
        public void Selected()
        {
            this.GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 0.01f);
            IMLEventDispatcher.selectGraph?.Invoke(graph);

        }

        /// <summary>
        /// method for when the colour is highlighted 
        /// </summary>
        /// <param name="colour"></param>
        public void SetColourHighlight(Color colour)
        {
            this.GetComponent<Renderer>().sharedMaterial.SetColor("_OutlineColor", colour);
        }
        
        public void SetBody(Texture2D texture)
        {
            Debug.Log("set body" + this);
            if (this != null && Application.isPlaying)
            {
               // Debug.Log("change colour");
                this.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
            }
            current = texture;
        }
        
        public void Deselected()
        {
            this.GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 0.00f);

        }
       


        public void ActivateInterface()
        {
            graph.universalInputActive = true;
        }
        
        public void DeactivateInterface(IMLComponent deselectedGraph)
        {
            if (graph = deselectedGraph)
            {
                Deselected();
                graph.universalInputActive = false;
            }
        }

        private void OnDestroy()
        {
            Debug.Log("here");
            IMLEventDispatcher.deselectGraph -= DeactivateInterface;
            IMLEventDispatcher.UniversalControlChange -= DestroyMe;
            IMLEventDispatcher.DestroyIMLGrab -= DestroyIt;
        }

    }
}

