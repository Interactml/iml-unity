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

    }

}
