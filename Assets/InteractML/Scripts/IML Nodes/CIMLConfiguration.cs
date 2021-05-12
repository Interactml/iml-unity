﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Linq;
using ReusableMethods;
using System;

namespace InteractML
{
    [NodeWidth(300)]
    [CreateNodeMenuAttribute("Interact ML/Machine Learning System/MLS Classification")]
    public class CIMLConfiguration: CRIMLConfiguration
    {

        #region Variables


        #endregion

        #region XNode Messages
        // Override Init to set learning type as classification
        protected override void Init()
        {
            SetLearningType();
            tooltips = IMLTooltipsSerialization.LoadTooltip("Classification_MLS");
            base.Init();
        }

        #endregion

        #region Unity Messages


        #endregion

        #region Public Methods

        /// <summary>
        /// Instantiates a rapidlibmodel
        /// </summary>
        /// <param name="learningType"></param>
        public override RapidlibModel InstantiateRapidlibModel(IMLSpecifications.LearningType learningType)
        {
            SetLearningType();
            return base.InstantiateRapidlibModel(learningType);
        }

        /// <summary>
        /// Loads the current model from disk (dataPath specified in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName"></param>
        public override bool LoadModelFromDisk(bool reCreateModel = false)
        {
            bool success = false;
            
            // Make sure to re-instantiate the model if null or flag is true
            if (m_Model == null || reCreateModel)
                m_Model = InstantiateRapidlibModel(LearningType);

            success = m_Model.LoadModelFromDisk(this.graph.name + "_IMLConfiguration" + this.id, reCreateModel);
            // We update the node learning type to match the one from the loaded model
            m_LearningType = IMLSpecifications.LearningType.Classification;
            // Configure inputs and outputs
            PredictedOutput = new List<IMLBaseDataType>(m_Model.GetNumExpectedOutputs());
            // TO DO
            // Still left to configure inputs
            // Still left to configure the type of the inputs and outputs

            return success;
        }

        #endregion

        #region Protected Methods
        protected override void SetLearningType()
        {
            m_LearningType = IMLSpecifications.LearningType.Classification;
            learningChoice = CR_LearningChoice.Classification; ;
        }
        protected override void OverrideModel(IMLSpecifications.LearningType learningType)
        {
            m_Model = new RapidlibModel(RapidlibModel.ModelType.kNN);
        }

        /// <summary>
        /// Override training Examples to only check for the single training examples type
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="portName"></param>
        protected override void CheckTrainingExamplesConnections(NodePort from, NodePort to, string portName)
        {
            // Evaluate the nodeport for training examples
            if (to.fieldName == portName)
            {
                // Check if the node connected was a training examples node
                bool isNotTrainingExamplesNode = this.DisconnectIfNotType<CIMLConfiguration, SingleTrainingExamplesNode>(from, to);

                // If we broke the connection...
                if (isNotTrainingExamplesNode)
                {
                    // Prepare flag to show error regarding training examples
                    m_ErrorWrongInputTrainingExamplesPort = true;
                }
                // If we accept the connection...
                else
                {
                    SingleTrainingExamplesNode examplesNode = from.node as SingleTrainingExamplesNode;
                    if(examplesNode.TargetValues.Count > 1)
                    {
                        from.Disconnect(to);
                        m_WrongNumberOfTargetValues = true; 
                    }
                    // We check that the connection is from a training examples node
                    if (examplesNode != null && !m_WrongNumberOfTargetValues)
                    {
                        // Update dynamic ports for output
                        AddDynamicOutputPorts(examplesNode, ref m_DynamicOutputPorts);
                    }

                }

            }

        }

        #endregion

    }
}
