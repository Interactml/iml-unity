using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace InteractML
{
    /// <summary>
    /// Simple class that lays down the structure of an IML data type (used for inputs/outputs specification)
    /// </summary>
    [System.Serializable, JsonConverter(typeof(IMLJsonTypeConverter))]
    public abstract class IMLBaseDataType
    {        
        /// <summary>
        /// The Value of the data type
        /// </summary>
        [SerializeField]
        public abstract float[] Values { get; set; }

        public abstract IMLSpecifications.DataTypes DataType { get; }

        /// <summary>
        /// Sets the internal values of the data structure
        /// </summary>
        /// <param name="newValues"></param>
        public abstract void SetValues(float[] newValues);

        /// <summary>
        /// Adds an amount to the internal data structure
        /// </summary>
        /// <param name="amount"></param>
        public virtual IMLBaseDataType Add(IMLBaseDataType amount)
        {
            if (amount != null && amount.Values != null && amount.Values.Length > 0)
            {
                if (amount.DataType == DataType)
                {
                    for (int i = 0; i < amount.Values.Length; i++)
                    {
                        Values[i] += amount.Values[i];                       
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("Wrong type passed in data type operation");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("Null reference in data type operation");
            }
            return this;
        }

        /// <summary>
        /// Substract an amount to the internal data structure
        /// </summary>
        /// <param name="amount"></param>
        public virtual IMLBaseDataType Substract(IMLBaseDataType amount)
        {
            if (amount != null && amount.Values != null && amount.Values.Length > 0)
            {
                if (amount.DataType == DataType)
                {
                    for (int i = 0; i < amount.Values.Length; i++)
                    {
                        Values[i] -= amount.Values[i];
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("Wrong type passed in data type operation");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("Null reference in data type operation");
            }
            return this;
        }

        {
            if (type == typeof(float))
                return new IMLFloat();
            else if (type == typeof(int))
                return new IMLInteger();
            else if (type == typeof(Vector2))
                return new IMLVector2();
            else if (type == typeof(Vector3))
                return new IMLVector3();
            else if (type == typeof(Vector4))
                return new IMLVector4();
            else if (type == typeof(bool))
                return new IMLBoolean();
            else if (type == typeof(float[]))
                return new IMLArray();
            else
                return null; // not implement datatype is null
        }

    }

}
