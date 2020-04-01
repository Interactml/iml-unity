using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;


namespace InteractML.FeatureExtractors
{
    /// <summary>
    /// Gets an int and converts it to a IMLInteger Feature
    /// </summary>
    [NodeTint("#3A3B5B")]
    public class ExtractInt : Node, IFeatureIML
    {
        /// <summary>
        /// Dummy float from which we extract a feature
        /// </summary>
        [Input]
        public int inputInteger;

        /// <summary>
        /// Node data sent outside of this node onwards
        /// </summary>
        [Output]
        public Node integerFeatureExtracted;

        /// <summary>
        /// Feature Values extracted (ready to be read by a different node)
        /// </summary>
        public IMLBaseDataType FeatureValues { get { return m_IntExtracted; } }

        /// <summary>
        /// The private feature values extracted in a more specific data type
        /// </summary>
        [SerializeField]
        private IMLInteger m_IntExtracted;

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

            if (m_IntExtracted == null)
            {
                m_IntExtracted = new IMLInteger();

            }

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

            int valueToOutput = GetInputValue<int>("inputInteger", this.inputInteger);

            if (m_IntExtracted == null)
            {
                m_IntExtracted = new IMLInteger();

            }

            m_IntExtracted.SetValue(valueToOutput);

            return this;

        }

    }

}

