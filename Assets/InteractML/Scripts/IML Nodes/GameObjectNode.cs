using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML
{
    public class GameObjectNode : Node
    {
        /// <summary>
        /// The GameObject from the scene to use
        /// </summary>
        [Output]
        public GameObject GameObjectFromScene;

        [HideInInspector]
        public bool GameObjMissing;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            if (GameObjectFromScene != null)
            {
                GameObjMissing = false;
                return GameObjectFromScene;
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