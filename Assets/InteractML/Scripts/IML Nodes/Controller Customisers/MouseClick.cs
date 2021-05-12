using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;

namespace InteractML.ControllerCustomisers
{
    /// <summary>
    /// Mouse Click
    /// </summary>
    [NodeWidth(250)]
    public class MouseClick : IMLNode
    {

        /// <summary>
        /// Node data sent outside of this node onwards
        /// </summary>
        [Output]
        public bool ControllerOutput;


    }
}



