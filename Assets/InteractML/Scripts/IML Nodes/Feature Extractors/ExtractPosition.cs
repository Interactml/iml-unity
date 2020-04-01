using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;

namespace InteractML.FeatureExtractors
{
    /// <summary>
    /// Feature extractor for positions
    /// </summary>
    [NodeTint("#3A3B5B")]
    public class ExtractPosition : Node, IFeatureIML
    {
        /// <summary>
        /// GameObject from which we extract a feature
        /// </summary>
        [Input]
        public GameObject gameObjectIntoNode;

        /// <summary>
        /// Node data sent outside of this node onwards
        /// </summary>
        [Output]
        public Node positionExtracted;

        /// <summary>
        /// Controls whether to use local space or not
        /// </summary>
        public bool LocalSpace;

        /// <summary>
        /// Feature Values extracted (ready to be read by a different node)
        /// </summary>
        public IMLBaseDataType FeatureValues { get { return m_PositionExtracted; } }

        /// <summary>
        /// The private feature values extracted in a more specific data type
        /// </summary>
        [SerializeField]
        private IMLVector3 m_PositionExtracted;

        /// <summary>
        /// Lets external classes known if they should call UpdateFeature
        /// </summary>
        public bool isExternallyUpdatable { get { return isConnectedToSomething; } }

        /// <summary>
        /// Private logic to know when this node should be updatable
        /// </summary>
        private bool isConnectedToSomething { get { return (Outputs.Count() > 0); } }

        /// <summary>
        /// Was the feature already updated?
        /// </summary>
        public bool isUpdated { get; set; }

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

            if (m_PositionExtracted == null)
            {
                m_PositionExtracted = new IMLVector3();

            }

            name = "LIVE POSITION DATA";


        }
        public void OnDestroy()
        {
            // Remove reference of this node in the IMLComponent controlling this node (if any)
            var MLController = graph as IMLController;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.DeleteFeatureNode(this);
            }
        }
        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return UpdateFeature();
        }

        /// <summary>
        /// Updates Feature values
        /// </summary>
        /// <returns></returns>
        public object UpdateFeature()
        {
            if (m_PositionExtracted == null)
            {
                m_PositionExtracted = new IMLVector3();

            }


            var gameObjRef = GetInputValue<GameObject>("gameObjectIntoNode", this.gameObjectIntoNode);

            if (gameObjRef == null)
            {
                // If the gameobject is null, we throw an error on the editor console
                Debug.LogWarning("GameObject missing in Extract Position Node!");
            }
            else
            {
                
                // Set values of our feature extracted
                if (LocalSpace)
                    m_PositionExtracted.SetValues(gameObjRef.transform.localPosition);
                else
                    m_PositionExtracted.SetValues(gameObjRef.transform.position);

            }

            return this;

        }
    }
}

