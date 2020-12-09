using System.Collections;
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
    public class KeyboardPress : IMLNode
    {
        
        /// <summary>
        /// Node data sent outside of this node onwards
        /// </summary>
        [Output]
        public bool ControllerOutput;

   
    }
}


