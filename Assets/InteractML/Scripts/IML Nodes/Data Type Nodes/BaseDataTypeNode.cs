using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;

namespace InteractML.DataTypeNodes
{
    public abstract class BaseDataTypeNode<T> : IMLNode, IFeatureIML
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

        public void OnDestroy()
        {
            // Remove reference of this node in the IMLComponent controlling this node (if any)
            var MLController = graph as IMLController;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.DeleteFeatureNode(this);
            }
        }

        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            Out = Value;
            return Out;
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            System.Type[] portTypesAccept = new System.Type[] { typeof(T) };
            System.Type[] nodeTypesAccept = new System.Type[] { this.GetType(), typeof(IFeatureIML), typeof(IMLConfiguration) };
            this.DisconnectPortAndNodeIfNONETypes(from, to, portTypesAccept, nodeTypesAccept);

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
            // Read input (if it returns default(T) there is no connection)
            var inputReceived = GetInputValue<T>("m_In");
            
            // Check if we have something connected to the input port
            // (inputReceived != null && !inputReceived.Equals(default(T)))
            if (inputReceived != null)
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
