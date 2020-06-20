using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    [NodeWidth(250)]
    public class Vector4Node : BaseDataTypeNode<Vector4>
    {
        // IML Feature
        public override IMLBaseDataType FeatureValues
        {
            get
            {
                // Make sure feature value is never null
                if (m_FeatureValues == null)
                    m_FeatureValues = new IMLVector4();
                
                // Update local IML Data copy
                m_FeatureValues.SetValues(Value);
                return m_FeatureValues;
            }
        }
        /// <summary>
        /// Local specific IML data type
        /// </summary>
        private IMLVector4 m_FeatureValues;

        public bool ReceivingData;
        public bool x_switch = true;
        public bool y_switch = true;
        public bool z_switch = true;
        public bool w_switch = true;
        float x, y, z, w;
        int counter, count;

        [HideInInspector]
        public IMLNodeTooltips tips;

        public bool InputConnected;
        public Vector4 m_UserInput;
        Vector4 receivedVector4;

        // Use this for initialization
        protected override void Init()
        {
            counter = 0;
            count = 5;
            tips = IMLTooltipsSerialization.LoadTooltip("Vector4");
            base.Init();
        }

        // Check that a feature connected is of the right type
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);

            // Make sure that the IFeatureIML connected is matching our type
            this.DisconnectFeatureNotSameIMLDataType(from, to, IMLSpecifications.DataTypes.Vector4);

        }

        /// <summary>
        /// Updates Feature values
        /// </summary>
        /// <returns></returns>
        protected override object Update()
        {
            base.Update();
            //check if receiving data
            if (counter == count)
            {
                counter = 0;
                if ((x == FeatureValues.Values[0] || !x_switch) && y == FeatureValues.Values[1] && z == FeatureValues.Values[2] && w == FeatureValues.Values[2])
                {
                    ReceivingData = false;
                }
                else
                {
                    ReceivingData = true;

                }
                x = FeatureValues.Values[0];
                y = FeatureValues.Values[1];
                z = FeatureValues.Values[2];
                w = FeatureValues.Values[2];
            }

            counter++;

            //check if input connected
            if (this.GetInputNodesConnected("m_In") == null)
            {
                InputConnected = false;
                if (!x_switch) m_UserInput.x = 0;
                if (!y_switch) m_UserInput.y = 0;
                if (!z_switch) m_UserInput.z = 0;
                if (!w_switch) m_UserInput.w = 0;

                Value = m_UserInput;
            }
            else
            {
                InputConnected = true;
                base.Update();
                receivedVector4 = Value;
                if (!x_switch) receivedVector4.x = 0;
                if (!y_switch) receivedVector4.y = 0;
                if (!z_switch) receivedVector4.z = 0;
                if (!z_switch) receivedVector4.w = 0;
                Value = receivedVector4;
            }

            return this;

        }

    }
}