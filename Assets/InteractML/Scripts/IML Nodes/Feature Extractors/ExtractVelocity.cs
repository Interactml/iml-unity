﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using XNode;

namespace InteractML.FeatureExtractors
{
    /// <summary>
    /// Extract the velocity from any other Feature
    /// </summary>
    [NodeWidth(250)]
    public class ExtractVelocity : BaseExtractorNode, IFeatureIML
    {
        /// <summary>
        /// The feature that has been previously extracted and from which we are calculating the velocity (i.e. position, rotation, etc)
        /// </summary>
        [Input]
        public Node FeatureToInput;

        /// <summary>
        /// Node data sent outside of this node onwards
        /// </summary>
        [Output]
        public Node VelocityExtracted;

        /// <summary>
        /// Feature Values extracted (ready to be read by a different node)
        /// </summary>
        public IMLBaseDataType FeatureValues { get { return m_VelocityExtracted; } }

        /// <summary>
        /// The private feature values extracted in a more specific data type
        /// </summary>
        [SerializeField]
        private IMLSerialVector m_VelocityExtracted;
        private float[] m_CurrentVelocity;
        /// <summary>
        /// Used to calculate the velocity
        /// </summary>
        [HideInInspector]
        public float[] m_LastFrameFeatureValue;

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

        // check whether incoming data
        public bool ReceivingData = false;

        /// <summary>
        /// Controls whether to use each axis values in output 
        /// </summary>
        public bool x_switch = true;
        public bool y_switch = true;
        public bool z_switch = true;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            tooltips = IMLTooltipsSerialization.LoadTooltip("Velocity");
            // The velocity extractor expects any other feature extracted to make calculations
            FeatureToInput = GetInputValue<Node>("FeatureToInput");
            // If we managed to get the input
            if (FeatureToInput != null)
            {
                // We check that it is an IML Feature
                var featureToUse = (FeatureToInput as IFeatureIML).FeatureValues;
                if (featureToUse != null)
                {
                    // Calculate the velocity arrays size
                    m_CurrentVelocity = new float[featureToUse.Values.Length];
                    m_LastFrameFeatureValue = new float[m_CurrentVelocity.Length];

                }
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
            //Debug.Log("Extracting Velocity...");
            // The velocity extractor expects any other feature extracted to make calculations
            FeatureToInput = GetInputValue<Node>("FeatureToInput");
            // If we managed to get the input
            if (FeatureToInput != null)
            {
                // We check that it is an IML Feature
                var featureToUse = (FeatureToInput as IFeatureIML).FeatureValues;
                if (featureToUse != null)
                {
                    // Calculate the velocity arrays size
                    //m_CurrentVelocity = new float[featureToUse.Values.Length];

                    // If the velocity hasn't been updated yet... (unlocked externally in the IML Component)
                    if (!isUpdated)
                    {
                        ReceivingData = false;
                        // is the GameObject moving 
                        if (m_LastFrameFeatureValue != null)
                        {
                            for (int i = 0; i < m_LastFrameFeatureValue.Length; i++)
                            {
                                if (m_LastFrameFeatureValue[i] != featureToUse.Values[i])
                                {
                                    ReceivingData = true;
                                }
                            }
                        }

                        // We check in case the input feature length changed
                        if (m_CurrentVelocity == null || m_CurrentVelocity.Length != featureToUse.Values.Length)
                        {
                            // If it did, we resize the current vel vector and lastframe vector
                            m_CurrentVelocity = new float[featureToUse.Values.Length];
                            m_LastFrameFeatureValue = null;
                        }

                        if (m_LastFrameFeatureValue == null || m_LastFrameFeatureValue.Length != m_CurrentVelocity.Length)
                        {
                            if (m_CurrentVelocity == null)
                            {
                                Debug.Log("Current Velocity is null");
                            }
                            m_LastFrameFeatureValue = new float[m_CurrentVelocity.Length];
                        }

                        // Calculate velocity itself
                        for (int i = 0; i < m_CurrentVelocity.Length; i++)
                        {
                            if ((i == 0 && !x_switch) || (i == 1 && !y_switch) || (i == 2 && !z_switch))
                            {
                                Debug.Log("here");
                                m_CurrentVelocity[i] = 0;
                            }
                            else
                            {
                                m_CurrentVelocity[i] = (featureToUse.Values[i] - m_LastFrameFeatureValue[i]) / Time.smoothDeltaTime;
                            }


                        }
                        // We make sure that the velocity extracted serial vector is not null
                        if (m_VelocityExtracted == null)
                        {
                            m_VelocityExtracted = new IMLSerialVector(m_CurrentVelocity);
                        }

                        // Set values for velocity extracted and for last frame feature value
                        m_VelocityExtracted.SetValues(m_CurrentVelocity);

                        featureToUse.Values.CopyTo(m_LastFrameFeatureValue, 0);

                        //for (int i = 0; i < m_CurrentVelocity.Length; i++)
                        //{
                        //    //Debug.Log(i + " = " + m_CurrentVelocity[i]);

                        //    // Store last known feature values for next frame
                        //    //m_LastFrameFeatureValue[i] = featureToUse.Values[i];
                        //}

                        // Make sure to mark the feature as updated to avoid calculating twice
                        isUpdated = true;
                    }

                    return this;
                }
                else
                {
                    //// Dispose of arrays to avoid carrying on any configs
                    //m_CurrentVelocity = null;
                    //m_LastFrameFeatureValue = null;

                    return null;
                }
            }
            // If we couldn't get an input, we return null
            else
            {
                //// Dispose of arrays to avoid carrying on any configs
                //m_CurrentVelocity = null;
                //m_LastFrameFeatureValue = null;

                return null;
            }

        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
                System.Type[] portTypesAccept = new System.Type[] { };
                System.Type[] nodeTypesAccept = new System.Type[] { typeof(IFeatureIML), typeof(IMLConfiguration) };
                this.DisconnectPortAndNodeIfNONETypes(from, to, portTypesAccept, nodeTypesAccept);
        }


    }
}
