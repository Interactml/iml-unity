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
    }
}