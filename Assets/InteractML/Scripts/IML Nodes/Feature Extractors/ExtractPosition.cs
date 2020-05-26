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

        /// <summary>
        /// Controls whether to use local space or not
        /// </summary>
        public bool LocalSpace = false;

        /// <summary>
        /// Controls whether to use each axis values in output 
        /// </summary>
        public bool x_switch = true;
        public bool y_switch = true;
        public bool z_switch = true;


        /// <summary>
        /// Feature Values extracted (ready to be read by a different node)
        /// </summary>
        [HideInInspector]
        public IMLBaseDataType FeatureValues { get { return m_PositionExtracted; } }

        /// <summary>
        /// The private feature values extracted in a more specific data type
        /// </summary>
        [SerializeField]
        private IMLVector3 m_PositionExtracted;
        private IMLVector3 old_PositionExtracted;
        /// <summary>
        /// Lets external classes see position data
        /// </summary> 

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

        public bool ReceivingData;

        float x, y, z;
        int counter = 0;
        int count = 5;
        // Use this for initialization
        protected override void Init()
        {
            counter = 0;
            count = 5;

            base.Init();

            if (m_PositionExtracted == null)
            {
                m_PositionExtracted = new IMLVector3();

            }

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
        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return UpdateFeature();
        }

        /// <summary>
        /// Updates Feature values
        /// </summary>
        /// <returns></returns>
        public object UpdateFeature()
        {
            //check if receiving data
            if(counter == count)
            {
                counter = 0;
                if ((x == FeatureValues.Values[0]||!x_switch)&& y == FeatureValues.Values[1] && z == FeatureValues.Values[2])
                {
                    ReceivingData = false;
                }
                else
                {
                    ReceivingData = true; 

                }
                x = FeatureValues.Values[0];
                y = FeatureValues.Values[1];
                z = FeatureValues.Values[2];
                
            }

            counter++;
            
            if (m_PositionExtracted == null)
            {
                m_PositionExtracted = new IMLVector3();

            }

            var gameObjRef = GetInputValue<GameObject>("GameObjectDataIn", this.GameObjectDataIn);

            if (gameObjRef == null)
            {
                // If the gameobject is null, we throw an error on the editor console
                Debug.LogWarning("GameObject missing in Extract Position Node!");
            }
            else
            {

                // Set values of our feature extracted
                if (LocalSpace)
                { 
                    m_PositionExtracted.SetValues(gameObjRef.transform.localPosition);
                }
                else
                { 
                    m_PositionExtracted.SetValues(gameObjRef.transform.position);
                }

            }

            if (!x_switch)
                FeatureValues.Values[0] = 0;

            if (!y_switch)
                FeatureValues.Values[1] = 0;

            if (!z_switch)
                FeatureValues.Values[2] = 0;

            return this;

        }
    }
}

