using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;

namespace InteractML.FeatureExtractors
{
    /// <summary>
    /// Extracts a serialVector array into a Feature for a ml model
    /// </summary>
    [NodeTint("#3A3B5B")]
    [NodeWidth(250)]
    public class ExtractSerialVector : Node, IFeatureIML
    {
        /// <summary>
        /// Array of floats from which we extract a feature
        /// </summary>
        [Input]
        public float[] inputSerialVector;

        /// <summary>
        /// Node data sent outside of this node onwards
        /// </summary>
        [Output]
        public Node serialVectorFeatureExtracted;

        /// <summary>
        /// Feature Values extracted (ready to be read by a different node)
        /// </summary>
        public IMLBaseDataType FeatureValues { get { return m_arrayExtracted; } }

        /// <summary>
        /// The private feature values extracted in a more specific data type
        /// </summary>
        [SerializeField]
        private IMLSerialVector m_arrayExtracted;

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

            if (m_arrayExtracted == null)
            {
                m_arrayExtracted = new IMLSerialVector();

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
            float[] valueToOutput = GetInputValue<float[]>("inputSerialVector", this.inputSerialVector);

            if (m_arrayExtracted == null)
            {
                m_arrayExtracted = new IMLSerialVector();

            }

            m_arrayExtracted.SetValues(valueToOutput);

            return this;

        }
    }

}
