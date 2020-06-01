using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XNode;

namespace InteractML
{
    /// <summary>
    /// Node that will show information about a script subscribed to an IML Controller (MonoBehaviour)
    /// </summary>
    public class ScriptNode : IMLNode
    {
        #region Variables

        [Input]
        public float testIn;

        [Input]
        public float testIn2;

        [Output]
        public float testOut;


        /// <summary>
        /// The script we are referencing
        /// </summary>
        public MonoBehaviour Script;

        /// <summary>
        /// Dictionary 
        /// </summary>
        private Dictionary<FieldInfo, IMLFieldInfoContainer> m_DataContainersPerFieldInfo;

        #endregion

        #region XNode Messages

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

            // Init data containers dictionary
            if (m_DataContainersPerFieldInfo == null)
                m_DataContainersPerFieldInfo = new Dictionary<FieldInfo, IMLFieldInfoContainer>();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

        #endregion
    }
}
