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
        public bool float_switch = true;
        float f;
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

            if (!float_switch)
                FeatureValues.Values[0] = 0;

            return this;

        }

    }
}
