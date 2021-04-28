using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML
{
    [CreateNodeMenuAttribute("Interact ML/Game Object")]
    [NodeWidth(250)]
    public class GameObjectNode : IMLNode
    {
        #region Variables

        /// <summary>
        /// The GameObject from the scene to use
        /// </summary>
        [Output] public GameObject GameObjectDataOut;

        /// <summary>
        /// Has the node been updated?
        /// </summary>
        [HideInInspector]
        public bool state;

        /// <summary>
        /// Marks if this GOnode is already assigned to a GO
        /// </summary>
        public bool IsTaken { get { return (GameObjectDataOut != null); } }

        /// <summary>
        /// Hash value from the GO. Useful to identify to which GO instance this node belongs to
        /// </summary>
        [SerializeField, HideInInspector]
        public int GOHashCode;


#if UNITY_EDITOR
        /// <summary>
        /// Flag that marks if this node was created during playMode (useful when deleting things after leaving playmode)
        /// </summary>
        [HideInInspector]
        public bool CreatedDuringPlaymode;
#endif

        #endregion

        #region XNode Messages

        /// <summary>
        /// Remove reference of this node in the IMLComponent controlling this node (if any)
        /// </summary> 
        public void OnDestroy()
        { 
            var MLController = graph as IMLGraph;
            if (MLController != null)
                MLController.SceneComponent.DeleteGameObjectNode(this);
        }

        /// <summary>
        /// Return the correct value of an output port when requested
        /// </summary>
        /// <param name="port"></param>
        public override object GetValue(NodePort port)
        {
            if (GameObjectDataOut != null)
                return GameObjectDataOut;
            else
                return null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets GameObject and update internal references
        /// </summary>
        /// <param name="gameObject"></param>
        public void SetGameObject(GameObject gameObject)
        {
            // If the GameObject is null but we have some memory of the previous GameObject.
            if (GameObjectDataOut == null && GOHashCode != default(int))
            {
                // It is the same GO! Assign it but don't clear ports
                // do nothing
            }

            // Set the GO 
            GameObjectDataOut = gameObject;
            // Update node name
            name = gameObject.name + " (GameObject)";
            // Update hash reference of GO held
            GOHashCode = gameObject.GetHashCode();

        }

        #endregion
    }
}