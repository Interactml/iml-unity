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
    [CreateNodeMenuAttribute("Interact ML/Teach The Machine/TTM DTW")]
    [NodeWidth(300)]
    public class SeriesTrainingExamplesNode : TrainingExamplesNode
    {

        #region Variables 

        #endregion

        #region XNode Messages

        #endregion

        #region Unity Messages
        #endregion

        #region Protected Methods


        #endregion

        

        /// <summary>
        /// Sets Data Collection Type 
        /// </summary>
        protected override void SetDataCollection()
        {
            ModeOfCollection = CollectionMode.Series;
        }

        #region Public Methods
        /// <summary>
        /// Save IML Training Data to Disk 
        /// </summary>
        public override void SaveDataToDisk()
        {
            IMLDataSerialization.SaveTrainingSeriesCollectionToDisk(TrainingSeriesCollection, GetJSONFileName());
        }
        

        #endregion

    }
}