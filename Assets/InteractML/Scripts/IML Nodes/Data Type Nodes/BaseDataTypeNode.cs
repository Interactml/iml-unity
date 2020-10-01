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

        /// <summary>
        // Data value name
        /// </summary>
        public string ValueName;

        /// <summary>
        // Input data variables
        /// </summary>
        public virtual T In { get { return m_In; } set { m_In = value; } }
        [Input, SerializeField]
        private T m_In;

        /// <summary>
        // Value itself contained in the node
        /// </summary>
        public virtual T Value { get { return m_Value; } set { m_Value = value; } }
        [SerializeField]
        private T m_Value;

        /// <summary>
        // Output data variables
        /// </summary>
        public virtual T Out { get { return m_Out; } set { m_Out = value; } }
        [Output, SerializeField]
        private T m_Out;

        /// <summary>
        /// Lets external classes know if they should call UpdateFeature (always true for a data type)
        /// </summary>
        bool IFeatureIML.isExternallyUpdatable { get { return true; } }

        /// <summary>
        /// Was the feature already updated?
        /// </summary>
        bool IFeatureIML.isUpdated { get ; set; }

        /// <summary>
        /// Is the node currently receiving input data
        /// </summary>
        public bool ReceivingData;

        /// <summary>
        /// Does the node currently have connections in the input port
        /// </summary>
        public bool InputConnected;

        /// <summary>
        /// Variables to count the number of frames the input values stay the same 
        /// </summary>
        public int Counter, Count;

        /// <summary>
        /// Array of booleans, one per each feature value to allow user to toggle each value on/off
        /// </summary>
        public bool[] ToggleSwitches;

        /// <summary>
        // Variable storing previous frame feature values to test incoming data is changing
        /// </summary>
        public IMLBaseDataType PreviousFeatureValues;

        /// <summary>
        // Field to store input value from editable field when nothing is connected to input port
        /// </summary>
        public IMLBaseDataType UserInput { get; set; }

        /// <summary>
        // IMLFeature variables
        /// </summary>
        public abstract IMLBaseDataType FeatureValues { get; }



        #endregion

        #region XNode Messages

        // Use this for initialization
        protected override void Init()
        {
            // initialise counters to change toggle colour
            Counter = 0;
            Count = 5;

            // create new array of boolean for each of the features in the data type and set all to true
            ToggleSwitches = new bool[FeatureValues.Values.Length];
            for (int i = 0; i < FeatureValues.Values.Length; i++)
                ToggleSwitches[i] = true;

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
