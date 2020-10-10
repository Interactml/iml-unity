using InteractML.DataTypeNodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML.FeatureExtractors
{
    /// <summary>
    /// Base Class for Feature Extractor Nodes
    /// </summary>
    public abstract class BaseExtractorNode : IMLNode, IFeatureIML
    {

        #region Variables

        /// <summary>
        /// IMLFeature variables
        /// </summary>
        public abstract IMLBaseDataType FeatureValues { get; }

        /// <summary>
        /// Lets external classes know if they should call UpdateFeature (always true for a data type)
        /// </summary>
        bool IFeatureIML.isExternallyUpdatable { get { return true; } }

        /// <summary>
        /// Was the feature already updated?
        /// </summary>
        bool IFeatureIML.isUpdated { get; set; }

        /// <summary>
        /// Is the node currently receiving input data
        /// </summary>
        public bool ReceivingData;

        /// <summary>
        /// Variables to count the number of frames the input values stay the same 
        /// </summary>
        public int Counter, Count;

        /// <summary>
        /// Array of booleans, one per each feature value to allow user to toggle each value on/off
        /// </summary>
        public bool[] ToggleSwitches;

        /// <summary>
        // Variable storing previous frame feature values to test incoming data is changing
        /// </summary>
        public IMLBaseDataType PreviousFeatureValues;

        #endregion

        #region XNode Messages

        /// <summary>
        /// Initialise node
        /// </summary>
        /// <returns></returns>
        protected override void Init()
        {
            // initialise counters to change toggle colour
            Counter = 0;
            Count = 5;

            // create new array of boolean for each of the features in the data type and set all to true
            ToggleSwitches = new bool[FeatureValues.Values.Length];
            for (int i = 0; i < FeatureValues.Values.Length; i++)
                ToggleSwitches[i] = true;

            base.Init();
        }

        /// <summary>
        /// Remove node from graph and asset when deleted
        /// </summary>
        /// <returns></returns>
        public void OnDestroy()
        {
            // Remove reference of this node in the IMLComponent controlling this node (if any)
            var MLController = graph as IMLController;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.DeleteFeatureNode(this);
            }
        }

        //// Check that we are only connecting to a GameObject
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            this.DisconnectIfNotType<BaseExtractorNode, GameObjectNode>(from, to);
        }

        /// <summary>
        /// Remove connection to nodes port
        /// </summary>
        /// <returns></returns>
        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);
        }

        #endregion

        #region IFeature Methods

        /// <summary>
        /// This update fetches the input value connected to this data type node
        /// </summary>
        /// <returns></returns>
        object IFeatureIML.UpdateFeature()
        {
            return null; // Replace this
        }

        #endregion
    
    }
}

