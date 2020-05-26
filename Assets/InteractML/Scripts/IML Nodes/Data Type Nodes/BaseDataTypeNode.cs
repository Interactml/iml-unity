using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    public abstract class BaseDataTypeNode<T> : Node, IFeatureIML
    {
        #region Variables

        public string ValueName;

        // Data variables
        // Input
        public virtual T In { get { return m_In; } set { m_In = value; } }
        [Input, SerializeField]
        private T m_In;

        // Value itself contained in the node
        public virtual T Value { get { return m_Value; } set { m_Value = value; } }
        [SerializeField]
        private T m_Value;

        // Output
        public virtual T Out { get { return m_Out; } set { m_Out = value; } }
        [Output, SerializeField]
        private T m_Out;

        // IMLFeature variables
        public abstract IMLBaseDataType FeatureValues { get; }

        /// <summary>
        /// Lets external classes know if they should call UpdateFeature (always true for a data type)
        /// </summary>
        bool IFeatureIML.isExternallyUpdatable { get { return true; } }

        /// <summary>
        /// Was the feature already updated?
        /// </summary>
        bool IFeatureIML.isUpdated { get ; set; }

        #endregion

        #region XNode Messages

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            Out = Value;
            return Out;
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            // Only allow other features and same data type
            bool disconnected = this.DisconnectIfNotType<BaseDataTypeNode<T>, IFeatureIML>(from, to);
            // If it is a feature...
            if (!disconnected)
            {

                // Disconnect if it is a feature data type but not the same data type...
                this.DisconnectIfNotSameDataType(from, to);
            }

        }

        #endregion

        #region IFeature Methods

        /// <summary>
        /// This update fetches the input value connected to this data type node
        /// </summary>
        /// <returns></returns>
        object IFeatureIML.UpdateFeature()
        {
            return this.Update();
        }

        protected virtual object Update()
        {
            // Read input (if it returns default(T) there is no connection )
            var inputReceived = GetInputValue<T>("m_In");
            // Check if we have something connected to the input port
            if (inputReceived != null && !inputReceived.Equals(default(T)))
            {
                // Update the value of this data node
                Value = inputReceived;

            }
            // Return entire node to satisfy IFeatureIML requirements
            return this;
        }

        #endregion

    }
}
