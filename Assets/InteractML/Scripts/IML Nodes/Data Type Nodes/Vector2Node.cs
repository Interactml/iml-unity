using UnityEngine;

namespace InteractML.DataTypeNodes
{
    public class Vector2Node : BaseDataTypeNode<Vector2>
    {
        // IML Feature
        public override IMLBaseDataType FeatureValues
        {
            get
            {
                // Update local IML Data copy
                m_FeatureValues.SetValues(Value);
                return m_FeatureValues;
            }
        }
        /// <summary>
        /// Local specific IML data type
        /// </summary>
        private IMLVector2 m_FeatureValues;

    }
}