using UnityEngine;

namespace InteractML.DataTypeNodes
{
    public class FloatNode : BaseDataTypeNode<float>
    {
        // IML Feature
        public override IMLBaseDataType FeatureValues
        {
            get
            {
                // Update local IML Data copy
                m_FeatureValues.SetValue(Value);
                return m_FeatureValues;
            }
        }
        /// <summary>
        /// Local specific IML data type
        /// </summary>
        private IMLFloat m_FeatureValues;

    }
}
