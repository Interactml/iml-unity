using UnityEngine;

namespace InteractML.DataTypeNodes
{
    [NodeWidth(250)]
    public class Vector3Node : BaseDataTypeNode<Vector3>, IFeatureIML
    {
        // IML Feature
        public override IMLBaseDataType FeatureValues
        {
            get
            {
                // Make sure the feature values are never null
                if (m_FeatureValues == null)
                    m_FeatureValues = new IMLVector3();

                // Update local IML Data copy
                m_FeatureValues.SetValues(Value);
                return m_FeatureValues;
            }
        }
        /// <summary>
        /// Local specific IML data type
        /// </summary>
        private IMLVector3 m_FeatureValues;

    }
}