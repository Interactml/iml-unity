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
        /// Returns an instance of an IMLDataType based on the type passed in
        /// </summary>
        /// <param name="type"></param>
        public static IMLBaseDataType GetDataTypeInstance (System.Type type)
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
            else
                return null; // not implement datatype is null

        }

    }

}
