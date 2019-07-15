using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML
{
    /// <summary>
    /// Outputs the realtime IML Predictions from an IML Configuration node
    /// </summary>
    public class RealtimeIMLOutputNode : Node
    {
        /// <summary>
        /// The model output we want to export outside of the IML Controller
        /// </summary>
        [Input]
        public Node IMLModelOutputs;

        /// <summary>
        /// The output vector of the IML Config node connected
        /// </summary>
        [SerializeField]
        private double[] m_IMLOutputVector;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

        /// <summary>
        /// Returns the output from the IML Config Node connected
        /// </summary>
        /// <returns></returns>
        public double[] GetIMLControllerOutputs()
        { 
            var IMLConfigNodeConnected = GetInputValue<Node>("IMLModelOutputs") as IMLConfiguration;

            // If there is an IML node connected...
            if (IMLConfigNodeConnected)
            {
                m_IMLOutputVector = IMLConfigNodeConnected.DelayedPredictedOutput;

                //for (int i = 0; i < m_IMLOutputVector.Length; i++)
                //{
                //    Debug.Log("Realtime Output Graph DEBUG: " + m_IMLOutputVector[i]);
                //}

                // Return the raw rapidlib array
                return m_IMLOutputVector;
            }
            // We return null if there is nothing connected
            else
            {
                return null;
            }

        } 
    }
}
