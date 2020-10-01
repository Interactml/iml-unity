using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Linq;
using ReusableMethods;

namespace InteractML
{
    /// <summary>
    /// Holds the information and list of a training examples node
    /// </summary>
    [CreateNodeMenuAttribute("Interact ML/Teach The Machine/TTM Classification Regression")]
    [NodeWidth(300)]
    public class SingleTrainingExamplesNode : TrainingExamplesNode
    {


        #region Protected Methods
        protected override void Init()
        {
            base.Init();

            Initialize();

            tooltips = IMLTooltipsSerialization.LoadTooltip("SingleTrainingExamples");
        }

        /// <summary>
        /// Sets Data Collection Type 
        /// </summary>
        protected override void SetDataCollection()
        {
            ModeOfCollection = CollectionMode.SingleExample;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Save IML Training Data to Disk 
        /// </summary>
        public override void SaveDataToDisk()
        {
           IMLDataSerialization.SaveTrainingSetToDisk(TrainingExamplesVector, GetJSONFileName());
        }
        /// <summary>
        /// LoadsIML training data from disk 
        /// </summary>
        public override void LoadDataFromDisk()
        {
            //Load training data from disk
            var auxTrainingExamplesVector = IMLDataSerialization.LoadTrainingSetFromDisk(GetJSONFileName());
            if (!Lists.IsNullOrEmpty(ref auxTrainingExamplesVector))
            {
                TrainingExamplesVector = auxTrainingExamplesVector;
                //Debug.Log("Training Examples Vector loaded!");
            }
        }

        #endregion

    }
}