using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;

namespace InteractML.FeatureExtractors
{
    /// <summary>
    /// Feature extractor for positions
    /// </summary>
    [NodeWidth(250)]
    public class ExtractPosition : BaseExtractorNode, IFeatureIML
    {
        /// <summary>
        /// GameObject from which we extract a feature
        /// </summary>
        [Input]
        public GameObject GameObjectDataIn;

        /// <summary>
        /// Node data sent outside of this node onwards
        /// </summary>
        [Output]
        public Node LiveDataOut;
        //private Vector3 m_LiveDataOut;
        
        /// <summary>
        /// Controls whether to use local space or not
        /// </summary>
        public bool LocalSpace = false;


        /// <summary>
        /// Feature Values extracted (ready to be read by a different node)
        /// </summary>
        [HideInInspector]
        public override IMLBaseDataType FeatureValues { get { return m_PositionExtracted; } }

        /// <summary>
        /// The private feature values extracted in a more specific data type
        /// </summary>
        [SerializeField]
        private IMLVector3 m_PositionExtracted;

        /// <summary>
        /// Lets external classes known if they should call UpdateFeature
        /// </summary>
        public bool isExternallyUpdatable { get { return isConnectedToSomething; } }

        /// <summary>
        /// Private logic to know when this node should be updatable
        /// </summary>
        private bool isConnectedToSomething { get { return (Outputs.Count() > 0); } }

        /// <summary>
        /// Was the feature already updated?
        /// </summary>
        public bool isUpdated { get; set; }


        // Use this for initialization
        protected override void Init()
        {
            // Make sure feature extractor value is never null
            if (m_PositionExtracted == null)
                m_PositionExtracted = new IMLVector3();

            PreviousFeatureValues = new IMLVector3();

            tooltips = IMLTooltipsSerialization.LoadTooltip("Position");

            base.Init();
        }


        /// <summary>
        /// Return the correct value of an output port when requested
        /// </summary>
        /// <returns></returns>
        public override object GetValue(NodePort port)
        {
            return UpdateFeature();

            // to change output type to Vector3
            //LiveDataOut = m_LiveDataOut;
            //return LiveDataOut;
        }

        /// <summary>
        /// Updates Feature values
        /// </summary>
        /// <returns></returns>
        public object UpdateFeature()
        {
            // update if node is receiving data
            ReceivingData = FeatureExtractorMethods.IsReceivingData(this);

            var gameObjRef = GetInputValue<GameObject>("GameObjectDataIn", this.GameObjectDataIn);

            if (gameObjRef != null)
            {
                // Set values of our feature extracted
                if (LocalSpace)
                    m_PositionExtracted.SetValues(gameObjRef.transform.localPosition);
                else
                    m_PositionExtracted.SetValues(gameObjRef.transform.position);
            }
            

            var receivedValue = FeatureValues.Values;
            // for each of the feature values 
            for (int i = 0; i < FeatureValues.Values.Length; i++)
            {
                if (!ToggleSwitches[i]) { receivedValue[i] = 0; }
            }
            
            FeatureValues.Values = receivedValue;

            //m_LiveDataOut.x = FeatureValues.Values[0];
            //m_LiveDataOut.y = FeatureValues.Values[1];
            //m_LiveDataOut.z = FeatureValues.Values[2];

            return this;

        }

        // Check that we are only connecting to a GameObject
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            if (from.node.GetType() == this.GetType())
            {
                System.Type[] portTypesAccept = new System.Type[] { };
                System.Type[] nodeTypesAccept = new System.Type[] { typeof(IFeatureIML), typeof(IMLConfiguration) };
                this.DisconnectPortAndNodeIfNONETypes(from, to, portTypesAccept, nodeTypesAccept);
            }
            else
            {
                this.DisconnectIfNotType<BaseExtractorNode, GameObjectNode>(from, to);

            }
        }
    }
}

