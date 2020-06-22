using UnityEngine;
using XNode;

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

        public bool ReceivingData;
        public bool InputConnected;
        public float m_UserInput;
        float receivedFloat;
        public bool float_switch = true;
        float f;
        int counter, count;

        [HideInInspector]
        public IMLNodeTooltips tips;

        // Use this for initialization
        protected override void Init()
        {
            counter = 0;
            count = 5;
            tips = IMLTooltipsSerialization.LoadTooltip("Float");
            base.Init();
        }

        // Check that a feature connected is of the right type
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);

            // Make sure that the IFeatureIML connected is matching our type
            this.DisconnectFeatureNotSameIMLDataType(from, to, IMLSpecifications.DataTypes.Float);

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

            counter++;

            //check if input connected
            if (this.GetInputNodesConnected("m_In") == null)
            {
                InputConnected = false;
                if (!float_switch) m_UserInput = 0;
                Value = m_UserInput;
            }
            else
            {
                InputConnected = true;
                base.Update();
                receivedFloat = Value;
                if (!float_switch) receivedFloat = 0;
                Value = receivedFloat;
            }

            return this;

        }

    }
}
