using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractML
{
    /// <summary>
    /// Defines the structure of an IML Input
    /// </summary>
    [System.Serializable]
    public class IMLInput
    {
        public IMLSpecifications.InputsEnum InputType;
        public IMLBaseDataType InputData;

        

        public IMLInput()
        {
            //UnityEngine.Debug.Log("IMLInput Constructor called");
        }

        public IMLInput(IMLInput newInput)
        {
            if (newInput != null)
            {
                SetInputData(newInput.InputData);
                InputType = newInput.InputType;
            }
        }

        public IMLInput(IMLBaseDataType newInputData)
        {
            SetInputData(newInputData);
        }

        public IMLInput(float[] newInputData)
        {
            // If the data passed in matches in length the one we expect...
            if (newInputData.Length == InputData.Values.Length)
            { 
                for (int i = 0; i < InputData.Values.Length; i++)
                    InputData.Values[i] = newInputData[i];
            }
            else
            { 
                UnityEngine.Debug.LogError("Length of newInputData not matching expected length of Input Feature!");
            }
        }

        private void SetInputData(IMLBaseDataType newInputData)
        {
            if (InputData == null)
            {
                IMLDataSerialization.InstantiateIMLData(ref InputData, newInputData);
            }
            // Make sure we are instantiating a new copy, not a reference
            InputData.Values = new float[newInputData.Values.Length];
            Array.Copy(newInputData.Values, InputData.Values, newInputData.Values.Length);

        }

        private void SetDataType()
        {

        }


    }

}