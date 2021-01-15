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
        // used for UI set what the user facing machine learning algorithm is called
        protected override void SetLearningType()
        {
            // set learning type
            m_LearningType = IMLSpecifications.LearningType.Classification;
            // bool whether is kNN algorithm
            isKNN = true;
        }

        protected override void SetTrainingType()
        {
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
        /*protected override void CheckTrainingExamplesConnections(XNode.NodePort from, XNode.NodePort to, string portName)
        {
            

        }*/

        #endregion
    }
}
