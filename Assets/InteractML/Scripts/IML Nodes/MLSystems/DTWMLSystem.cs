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
    [CreateNodeMenuAttribute("Interact ML/Machine Learning System/MLS DTW")]
    public class DTWMLSystem : MLSystem
    {

        public override void Initialize()
        {
            // Disable async training when initializing
            UseAsync = false;
            base.Initialize();
        }

        public override RapidlibModel InstantiateRapidlibModel()
        {
            RapidlibModel model;
            model = new RapidlibModel(RapidlibModel.ModelType.DTW);
            return model;
        }

        #region Protected Methods
        // used for UI set what the user facing machine learning algorithm is called
        protected override void SetLearningType()
        {
            m_LearningType = IMLSpecifications.LearningType.DTW;
            isKNN = true;
        }

        protected override void SetTrainingType()
        {
            m_trainingType = IMLSpecifications.TrainingSetType.SeriesTrainingExamples;

        }

        /// <summary>
        /// Override training Examples to only check for the single training examples type
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="portName"></param>
       /* protected override void CheckTrainingExamplesConnections(XNode.NodePort from, XNode.NodePort to, string portName)
        {
            // Evaluate the nodeport for training examples
            if (to.fieldName == portName)
            {
                // Check if the node connected was a training examples node
                bool isNotTrainingExamplesNode = this.DisconnectIfNotType<DTWMLSystem, SeriesTrainingExamplesNode>(from, to);

                // If we broke the connection...
                if (isNotTrainingExamplesNode)
                {
                    // Prepare flag to show error regarding training examples
                    m_ErrorWrongInputTrainingExamplesPort = true;
                }
                // If we accept the connection...
                else
                {
                    SeriesTrainingExamplesNode examplesNode = from.node as SeriesTrainingExamplesNode;

                    if(examplesNode.TargetValues.Count > 1)
                    {
                        from.Disconnect(to);
                        m_WrongNumberOfTargetValues = true; 
                    }

                    // We check that the connection is from a training examples node
                    if (examplesNode != null)
                    {
                        // Update dynamic ports for output
                        AddDynamicOutputPorts(examplesNode, ref m_DynamicOutputPorts);
                    }
                }
            }
        }*/


        #endregion

    }
}
