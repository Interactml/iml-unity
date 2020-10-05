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

        // Use this for initialization
        protected override void Init()
        {
            UserInput = new IMLVector2();

            tooltips = IMLTooltipsSerialization.LoadTooltip("Vector2");
            
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
            //check if receiving data
            if (Counter == Count)
            {
                Counter = 0;
                if (PreviousFeatureValues == FeatureValues)
                    ReceivingData = false;
                else
                    ReceivingData = true;

                PreviousFeatureValues = FeatureValues;
            }

            Counter++;

            //If there is no input connected take input from the user
            if (this.GetInputNodesConnected("m_In") == null)
            {

                InputConnected = false;

                // for each of the feature values 
                for (int i = 0; i < FeatureValues.Values.Length; i++)
                {
                    // check toggle array length is the same as the amount of values in the user input 
                    //if toggles are off set user input value to 0
                    if (!ToggleSwitches[i]) { UserInput.Values[i] = 0; }
                }

                //Set feature value to user input
                Vector2 v = new Vector2(UserInput.Values[0], UserInput.Values[1]);
                Value = v;
            }
            else
            {
                InputConnected = true;

                base.Update();

                var receivedValue = Value;
                // for each of the feature values 
                for (int i = 0; i < FeatureValues.Values.Length; i++)
                {
                    if (!ToggleSwitches[i]) { receivedValue[i] = 0; }
                }

                Value = receivedValue;
            }

            return this;

        }
    }
}