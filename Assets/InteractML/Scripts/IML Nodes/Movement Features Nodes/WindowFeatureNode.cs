using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;

namespace InteractML.GameObjectMovementFeatures
{
    /// <summary>
    /// Extract a window of features from any other collection of features
    /// </summary>
    [NodeWidth(250)]
    [CreateNodeMenuAttribute("Interact ML/Movement Features/Window of Features")]
    public class WindowFeatureNode : BaseMovementFeatureNode, IFeatureIML
    {
        #region Variables

        /// <summary>
        /// The list of features that are being input to this node
        /// </summary>
        [Input]
        public List<Node> FeaturesAsInput;

        /// <summary>
        /// The window sent outside of this node onwards
        /// </summary>
        [Output]
        public Node LiveDataOut;

        /// <summary>
        /// Feature values extracted (ready to be read by a different node)
        /// </summary>
        public override IMLBaseDataType FeatureValues { get => m_WindowExtracted; }
        
        /// <summary>
        /// The window of features extracted
        /// </summary>
        private IMLArray m_WindowExtracted;
        /// <summary>
        /// Raw values of the window (internally used)
        /// </summary>
        private float[] m_WindowRawValues;

        /// <summary>
        /// Size of the window to calculate (including feature length + num samples)
        /// </summary>
        private int m_WindowTotalSize;
        [SerializeField]
        private int m_LastWindowSamples;
        [SerializeField]
        private int m_WindowSamples = 1;
        /// <summary>
        /// How many samples are taken for the window
        /// </summary>
        public int WindowSamples { get => m_WindowSamples; set => m_WindowSamples = value; }

        /// <summary>
        /// Lets external classes known if they should call UpdateFeature
        /// </summary>
        public bool isExternallyUpdatable { get { return isConnectedToSomething; } }

        /// <summary>
        /// Private logic to know when this node should be updatable
        /// </summary>
        private bool isConnectedToSomething { get { return (Outputs.Count() > 0); } }

        /// <summary>
        /// Was the feature already updated?
        /// </summary>
        public bool isUpdated { get; set; }

        #endregion

        #region IML Feature Methods

        public override void Initialize()
        {
            // This extractor expects any other feature extracted to make calculations
            GetInputFeatures(ref FeaturesAsInput);

            // If we managed to get the inputs
            if (FeaturesAsInput != null && FeaturesAsInput.Count > 0 && m_WindowSamples > 0)
            {
                RecalculateWindowSize(FeaturesAsInput, m_WindowSamples, 
                    ref m_LastWindowSamples, ref m_WindowTotalSize, ref m_WindowRawValues, ref m_WindowExtracted);
            }
            else
            {
                // init arrays
                m_WindowRawValues = new float[0];
                m_WindowExtracted = new IMLArray(m_WindowRawValues);
                // initialise toggle and receiving data bool arrays
                FeatureValueReceivingData = new bool[0];
                ToggleSwitches = new bool[0];
            }

            // initialise counters to change toggle colour
            Counter = 0;
            Count = 5;
        }

        public object UpdateFeature()
        {            
            // This extractor expects any other feature extracted to make calculations
            GetInputFeatures(ref FeaturesAsInput);

            if (m_LastWindowSamples != m_WindowSamples) RecalculateWindowSize(FeaturesAsInput, m_WindowSamples, 
                ref m_LastWindowSamples, ref m_WindowTotalSize, ref m_WindowRawValues, ref m_WindowExtracted);

            // If we managed to get the inputs
            if (FeaturesAsInput != null && FeaturesAsInput.Count > 0 && m_WindowSamples > 0 && m_WindowTotalSize > 0)
            {
                if (!isUpdated)
                {
                    // iterate through features, taking into account the sample size
                    int arrayIndex = 0;
                    foreach (var feature in FeaturesAsInput)
                    {
                        var featureToUse = (feature as IFeatureIML).FeatureValues;
                        if (featureToUse != null && arrayIndex < m_WindowRawValues.Length)
                        {
                            // Add feature values to raw features array
                            featureToUse.Values.CopyTo(m_WindowRawValues, arrayIndex);
                            // do we have several window samples? copy values in corresponding indexes
                            if (WindowSamples > 1)
                            {
                                for (int i = 1; i < WindowSamples; i++)
                                {
                                    int windowSlice = m_WindowTotalSize / m_WindowSamples;
                                    featureToUse.Values.CopyTo(m_WindowRawValues, arrayIndex + (windowSlice * i));
                                }
                            }
                            arrayIndex += featureToUse.Values.Length;
                        }
                    }
                    // Populate IML array from window values
                    m_WindowExtracted.SetValues(m_WindowRawValues);

                    // check if inputs have changed and update size of toggle bool array and receiving data bool array
                    MovementFeatureMethods.UpdateToggleSwitchArray(this, m_WindowRawValues.Length);
                    MovementFeatureMethods.UpdateReceivingDataArray(this, m_WindowRawValues.Length);

                    // Make sure to mark the feature as updated to avoid calculating twice
                    isUpdated = true;
                }


                return this;
            }
            // If we couldn't get an input, we return null
            else
            {
                return null;
            }

        }

        #endregion

        #region Private Methods

        private void GetInputFeatures(ref List<Node> FeatureValues)
        {
            FeaturesAsInput = GetInputValue<List<Node>>("FeaturesAsInput");
            if (FeaturesAsInput == null) FeaturesAsInput = new List<Node>();
            var featuresConnected = this.GetInputNodesConnected("FeaturesAsInput");
            if (featuresConnected == null) return;
            foreach (var feature in featuresConnected)
            {
                if (!FeaturesAsInput.Contains(feature)) FeaturesAsInput.Add(feature);
            }

        }

        private void RecalculateWindowSize(List<Node> features, int windowSamples, ref int lastWindowSamples, ref int windowTotalSize, ref float[] windowRawValues, ref IMLArray windowExtracted)
        {
            // If we managed to get the inputs
            if (features != null && features.Count > 0 && windowSamples > 0)
            {
                // Reset window size calculation
                windowTotalSize = 0;
                // We check that they are IML Features
                foreach (var feature in features)
                {
                    var featureToUse = (feature as IFeatureIML).FeatureValues;
                    if (featureToUse != null)
                    {
                        // Add feature length to the window total size
                        windowTotalSize += featureToUse.Values.Length;
                    }
                }
                // Multiply samples that want to be taken by total feature length
                windowTotalSize = windowTotalSize * windowSamples;
                // Create array for window values
                windowRawValues = new float[m_WindowTotalSize];
                windowExtracted = new IMLArray(m_WindowRawValues);

                // check if inputs have changed and update size of toggle bool array and receiving data bool array
                MovementFeatureMethods.UpdateToggleSwitchArray(this, windowRawValues.Length);
                MovementFeatureMethods.UpdateReceivingDataArray(this, windowRawValues.Length);

                // save last window samples in case of a change of sample
                lastWindowSamples = windowSamples;
            }

        }

        #endregion

        #region XNode Messages

        public override object GetValue(NodePort port)
        {
            return UpdateFeature();
        }

        // Check we are only connecting to all data type nodes and movement feature nodes, MLS or script node
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            // control what connections the input port accepts (not output port)
            if (to.node == this)
            {
                System.Type[] portTypesAccept = new System.Type[] { };
                System.Type[] nodeTypesAccept = new System.Type[] { typeof(IFeatureIML), typeof(MLSystem), typeof(ScriptNode) };
                this.DisconnectPortAndNodeIfNONETypes(from, to, portTypesAccept, nodeTypesAccept);

                Initialize();
            }
        }

        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);
            if (port.IsInput)
            {
                // need to reset
                Initialize();
            }

        }


        #endregion
    }
}