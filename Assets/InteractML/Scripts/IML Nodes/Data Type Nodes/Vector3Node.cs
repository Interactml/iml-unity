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

        public bool ReceivingData;
        public bool x_switch = true;
        public bool y_switch = true;
        public bool z_switch = true;
        float x, y, z;
        int counter, count;

        // Use this for initialization
        protected override void Init()
        {
            counter = 0;
            count = 5;

            base.Init();
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

            if (!x_switch)
                FeatureValues.Values[0] = 0;

            if (!y_switch)
                FeatureValues.Values[1] = 0;

            if (!z_switch)
                FeatureValues.Values[2] = 0;

            return this;

        }

    }
}