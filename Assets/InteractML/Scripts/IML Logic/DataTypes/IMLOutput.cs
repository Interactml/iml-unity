using System;
using System.Collections;
using System.Collections.Generic;

namespace InteractML
{
    /// <summary>
    /// Defines the structure of an IML Output
    /// </summary>
    [System.Serializable]
    public class IMLOutput
    {
        public IMLSpecifications.OutputsEnum OutputType;
        public IMLBaseDataType OutputData;

        public IMLOutput()
        {
            //UnityEngine.Debug.Log("IMLOutput Constructor called");
        }

        public IMLOutput(IMLBaseDataType newOutputData)
        {
            SetOutputData(newOutputData);
        }

        private void SetOutputData (IMLBaseDataType newOutputData)
        {
            if (OutputData == null)
            {
                IMLDataSerialization.InstantiateIMLData(ref OutputData, newOutputData);
            }
            // Make sure we are instantiating a new copy, not a reference
            OutputData.Values = new float[newOutputData.Values.Length];
            Array.Copy(newOutputData.Values, OutputData.Values, newOutputData.Values.Length);

        }

    }

}