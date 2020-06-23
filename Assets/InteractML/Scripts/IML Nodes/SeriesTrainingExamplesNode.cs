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

        // Use this for initialization


        #endregion

        #region Unity Messages
        #endregion

        #region Public Methods


        protected override void Init()
        {
            base.Init();

            Initialize();

            TrainingTips = IMLTooltipsSerialization.LoadTooltip("SeriesTrainingExamples");
        }

        /// <summary>
        /// Clears all the training examples stored in the node
        /// </summary>
        public override void ClearTrainingExamples()
        {
            // Clear series in node
            TrainingSeriesCollection.Clear();
            //UpdateOutputsList();
            //UpdateTargeValues();
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

        /// <summary>
        /// Sets the collect data flag to false to stop collecting data
        /// </summary>
        protected override void StopCollectingData()
        {
            m_CollectData = false;

            // We add our series to the series collection
            TrainingSeriesCollection.Add(new IMLTrainingSeries(m_SingleSeries));
            m_SingleSeries.ClearSerie();

            // Save data to disk
            SaveDataToDisk();
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