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

        /// <summary>
        /// Local specific Vector2 Value
        /// </summary>
        private Vector2 m_UpdatedValue;

        // Use this for initialization
        public override void Initialize()
        {
            // initialise variables
            PreviousFeatureValues = new IMLVector2();
            UserInput = new IMLVector2();
            m_UpdatedValue = new Vector2();

            base.Initialize();
        }

        // Check that a feature connected is of the right type
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            // control what connections the input port accepts 
            if (to.node == this)
            {
                // only allow 1 connection (but don't override the original - user must disconnect original input to connect a different one)
                if (this.GetInputNodesConnected("m_In").Count > 1) { from.Disconnect(to); }

                // check incoming node type and port data type is accepted by input port
                base.OnCreateConnection(from, to);

                // if array check number of elements with number of features
                DataTypeNodeMethods.CheckArraySizeAgainstFeatureValues(this, from, to);


            }

        }

        /// <summary>
        /// Updates Feature values
        /// </summary>
        /// <returns></returns>
        protected override object Update()
        {
            // update if node is receiving data
            ReceivingData = DataTypeNodeMethods.IsReceivingData(this);

            // update if node has input connected
            InputConnected = DataTypeNodeMethods.IsInputConnected(this);

            // if there is no input connected take input from the user
            if (!InputConnected)
            {
                // check if each toggle is off and set feature value to 0, return float array of updated feature values
                ReceivedValue = DataTypeNodeMethods.CheckTogglesAndUpdateFeatures(this, UserInput.Values);

                // convert float array to vector3
                m_UpdatedValue.x = ReceivedValue[0];
                m_UpdatedValue.y = ReceivedValue[1];

                // update values in node
                Value = m_UpdatedValue;
            }
            else
            {
                // update node value based on input
                base.Update();

                // convert input vector to float array
                ReceivedValue[0] = Value.x;
                ReceivedValue[1] = Value.y;

                // check if each toggle is off and set feature value to 0, return float array of updated feature values
                ReceivedValue = DataTypeNodeMethods.CheckTogglesAndUpdateFeatures(this, ReceivedValue);

                // convert float array to vector3 
                m_UpdatedValue.x = ReceivedValue[0];
                m_UpdatedValue.y = ReceivedValue[1];

                // update values in node
                Value = m_UpdatedValue;
            }

            return this;

        }

    
    }
}