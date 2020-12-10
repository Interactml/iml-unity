using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;

namespace InteractML.MovementFeatures
{
    /// <summary>
    /// Feature extractor for positions
    /// </summary>
    [NodeWidth(250)]
    public class PositionNode : BaseMovementFeatureNode, IFeatureIML
    {
        /// <summary>
        /// GameObject from which we extract a feature
        /// </summary>
        [Input]
        public GameObject GameObjectDataIn;

        /// <summary>
        /// Node data sent outside of this node onwards
        /// </summary>
        [Output]
        public Node LiveDataOut;
        
        /// <summary>
        /// Controls whether to use local space or not
        /// </summary>
        public bool LocalSpace = false;

        [HideInInspector]
        public bool GameObjInputMissing;

        /// <summary>
        /// Feature Values extracted (ready to be read by a different node)
        /// </summary>
        [HideInInspector]
        public override IMLBaseDataType FeatureValues { get { return m_PositionExtracted; } }

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
        public override void Initialize()
        {
            // Make sure feature extractor value is never null
            if (m_PositionExtracted == null)
                m_PositionExtracted = new IMLVector3();

            // initialise helper variables
            PreviousFeatureValues = new IMLVector3();

        }


        /// <summary>
        /// Return the correct value of an output port when requested
        /// </summary>
        /// <returns></returns>
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
            // update if node is receiving data
            ReceivingData = MovementFeatureMethods.IsReceivingData(this);

            // gameobject input
            var gameObjRef = GetInputValue<GameObject>("GameObjectDataIn", this.GameObjectDataIn);

            // check if there's a gameobject connected
            if (gameObjRef == null)
            {
                if ((graph as IMLGraph).IsGraphRunning)
                {
                    // If the gameobject is null, we throw an error on the editor console
                    //Debug.LogWarning("GameObject missing in Extract Rotation Node!");
                }
                GameObjInputMissing = true;
            }
            else
            {
                // Set values of our feature extracted
                if (LocalSpace)
                    m_PositionExtracted.SetValues(gameObjRef.transform.localPosition);
                else 
                    m_PositionExtracted.SetValues(gameObjRef.transform.position);
            } 

            // check if each toggle is off and set feature value to 0, return float array of updated feature values
            m_PositionExtracted.Values = MovementFeatureMethods.CheckTogglesAndUpdateFeatures(this, m_PositionExtracted.Values);
          
            return this;

        }

        // Check that we are only connecting to a GameObject
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            // control what connections the input port accepts (not output port)
            if (to.node == this)
            {
                // only allow 1 connection (but don't override the original - user must disconnect original input to connect a different one)
                if (this.GetInputNodesConnected("GameObjectDataIn").Count > 1) { from.Disconnect(to); }

                // only accept gameobject node, disconnect otherwise
                this.DisconnectIfNotType<BaseMovementFeatureNode, GameObjectNode>(from, to);
            }
        }
    }
}

