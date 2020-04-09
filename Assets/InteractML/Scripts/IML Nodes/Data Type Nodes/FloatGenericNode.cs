using UnityEngine;

namespace InteractML.DataTypeNodes
{
    public class FloatGenericNode : BaseDataTypeNode<float>
    {

        // Input
        public override float In { get { return m_In; } set { m_In = value; } }
        [Input, SerializeField]
        private float m_In;

        // Value itself contained in the node
        public override float Value { get { return m_Value; } set { m_Value = value; } }
        [SerializeField]
        private float m_Value;

        // Output
        public override float Out { get { return m_Out; } set { m_Out = value; } }
        [Output, SerializeField]
        private float m_Out;

    }
}
