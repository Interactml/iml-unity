using UnityEngine;

namespace InteractML.DataTypeNodes
{
    public class Vector2GenericNode : BaseDataTypeNode<Vector2>
    {

        // Input
        public override Vector2 In { get { return m_In; } set { m_In = value; } }
        [Input, SerializeField]
        private Vector2 m_In;

        // Value itself contained in the node
        public override Vector2 Value { get { return m_Value; } set { m_Value = value; } }
        [SerializeField]
        private Vector2 m_Value;

        // Output
        public override Vector2 Out { get { return m_Out; } set { m_Out = value; } }
        [Output, SerializeField]
        private Vector2 m_Out;

    }
}