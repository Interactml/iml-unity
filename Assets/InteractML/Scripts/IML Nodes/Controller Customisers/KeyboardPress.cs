﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;

namespace InteractML.ControllerCustomisers
{
    /// <summary>
    /// Keyboard press 
    /// </summary>
    [NodeWidth(250)]
    public class KeyboardPress : CustomController
    {

        KeyboardHandler handler;

        string attachedNodeID;

        public override void Initialize()
        {
            name = "Keyboard Input";
            buttonOptions = InputHelperMethods.deviceEnumSetUp(IMLInputDevices.Keyboard);
            handler = new KeyboardHandler(inputNo, trigger, name);
            inputValue = false;
            SubscribeDelegates();
            
        }

        public override void UpdateLogic()
        {
            if (inputChange)
                inputChange = false;
            handler.HandleState();
        }

        public override void OnButtonChange()
        {
            handler.SetButtonNo(inputNo);
        }

        public override void OnTriggerChange()
        {
            handler.SetTriggerType(trigger);
        }

        public void OnDestroy()
        {
            // Unscibscribe from all events
            UnsubscribeDelegates();

            // Remove reference of this node in the IMLComponent controlling this node (if any)
            var MLController = graph as IMLGraph;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.DeleteCustomControllerNode(this);
            }
        }

        private void SubscribeDelegates()
        {
            handler.ButtonFire += StateChange;
        }
        private void UnsubscribeDelegates()
        {
            handler.ButtonFire -= StateChange;
        }

        private bool StateChange(string nodeID)
        {
            Debug.Log("here");
            inputChange = true;
            return inputChange;
        }
    }
}


