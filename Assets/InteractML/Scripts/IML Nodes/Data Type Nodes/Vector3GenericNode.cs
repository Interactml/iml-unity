using UnityEngine;

namespace InteractML.DataTypeNodes
{
    public class Vector3GenericNode : BaseDataTypeNode<Vector3>
    {

        // Input
        public override Vector3 In { get { return m_In; } set { m_In = value; } }
        [Input, SerializeField]
        private Vector3 m_In;

        // Value itself contained in the node
        public override Vector3 Value { get { return m_Value; } set { m_Value = value; } }
        [SerializeField]
        private Vector3 m_Value;

        // Output
        public override Vector3 Out { get { return m_Out; } set { m_Out = value; } }
        [Output, SerializeField]
        private Vector3 m_Out;

    }
}