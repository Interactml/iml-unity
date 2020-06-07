using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    [NodeWidth(250)]
    public class Vector2Node : BaseDataTypeNode<Vector2>
    {
        // IML Feature
        public override IMLBaseDataType FeatureValues
        {
            get
            {
                // Make sure feature value is never null
                if (m_FeatureValues == null)
                    m_FeatureValues = new IMLVector2();

                // Update local IML Data copy
                m_FeatureValues.SetValues(Value);
                return m_FeatureValues;
            }
        }
        /// <summary>
        /// Local specific IML data type
        /// </summary>
        private IMLVector2 m_FeatureValues;

        public bool ReceivingData;
        public bool x_switch = true;
        public bool y_switch = true;
        float x, y;
        int counter, count;

        // Use this for initialization
        protected override void Init()
        {
            counter = 0;
            count = 5;

            base.Init();
        }

        // Check that a feature connected is of the right type
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);

            // Make sure that the IFeatureIML connected is matching our type
            this.DisconnectFeatureNotSameIMLDataType(from, to, IMLSpecifications.DataTypes.Vector2);

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
                if ((x == FeatureValues.Values[0] || !x_switch) && y == FeatureValues.Values[1])
                {
                    ReceivingData = false;
                }
                else
                {
                    ReceivingData = true;

                }
                x = FeatureValues.Values[0];
                y = FeatureValues.Values[1];
            }

            counter++;

            if (!x_switch)
                FeatureValues.Values[0] = 0;

            if (!y_switch)
                FeatureValues.Values[1] = 0;

            return this;

        }
    }
}