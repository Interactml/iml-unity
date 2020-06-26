using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML
{
    [CreateNodeMenuAttribute("Interact ML/Game Object")]
    [NodeWidth(250)]
    public class GameObjectNode : Node
    {
        /// <summary>
        /// The GameObject from the scene to use
        /// </summary>
        [Output] public GameObject GameObjectDataOut;
        

        [HideInInspector]
        public bool GameObjMissing;
        [HideInInspector]
        public bool state;

        [HideInInspector]
        public IMLNodeTooltips tips;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

            var MLController = graph as IMLController;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.GetAllNodes();
               
            }

            tips = IMLTooltipsSerialization.LoadTooltip("GameObject");
        }

        public void OnDestroy()
        {
            // Remove reference of this node in the IMLComponent controlling this node (if any)
            var MLController = graph as IMLController;
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
                if ((graph as IMLController).IsGraphRunning)
                {
                    Debug.LogWarning("GameObject missing from GameObjectNode!!");
                }
                GameObjMissing = true;
                return null;
            }
        }

        
    }
}