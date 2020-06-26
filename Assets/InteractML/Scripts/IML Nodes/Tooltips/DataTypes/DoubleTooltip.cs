using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace InteractML
{
    /// <summary>
    /// Simple class that lays down the structure of an IML data type (used for inputs/outputs specification)
    /// </summary>
    [System.Serializable]
    public class DoubleTooltip
    {
        /// <summary>
        /// The Value of the data type
        /// </summary>
        public string[] Tips;

        public string[] Error { get; set; }

    }

}
