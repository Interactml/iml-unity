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

        /// <summary>
        /// Returns an instance of an IMLDataType based on the type passed in (value can be passed to set the instance value)
        /// </summary>
        /// <param name="type">i.e. float, int, Vector3...</param>
        public static IMLBaseDataType GetDataTypeInstance (System.Type type, object value = null)
        {
            if (type == typeof(float))
            {
                var result = new IMLFloat();
                if (value != null)
                {
                     if (value is float) result.SetValues((float)value);
                     else if (value is float[]) result.SetValues((float[])value);
                }
                return result;
            }
            else if (type == typeof(int))
            {
                var result = new IMLInteger();
                if (value != null)
                {
                    if (value is float) result.SetValues((int)value);
                    else if (value is float[]) result.SetValues((float[])value);
                }

                return result;
            }
            else if (type == typeof(Vector2))
            {
                var result = new IMLVector2();
                if (value != null)
                {
                    if (value is float) result.SetValues((Vector2)value);
                    else if (value is float[]) result.SetValues((float[])value);
                }

                return result;
            }
            else if (type == typeof(Vector3))
            {
                var result = new IMLVector3();
                if (value != null)
                {
                    if (value is float) result.SetValues((Vector3)value);
                    else if (value is float[]) result.SetValues((float[])value);
                }

                return result;
            }
            else if (type == typeof(Vector4))
            {
                var result = new IMLVector4();
                if (value != null)
                {
                    if (value is float) result.SetValues((Vector4)value);
                    else if (value is float[]) result.SetValues((float[])value);
                }

                return result;
            }
            else if (type == typeof(bool))
            {
                var result = new IMLBoolean();
                if (value != null)
                {
                    if (value is float) result.SetValues((bool)value);
                    else if (value is float[]) result.SetValues((float[])value);
                }

                return result;
            }
            else if (type == typeof(float[]))
            {
                var result = new IMLArray();
                if (value != null)
                {
                    if (value is float[]) result.SetValues((float[])value);
                }

                return result;
            }
            else
                return null; // not implement datatype is null
        }
    }

}
