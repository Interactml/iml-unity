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
        

        #endregion

    }
}