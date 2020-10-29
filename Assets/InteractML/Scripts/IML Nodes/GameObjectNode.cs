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
        

        [HideInInspector]
        public bool GameObjMissing;
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

        public void OnDestroy()
        {
            // Remove reference of this node in the IMLComponent controlling this node (if any)
            var MLController = graph as IMLGraph;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.DeleteGameObjectNode(this);
            }
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            if (GameObjectDataOut != null)
            {
                GameObjMissing = false;
                return GameObjectDataOut;
            }
            else
            {
                if ((graph as IMLGraph).IsGraphRunning)
                {
                    Debug.LogWarning("GameObject missing from GameObjectNode!!");
                }
                GameObjMissing = true;
                return null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets GO and update internal references
        /// </summary>
        /// <param name="gameObject"></param>
        public void SetGameObject(GameObject gameObject)
        {
            // If the GO is null but we have some memory of the previous GO...
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