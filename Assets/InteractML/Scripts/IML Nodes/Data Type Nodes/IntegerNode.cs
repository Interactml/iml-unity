using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    [NodeWidth(250)]
    public class IntegerNode : BaseDataTypeNode<int>
    {                
        // IML Feature
        public override IMLBaseDataType FeatureValues
        {
            get
            {
                // Make sure feature value is never null
                if (m_FeatureValues == null)
                    m_FeatureValues = new IMLInteger();

                // Update local IML Data copy
                m_FeatureValues.SetValue(Value);
                return m_FeatureValues;
            }
        }

        /// <summary>
        /// Local specific IML data type
        /// </summary>
        private IMLInteger m_FeatureValues;

        public int m_UserInput;
        public int receivedInt;
        public bool int_switch = true;
        public float f;

        // Use this for initialization
        protected override void Init()
        {
            tooltips = IMLTooltipsSerialization.LoadTooltip("Int");
            base.Init();
        }

        // Check that a feature connected is of the right type
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);

            // Make sure that the IFeatureIML connected is matching our type
            this.DisconnectFeatureNotSameIMLDataType(from, to, IMLSpecifications.DataTypes.Integer);

        }

        /// <summary>
        /// Updates Feature values
        /// </summary>
        /// <returns></returns>
        protected override object Update()
        {
            base.Update();
            //check if receiving data
            if (Counter == Count)
            {
                Counter = 0;
                if ((f == FeatureValues.Values[0]))
                {
                    ReceivingData = false;
                }
                else
                {
                    ReceivingData = true;

                }
                f = FeatureValues.Values[0];
            }

            Counter++;

            //check if input connected
            if (this.GetInputNodesConnected("m_In") == null)
            {
                InputConnected = false;
                if (!int_switch) m_UserInput = 0;
                Value = m_UserInput;
            }
            else
            {
                InputConnected = true;
                base.Update();
                receivedInt = Value;
                if (!int_switch) receivedInt = 0;
                Value = receivedInt;
            }
            return this;

        }

    }
}
