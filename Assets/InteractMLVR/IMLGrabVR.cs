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
            imlgrab = GetComponent<IMLGrab>();
#if MECM
            onSelectEnter.AddListener(Selected); // Using an older version of XR Interaction Toolkit
#else
            onSelectEntered.AddListener(Selected);
#endif
        }

        private void Selected(XRBaseInteractor interactable)
        {
            //imlgrab.Selected();
            //imlgrab.ActivateInterface();
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

