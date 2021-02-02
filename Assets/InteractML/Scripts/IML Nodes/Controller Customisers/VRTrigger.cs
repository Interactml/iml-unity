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

        private VRButtonHandler triggerButton;
        public IMLSides hand;


        private bool previousPress = false;


        public override void Initialize()
        {
             
        }



    }
}


