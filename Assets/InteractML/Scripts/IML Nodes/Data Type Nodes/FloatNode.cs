using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    /// <summary>
    /// Node containing IMLFloat Feature - receiving a float or editable float field 
    /// </summary>
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

        /// <summary>
        /// Initialise node
        /// </summary>
        /// <returns></returns>
        protected override void Init()
        {
            // initialise variables
            PreviousFeatureValues = new IMLFloat();
            UserInput = new IMLFloat();

            // load node specific tooltips
            tooltips = IMLTooltipsSerialization.LoadTooltip("Float");

            base.Init();
        }

        // Check that a feature connected is of the right type
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            // control what connections the input port accepts 
            if (to.node == this)
            {
                // only allow 1 connection (but don't override the original - user must disconnect original input to connect a different one)
                if (this.GetInputNodesConnected("m_In").Count > 1) { from.Disconnect(to); }

                // if array check number of elements with number of features
                DataTypeNodeMethods.CheckArraySizeAgainstFeatureValues(this, from, to);

                // check incoming node type and port data type is accepted by input port
                System.Type[] portTypesAccept = new System.Type[] { typeof(float), typeof(int), typeof(float[]) };
                System.Type[] nodeTypesAccept = new System.Type[] { typeof(IFeatureIML), typeof(MLSystem), typeof(ScriptNode) };
                this.DisconnectPortAndNodeIfNONETypes(from, to, portTypesAccept, nodeTypesAccept);
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

                // update values in node
                Value = ReceivedValue[0];
            }
            else
            {
                // update node value based on input
                base.Update();
                

                // convert input float to float array
                ReceivedValue[0] = Value;

                // check if each toggle is off and set feature value to 0, return float array of updated feature values
                ReceivedValue = DataTypeNodeMethods.CheckTogglesAndUpdateFeatures(this, ReceivedValue);

                // update values in node
                Value = ReceivedValue[0];
            }

            return this;

        }

    }

}

