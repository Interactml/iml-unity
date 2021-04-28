using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using InteractML;
using System.Linq;

namespace InteractML.GameObjectMovementFeatures
{
    /// <summary>
    /// Extracts the distance from one or several features to another one (i.e. fingers to the palm of the hand)
    /// </summary>
    [NodeWidth(250)]
    public class DistanceToFirstInputNode : BaseMovementFeatureNode, IFeatureIML
    {
        /// <summary>
        /// The feature that has been previously extracted and from which we are calculating the distance from (i.e. position, rotation, etc)
        /// </summary>
        [Input]
        public Node FirstInput;

        /// <summary>
        /// The features that we will measure the distance to the first feature to Input
        /// </summary>
        [Input]
        public List<Node> SecondInputs;

        /// <summary>
        /// Node data sent outside of this node onwards
        /// </summary>
        [Output]
        public Node DistanceBetweenInputs;

        /// <summary>
        /// Feature Values extracted (ready to be read by a different node)
        /// </summary>
        public override IMLBaseDataType FeatureValues { get { return m_DistancesExtracted; } }

        /// <summary>
        /// The private feature values extracted in a more specific data type
        /// </summary>
        [SerializeField]
        private IMLArray m_DistancesExtracted;
        private float[] m_DistancesToFirstInput;

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


        // Use this for initialization
        public override void Initialize()
        {
            // Make sure feature extractor value is never null
            if (m_DistancesExtracted == null)
            {
                m_DistancesExtracted = new IMLArray();
                ToggleSwitches = new bool[0];
                FeatureValueReceivingData = new bool[0];
            }

            // check if inputs have changed and update size of toggle bool array and receiving data bool array
            MovementFeatureMethods.UpdateToggleSwitchArray(this, m_DistancesExtracted.Values.Length);
            MovementFeatureMethods.UpdateReceivingDataArray(this, m_DistancesExtracted.Values.Length);

            // initialise helper variables
            PreviousFeatureValues = new IMLArray(m_DistancesExtracted);

            // initialise counters to change toggle colour
            Counter = 0;
            Count = 5;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return UpdateFeature();
        }


        public object UpdateFeature()
        {
            // This extractor expects any other feature extracted to make calculations
            FirstInput = GetInputValue<Node>("FirstInput");
            SecondInputs = GetInputValues<Node>("SecondInputs").ToList();

            if (!isUpdated)
            {
               
                // If there are connections for both input ports
                if (FirstInput != null && SecondInputs.Count > 0)
                {
                    // We check that the first input is an IML feature
                    var firstInputIMLFeature = (FirstInput as IFeatureIML).FeatureValues;
                    if (firstInputIMLFeature != null)
                    {
                        // Clear distances vector with new vector based on the number of second inputs
                        m_DistancesToFirstInput = new float[SecondInputs.Count];

                        for (int i = 0; i < SecondInputs.Count; i++)
                        {
                            // We check that the second inputs are also iml features
                            var secondInputIMLFeature = (SecondInputs[i] as IFeatureIML).FeatureValues;
                            if (secondInputIMLFeature != null)
                            {
                                // We make sure that the extracted value is not null
                                if (m_DistancesExtracted == null)
                                    m_DistancesExtracted = new IMLArray(m_DistancesToFirstInput);
                                    
                                // Calculate distance between each of the entries in the values vector
                                float[] distancesVectorBetweenEachFeature = new float[firstInputIMLFeature.Values.Length];

                                for (int j = 0; j < firstInputIMLFeature.Values.Length; j++)
                                {
                                    distancesVectorBetweenEachFeature[j] = secondInputIMLFeature.Values[j] - firstInputIMLFeature.Values[j];
                                }

                                // Follow the euclidean formula for distance: square and add together all distances
                                for (int j = 0; j < firstInputIMLFeature.Values.Length; j++)
                                {
                                    m_DistancesToFirstInput[i] += (distancesVectorBetweenEachFeature[j] * distancesVectorBetweenEachFeature[j]);
                                }

                                m_DistancesToFirstInput[i] = Mathf.Sqrt(m_DistancesToFirstInput[i]);

                            }
                            // If we couldn't get an input (in the second input), we return null
                            else
                            {
                                Debug.LogError("Could not get second input " + i + " when measuring distance!");
                                m_DistancesExtracted = null;
                                return null;
                            }
    
                        }

                        // Set values for distance extracted and for last frame feature value
                        m_DistancesExtracted.SetValues(m_DistancesToFirstInput);

                        // check if each toggle is off and set feature value to 0, return float array of updated feature values
                        m_DistancesExtracted.Values = MovementFeatureMethods.CheckTogglesAndUpdateFeatures(this, m_DistancesExtracted.Values);

                        // Make sure to mark the feature as updated to avoid calculating twice
                        isUpdated = true;

                        // check if inputs have changed and update size of toggle bool array and receiving data bool array
                        MovementFeatureMethods.UpdateToggleSwitchArray(this, m_DistancesExtracted.Values.Length);
                        MovementFeatureMethods.UpdateReceivingDataArray(this, m_DistancesExtracted.Values.Length);

                        // initialise helper variables
                        if (PreviousFeatureValues.Values == null)
                            PreviousFeatureValues = new IMLArray(m_DistancesExtracted);

                        // update if node is receiving data
                        ReceivingData = MovementFeatureMethods.IsReceivingData(this);

                        return this;
                    }
                    // If we couldn't get an input (in the first input), we return null
                    else
                    {
                        Debug.LogError("Could not get first input when measuring distance!");
                        m_DistancesExtracted = null;
                        return null;
                    }
                }
                // If we couldn't get an input (at all), we return null
                else
                {
                    m_DistancesExtracted = null;
                    return null;
                }

            }
            // Avoid calculating again and return the feature
            else
            {
                return this;
            }
            
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            // control what connections the input port accepts (not output port)
            if (to.node == this)
            {
                System.Type[] portTypesAccept = new System.Type[] { };
                System.Type[] nodeTypesAccept = new System.Type[] { typeof(IFeatureIML), typeof(MLSystem) };
                this.DisconnectPortAndNodeIfNONETypes(from, to, portTypesAccept, nodeTypesAccept);

                // control what connections the first port accepts
                if (to.fieldName == "FirstInput")
                {
                    // only allow 1 connection (but don't override the original - user must disconnect original input to connect a different one)
                    if (this.GetInputNodesConnected("FirstInput").Count > 1) { from.Disconnect(to); }

                    // check if there is a connection to the second input
                    if (this.GetInputPort("SecondInputs").IsConnected)
                    {
                        // check if the two inputted types have the same number of features, otherwise disconnect
                        if ((SecondInputs[0] as IFeatureIML).FeatureValues.Values.Length != (to.GetInputValue() as IFeatureIML).FeatureValues.Values.Length)
                        {
                            from.Disconnect(to);
                        }

                    }
                }

                // control what connections the second port accepts
                if (to.fieldName == "SecondInputs")
                {
                    // check if there is a connection to the first input
                    if (this.GetInputPort("FirstInput").IsConnected)
                    {
                        // check if the two inputted types have the same number of features, otherwise disconnect
                        if ((FirstInput as IFeatureIML).FeatureValues.Values.Length != (to.node.GetInputNodesConnected("SecondInputs")[0] as IFeatureIML).FeatureValues.Values.Length)
                        {
                            from.Disconnect(to);
                        }

                    }

                    // check if there is another connection to the second input
                    if (this.GetInputNodesConnected("SecondInputs").Count > 1)
                    {
                        // check if the inputted type has the same number of features as connected types, otherwise disconnect
                        if ((this.GetInputNodesConnected("SecondInputs")[0] as IFeatureIML).FeatureValues.Values.Length != (to.node.GetInputNodesConnected("SecondInputs")[this.GetInputNodesConnected("SecondInputs").Count - 1] as IFeatureIML).FeatureValues.Values.Length)
                        {
                            from.Disconnect(to);
                        }

                    }
                    
                }
            }
        }

    }
}

