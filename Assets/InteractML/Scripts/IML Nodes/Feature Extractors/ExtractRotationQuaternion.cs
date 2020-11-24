﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;

namespace InteractML.FeatureExtractors
{
    /// <summary>
    /// Feature extractor for rotations
    /// </summary>
    [NodeWidth(250)]
    public class ExtractRotationQuaternion : BaseExtractorNode, IFeatureIML
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

        /// <summary>
        /// Feature Values extracted (ready to be read by a different node)
        /// </summary>
        public override IMLBaseDataType FeatureValues { get { return m_RotationQuaternionExtracted; } }

        /// <summary>
        /// The private feature values extracted in a more specific data type
        /// </summary>
        [SerializeField]
        private IMLVector4 m_RotationQuaternionExtracted;

        [HideInInspector]
        public bool GameObjInputMissing;

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
            // initialise local variables
            if (m_RotationQuaternionExtracted == null)
                m_RotationQuaternionExtracted = new IMLVector4();

            // initialise helper variables
            PreviousFeatureValues = new IMLVector4();

            // load node specific tooltips
            tooltips = IMLTooltipsSerialization.LoadTooltip("Rotation Quaternion");

            base.Init();
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
            // update if node is receiving data
            ReceivingData = FeatureExtractorMethods.IsReceivingData(this);

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
                if(LocalSpace)
                    m_RotationQuaternionExtracted.SetValues(gameObjRef.transform.localRotation);
                else
                    m_RotationQuaternionExtracted.SetValues(gameObjRef.transform.rotation);

                GameObjInputMissing = false;
            }

            // check if each toggle is off and set feature value to 0, return float array of updated feature values
            m_RotationQuaternionExtracted.Values = FeatureExtractorMethods.CheckTogglesAndUpdateFeatures(this, m_RotationQuaternionExtracted.Values);
            return this;

        }

        // Check that we are only connecting to a GameObject
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            if (from.node.GetType() == this.GetType())
            {
                System.Type[] portTypesAccept = new System.Type[] { };
                System.Type[] nodeTypesAccept = new System.Type[] { typeof(IFeatureIML), typeof(MLSystem) };
                this.DisconnectPortAndNodeIfNONETypes(from, to, portTypesAccept, nodeTypesAccept);
            }
            else
            {
                this.DisconnectIfNotType<BaseExtractorNode, GameObjectNode>(from, to);

            }
        }
    }
}
