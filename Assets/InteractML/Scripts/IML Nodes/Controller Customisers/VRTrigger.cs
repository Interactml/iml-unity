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
    public class VRTrigger : IMLNode
    {
        //public ButtonHandler primaryAxis
        /// <summary>
        /// Node data sent outside of this node onwards
        /// </summary>
        [Output]
        public bool ControllerOutput;

        public InputHelpers.Button button = InputHelpers.Button.None;

        private bool previousPress = false;

        private string attachedID;

        private List<InputDevice> inputDevices = new List<InputDevice>();


        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);
            IMLNode node = (IMLNode)from.node;
            attachedID = node.id;
        }

        public void HandleState(XRController controller)
        {
            // Debug.Log(button);
            //Debug.Log(controller.axisToPressThreshold);
            if (controller.inputDevice.IsPressed(button, out bool pressed, controller.axisToPressThreshold))
            {
                if (previousPress != pressed)
                {
                    previousPress = pressed;
                    if (pressed)
                    {
                        Debug.Log("here");
                        //OnButtonDown?.Invoke(controller);
                    }
                    else
                    {
                        //OnButtonUp?.Invoke(controller);
                    }
                }
            }
        }


    }
}


