using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace InteractML.ControllerCustomisers
{
    /// <summary>
    /// VR Trigger
    /// </summary>
    [NodeWidth(250)]
    [CreateNodeMenuAttribute("Interact ML/Hardware Input/VR Trigger")]
    public class VRTrigger : CustomController
    {

        public VRButtonHandler button;
        public IMLSides hand;
        public IMLControllerInputs inputs;
        public IMLTriggerTypes triggerType;
       
        private bool previousPress = false;

        public override void Initialize()
        {
            //Debug.Log("initialize");
            name = "VRTrigger";
            buttonOptions = InputHelperMethods.deviceEnumSetUp(IMLInputDevices.VRControllers);
            int input = (int)inputs;
            button = new VRButtonHandler(input, hand, triggerType, "VRTrigger");
            inputValue = false;
            SubscribeDelegates();
        }

        public override void UpdateLogic()
        {
            //Debug.Log(inputChange);
            if (inputChange)
                inputChange = false;
            if (button == null)
            {
                int input = (int)inputs;
                button = new VRButtonHandler(input, hand, triggerType, "VRTrigger");
            }
                
            button.HandleState();
        }

        public override void OnButtonChange()
        {
            button.SetButtonNo(inputNo);
        }

        public override void OnTriggerChange()
        {
            button.SetTriggerType(trigger);
        }

        public override string GetButtonName()
        {
            string buttonName = button.button.name;
            string controllerName = System.Enum.GetName(typeof(IMLSides), hand);
            buttonName = string.Concat(controllerName, buttonName);
            return buttonName;
        }

        public void OnSideChange()
        {
            button.SetController(hand);
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
            button.ButtonFire += StateChange;
        }
        private void UnsubscribeDelegates()
        {
            button.ButtonFire -= StateChange;
        }

        private bool StateChange(string nodeID)
        {
            inputChange = true;
            return inputChange;
        }

        /*  public override void UpdateLogic()
          {
              Debug.Log("here");
              button.HandleState();
          }
          public override void OnButtonChange()
          {
              //button.imlButton = inputs; 
              button.SetButton();
          }

          public override void OnTriggerChange()
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
          }*/
    }
}


