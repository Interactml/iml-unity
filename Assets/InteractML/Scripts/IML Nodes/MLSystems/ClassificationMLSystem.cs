using System.Collections;
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
    public class ClassificationMLSystem: MLSystem
    {


        #region Protected Methods
        // to be deleted if new composition works
        /*
        protected override void SetLearningType()
        {
            m_LearningType = IMLSpecifications.LearningType.Classification;

        }*/

        protected override void SetTrainingType()
        {
            Debug.Log("here class");
            m_trainingType = IMLSpecifications.TrainingSetType.SingleTrainingExamples;
        }

        public override RapidlibModel InstantiateRapidlibModel()
        {
            RapidlibModel model;
            model = new RapidlibModel(RapidlibModel.ModelType.kNN);
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
                bool isNotTrainingExamplesNode = this.DisconnectIfNotType<ClassificationMLSystem, SingleTrainingExamplesNode>(from, to);

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
