using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Linq;
using ReusableMethods;
using System;

namespace InteractML
{
    [CreateNodeMenuAttribute("Interact ML/Machine Learning System/MLS Regression")]
    [NodeWidth(300)]
    public class RegressionMLSystem: MLSystem
    {

        #region Protected Methods
        // to be deleted if new composition works
        /*protected override void SetLearningType()
        {
            m_LearningType = IMLSpecifications.LearningType.Regression;
        }*/
        protected override void SetTrainingType()
        {
            Debug.Log("here regress");
            m_trainingType = IMLSpecifications.TrainingSetType.SingleTrainingExamples;
        }

        public override RapidlibModel InstantiateRapidlibModel()
        {
            Debug.Log("here");
            RapidlibModel model;
            model = new RapidlibModel(RapidlibModel.ModelType.NeuralNetwork);
            return model;
        }
        /// <summary>
        /// Override training Examples to only check for the single training examples type
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="portName"></param>
        protected override void CheckTrainingExamplesConnections(XNode.NodePort from, XNode.NodePort to, string portName)
        {
            // Evaluate the nodeport for training examples
            if (to.fieldName == portName)
            {
                // Check if the node connected was a training examples node
                bool isNotTrainingExamplesNode = this.DisconnectIfNotType<RegressionMLSystem, SingleTrainingExamplesNode>(from, to);

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
                    // We check that the connection is from a training examples node
                    if (examplesNode != null)
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
