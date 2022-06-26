using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    [NodeWidth(250)]
    [CreateNodeMenuAttribute("Interact ML/Variables/Array")]
    public class ArrayNode : BaseDataTypeNode<float[]>
    {
        // Override set behaviour to avoid passing data by reference because of arrays
        // Input
        public override float[] In { get => base.In; set => base.In = CopyTo(value, base.In); }

        // Value itself contained in the node
        public override float[] Value { get => base.Value; set => base.Value = CopyTo(value, base.Value); }

        // Output
        public override float[] Out { get => base.Out; set => base.Out = CopyTo(value, base.Out); }

        // IML Feature
        public override IMLBaseDataType FeatureValues
        {
            get
            {
                // Make sure feature value is never null
                if (m_FeatureValues == null) 
                    m_FeatureValues = new IMLArray();
                    

                // Update local IML Data copy
                m_FeatureValues.SetValues(Value);
                return m_FeatureValues;
            }
        }
        /// <summary>
        /// Local specific IML data type
        /// </summary>
        private IMLArray m_FeatureValues;

        /// <summary>
        /// Set maximum length of array accepted
        /// </summary>
        public int m_MaximumArraySize;

        /// <summary>
        /// Initialise node
        /// </summary>
        /// <returns></returns>
        public override void Initialize()
        {
            // Make sure feature value is never null
            if (m_FeatureValues == null)
                m_FeatureValues = new IMLArray();

            PreviousFeatureValues = new IMLArray();

            m_MaximumArraySize = 150;

            base.Initialize();
        }

        protected override object Update()
        {
            base.Update();

            // update if node is receiving data
            ReceivingData = DataTypeNodeMethods.IsReceivingData(this);

            ReceivedValue = Value;
            // check if each toggle is off and set feature value to 0, return float array of updated feature values
            ReceivedValue = DataTypeNodeMethods.CheckTogglesAndUpdateFeatures(this, ReceivedValue);

            Value = ReceivedValue;

            // check arrays and create new array of boolean for each of the features in the data type and set all to true
            DataTypeNodeMethods.UpdateToggleSwitchArray(this, FeatureValues.Values.Length);
            DataTypeNodeMethods.UpdateReceivingDataArray(this, FeatureValues.Values.Length);

            return this;
        }

        // Check that a feature connected is of the right type, refill toggleswitch array if array size changes
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            // control what connections the input port accepts 
            if (to.node == this)
            {
                // only allow 1 connection (but don't override the original - user must disconnect original input to connect a different one)
                if (this.GetInputNodesConnected("m_In").Count > 1) { from.Disconnect(to); }

                // check incoming node type and port data type is accepted by input port
                System.Type[] portTypesAccept = new System.Type[] { typeof(float), typeof(int), typeof(Vector2), typeof(Vector3), typeof(Vector4), typeof(float[]) };
                System.Type[] nodeTypesAccept = new System.Type[] { typeof(IFeatureIML), typeof(MLSystem), typeof(ScriptNode) };

                if (!this.DisconnectPortAndNodeIfNONETypes(from, to, portTypesAccept, nodeTypesAccept)) { 
                    // set float array to size matching amount of features
                    ReceivedValue = new float[FeatureValues.Values.Length];
                }

                // if connected a float array to input port
                if (to.GetInputValue().GetType() == typeof(float[]))
                {
                    if (to.node.GetInputValue<float[]>("m_In").Length > m_MaximumArraySize)
                        from.Disconnect(to);
                }
            }

        }

        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);

            if (port.IsInput)
            {
                // reset feature
                ReceivedValue = new float[0];
                Value = new float[0];
                m_FeatureValues.SetValues(Value);
            }
        }

        /// <summary>
        /// Copies data from incoming array to our array
        /// </summary>
        /// <param name="arrayIn"></param>
        /// <param name="arrayOut"></param>
        private float[] CopyTo(float[] arrayIn, float[] arrayOut)
        {
            // Null checks
            if (arrayIn == null)
                throw new System.Exception("Array In is null when attempting to copy in Array node.");
            if (arrayOut == null)
                arrayOut = new float[arrayIn.Length];
                //throw new System.Exception("Array out is null when attempting to copy in Array node.");
            // Length check
            if (arrayIn.Length != arrayOut.Length)
                arrayOut = new float[arrayIn.Length];
            // Copy contents to array out
            arrayIn.CopyTo(arrayOut, 0);            
            // return arrayout
            return arrayOut;
        }

    }
}