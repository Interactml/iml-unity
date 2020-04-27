using UnityEngine;

namespace InteractML.DataTypeNodes
{
    [NodeWidth(250)]
    public class FloatNode : BaseDataTypeNode<float>
    {
        // IML Feature
        public override IMLBaseDataType FeatureValues
        {
            get
            {
                // Make sure feature value is never null
                if (m_FeatureValues == null)
                    m_FeatureValues = new IMLFloat();

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
