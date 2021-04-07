﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using InteractML.CustomControllers;

namespace InteractML.ControllerCustomisers
{
    /// <summary>
    /// VR Trigger
    /// </summary>
    [NodeWidth(250)]
    public class VRTrigger : CustomController
    {

        public VRButtonHandler button;
        public IMLSides hand;
        public IMLControllerInputs inputs;
        public IMLTriggerTypes triggerType;
       
        private bool previousPress = false;

        public override void Initialize()
        {
            int input = (int)inputs;
            button = new VRButtonHandler(input, hand, triggerType, "VRTrigger");
        }
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            IMLNode node = to.node as IMLNode;
            button.nodeID = node.id;

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            m_inputEvent = true;

            return this;
        }
        public override void UpdateLogic()
        {
            button.HandleState();
        }
        public void OnButtonChange()
        {
            //button.imlButton = inputs; 
            button.SetButton();
        }
        
        public void OnTriggerChange()
        {
            button.SetTriggerType(triggerType);
        }
        
        public void OnHandChange()
        {
            button.SetController(hand);
        }
        private bool SetBool()
        {
            Debug.Log("fire");
            return true;
        }

        public void OnDestroy()
        {
            Debug.Log("here");
            var MLController = graph as IMLGraph;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.DeleteCustomControllerNode(this);
            }
        }
    }
}


