using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractML
{
    /// <summary>
    /// Holds a training example
    /// </summary>
    [System.Serializable]
    public class IMLTrainingExample
    {
        private List<IMLInput> m_Inputs;
        private List<IMLOutput> m_Outputs;
        public List<IMLInput> Inputs { get { return m_Inputs; } set { SetInputs(value); } }
        public List<IMLOutput> Outputs { get { return m_Outputs; } set { SetOutputs(value); } }

        public void AddInputExample(IMLBaseDataType inputExample)
        {
            // Check that list is initialised
            if (m_Inputs == null)
                m_Inputs = new List<IMLInput>();

            // Add entry to list with the example passed in
            var newInputExample = new IMLInput(inputExample);
            m_Inputs.Add(newInputExample);
        }

        public void AddOutputExample(IMLBaseDataType outputExample)
        {
            // Check that list is initialised
            if (m_Outputs == null)
                m_Outputs = new List<IMLOutput>();

            // Add entry to list with the example passed in
            var newOutputExample = new IMLOutput(outputExample);
            m_Outputs.Add(newOutputExample);

        }

        public void SetInputs (List<IMLInput> newInputs)
        {
            // Set the new inputs if not null
            if (newInputs != null)
            {
                m_Inputs = newInputs;
            }
            // If after setter is still null, we initialise an empty list
            if (m_Inputs == null)
            {
                m_Inputs = new List<IMLInput>();
            }
        }

        public void SetOutputs (List<IMLOutput> newOutputs)
        {
            // Set the new inputs if not null
            if (newOutputs != null)
            {
                m_Outputs = newOutputs;
            }
            // If after setter is still null, we initialise an empty list
            if (m_Outputs == null)
            {
                m_Outputs = new List<IMLOutput>();
            }

        }

        /// <summary>
        /// Returns all the IML Inputs as one array (places several features one after another)
        /// </summary>
        /// <returns></returns>
        public double[] GetInputs()
        {
            if (m_Inputs == null)
                m_Inputs = new List<IMLInput>();

            // INPUT VECTOR RAPIDLIB
            int rapidlibInputVectorSize = 0;
            // Go through all input features in IML Training Example and get their size
            for (int k = 0; k < m_Inputs.Count; k++)
            {
                rapidlibInputVectorSize += m_Inputs[k].InputData.Values.Length;
            }
            // Create rapidlib input by constructing a vector size of all IML input features combined
            double[] returnValue = new double[rapidlibInputVectorSize];
            // Create a pointer to know keep the boundaries in our input vector while we add IML features
            int pointerVector = 0;
            // Go through all IML input features and add their features to the rapidlib vector
            for (int k = 0; k < m_Inputs.Count; k++)
            {
                // Check that the boundaries are never surpassed
                if (pointerVector > rapidlibInputVectorSize)
                {
                    Debug.LogError("Trying to add input features to an array that is too small!");
                }
                var IMLInputFeature = m_Inputs[k];
                // Add IML data to rapidlib vector
                for (int w = 0; w < IMLInputFeature.InputData.Values.Length; w++)
                {
                    returnValue[w + pointerVector] = IMLInputFeature.InputData.Values[w];
                }
                // Move vector pointer forward
                pointerVector += IMLInputFeature.InputData.Values.Length;
            }       

            return returnValue;
        }

        /// <summary>
        /// Returns all the IML Outputs as one array (places several features one after another)
        /// </summary>
        /// <returns></returns>
        public double[] GetOutputs()
        {
            if (m_Outputs == null)
                m_Outputs = new List<IMLOutput>();

            // OUTPUT VECTOR RAPIDLIB
            int rapidlibOutputVectorSize = 0;
            // Go through all Output features in IML Training Example and get their size
            for (int k = 0; k < m_Outputs.Count; k++)
            {
                rapidlibOutputVectorSize += m_Outputs[k].OutputData.Values.Length;
            }
            // Create rapidlib Output by constructing a vector size of all IML Output features combined
            double[] returnValues = new double[rapidlibOutputVectorSize];
            // Create a pointer to know keep the boundaries in our Output vector while we add IML features
            int pointerVector = 0;
            // Go through all IML Output features and add their features to the rapidlib vector
            for (int k = 0; k < m_Outputs.Count; k++)
            {
                // Check that the boundaries are never surpassed
                if (pointerVector > rapidlibOutputVectorSize)
                {
                    Debug.LogError("Trying to add Output features to an array that is too small!");
                }
                var IMLOutputFeature = m_Outputs[k];
                // Add IML data to rapidlib vector
                for (int w = 0; w < IMLOutputFeature.OutputData.Values.Length; w++)
                {
                    returnValues[w + pointerVector] = IMLOutputFeature.OutputData.Values[w];
                }
                // Move vector pointer forward
                pointerVector += IMLOutputFeature.OutputData.Values.Length;
            }

            return returnValues;
        }
    }
}