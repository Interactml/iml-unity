using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace InteractML
{
    public class IMLGrabVR : XRBaseInteractable
    {
        IMLGrab imlgrab;

        public void Start()
        {
            imlgrab.GetComponent<IMLGrab>();
            onSelectEntered.AddListener(Selected);
        }

        private void Selected(XRBaseInteractor interactable)
        {
            imlgrab.Selected();
            imlgrab.ActivateInterface();
        }

        public void Update()
        {
            if (interactionManager == null)
            {
                interactionManager = FindObjectOfType<XRInteractionManager>();
            }
        }
        

    }
}

