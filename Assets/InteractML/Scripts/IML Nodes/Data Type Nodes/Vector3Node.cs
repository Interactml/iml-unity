using UnityEngine;
using XNode;

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

        public bool ReceivingData;
        public bool InputConnected;
        public Vector3 m_UserInput;
        Vector3 receivedVector3;

        public bool x_switch = true;
        public bool y_switch = true;
        public bool z_switch = true;
        float x, y, z;
        int counter, count;

        [HideInInspector]
        public IMLNodeTooltips tips;

        // Use this for initialization
        protected override void Init()
        {
            counter = 0;
            count = 5;

            base.Init();

            tips = IMLTooltipsSerialization.LoadTooltip("Vector3");
        }

        // Check that a feature connected is of the right type
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);

            // Make sure that the IMLDataType or feature connected is matching our type
            this.DisconnectFeatureNotSameIMLDataType(from, to, IMLSpecifications.DataTypes.Vector3);


        }

        /// <summary>
        /// Updates Feature values
        /// </summary>
        /// <returns></returns>
        protected override object Update()
        {

            //check if receiving data
            if (counter == count)
            {
                counter = 0;
                if ((x == FeatureValues.Values[0] || !x_switch) && y == FeatureValues.Values[1] && z == FeatureValues.Values[2])
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
            }

            counter++;

            //check if input connected
            if (this.GetInputNodesConnected("m_In") == null)
            {
                InputConnected = false;
                if (!x_switch) m_UserInput.x = 0;
                if (!y_switch) m_UserInput.y = 0;
                if (!z_switch) m_UserInput.z = 0;
                
                Value = m_UserInput;
            }
            else
            {
                InputConnected = true;
                base.Update();
                receivedVector3 = Value;
                if (!x_switch) receivedVector3.x = 0;
                if (!y_switch) receivedVector3.y = 0;
                if (!z_switch) receivedVector3.z = 0;
                Value = receivedVector3;
            }

            return this;

        }

    }
}