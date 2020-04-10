using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;

namespace InteractML.FeatureExtractors
{
    /// <summary>
    /// Gets a Vector4 and converts it to a IMLVector4 Feature
    /// </summary>
    [NodeTint("#3A3B5B")]
    [NodeWidth(250)]
    public class ExtractVector4 : Node, IFeatureIML
    {
        /// <summary>
        /// Dummy float from which we extract a feature
        /// </summary>
        [Input]
        public Vector4 inputVector4;

        /// <summary>
        /// Node data sent outside of this node onwards
        /// </summary>
        [Output]
        public Node vector4FeatureExtracted;

        /// <summary>
        /// Feature Values extracted (ready to be read by a different node)
        /// </summary>
        public IMLBaseDataType FeatureValues { get { return m_v4Extracted; } }

        /// <summary>
        /// The private feature values extracted in a more specific data type
        /// </summary>
        [SerializeField]
        private IMLVector4 m_v4Extracted;

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

            if (m_v4Extracted == null)
            {
                m_v4Extracted = new IMLVector4();

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
            Vector4 valueToOutput = GetInputValue<Vector4>("inputVector4", this.inputVector4);

            if (m_v4Extracted == null)
            {
                m_v4Extracted = new IMLVector4();

            }

            m_v4Extracted.SetValues(valueToOutput);

            return this;
        }
    }
}
