using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;

namespace InteractML.ControllerCustomisers
{
    /// <summary>
    /// VR Trigger
    /// </summary>
    [NodeWidth(250)]
    public class VRTrigger : IMLNode
    {

        /// <summary>
        /// Node data sent outside of this node onwards
        /// </summary>
        [Output]
        public bool ControllerOutput;


    }
}


