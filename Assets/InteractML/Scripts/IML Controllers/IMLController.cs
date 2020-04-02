using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML
{
    /// <summary> 
    /// Defines an example nodegraph IML Controller that can be created as an asset in the Project window. 
    /// </summary>
    [CreateAssetMenu(fileName = "New IML Controller", menuName = "InteractML/IML Controller")]
    public class IMLController : NodeGraph
    {
        /// <summary>
        /// The IML Component in the scene that this graph is attached to
        /// </summary>
        public IMLComponent SceneComponent;

        /// <summary>
        /// Flag that tells us if the graph is supposed to currently run
        /// </summary>
        public bool IsGraphRunning { get { return (SceneComponent != null); } }


    }
}

