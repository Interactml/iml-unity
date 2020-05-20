using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace InteractML
{
    /// <summary>
    /// Simple class that lays down the structure of an Tooltips for each node
    /// </summary>
    [System.Serializable]
    public class IMLNodeTooltips
    {
        /// <summary>
        /// The Value of the data type
        /// </summary>
        /// 
        public string HelpTooltip { get; set; }

        public string[] PortTooltip { get; set; }

        //public DoubleTooltip BodyTooltip { get; set; }

        public string[] BottomError { get; set; }


    }

}
