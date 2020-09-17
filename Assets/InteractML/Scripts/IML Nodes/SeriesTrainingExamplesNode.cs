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

        #region Public Methods


        protected override void Init()
        {
            base.Init();

            Initialize();

            tooltips = IMLTooltipsSerialization.LoadTooltip("SeriesTrainingExamples");
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Sets Data Collection Type 
        /// </summary>
        protected override void SetDataCollection()
        {
            ModeOfCollection = CollectionMode.Series;
        }


        public override void SaveDataToDisk()
        {
            IMLDataSerialization.SaveTrainingSeriesCollectionToDisk(TrainingSeriesCollection, GetJSONFileName());
        }

        public override void LoadDataFromDisk()
        {
            // Load IML Series Collection from disk
            var auxTrainingSeriesCollection = IMLDataSerialization.LoadTrainingSeriesCollectionFromDisk(GetJSONFileName());
            if (!Lists.IsNullOrEmpty(ref auxTrainingSeriesCollection))
            {
                TrainingSeriesCollection = auxTrainingSeriesCollection;
            }
        }

        #endregion

    }
}