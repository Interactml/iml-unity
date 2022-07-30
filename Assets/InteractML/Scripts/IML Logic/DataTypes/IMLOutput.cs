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

                //switch (newOutputData.DataType)
                //{
                //    case IMLSpecifications.DataTypes.Float:
                //        OutputData = new IMLFloat(newOutputData);
                //        break;
                //    case IMLSpecifications.DataTypes.Integer:
                //        OutputData = new IMLInteger(newOutputData);
                //        break;
                //    case IMLSpecifications.DataTypes.Vector2:
                //        OutputData = new IMLVector2(newOutputData);
                //        break;
                //    case IMLSpecifications.DataTypes.Vector3:
                //        OutputData = new IMLVector3(newOutputData);
                //        break;
                //    case IMLSpecifications.DataTypes.Vector4:
                //        OutputData = new IMLVector4(newOutputData);
                //        break;
                //    case IMLSpecifications.DataTypes.Boolean:
                //        OutputData = new IMLBoolean(newOutputData);
                //        break;
                //    case IMLSpecifications.DataTypes.Array:
                //        OutputData = new IMLArray(newOutputData);
                //        break;
                //    default:
                //        break;
                //}
            }
            // Make sure we are instantiating a new copy, not a reference
            OutputData.Values = new float[newOutputData.Values.Length];
            Array.Copy(newOutputData.Values, OutputData.Values, newOutputData.Values.Length);

        }

    }

}