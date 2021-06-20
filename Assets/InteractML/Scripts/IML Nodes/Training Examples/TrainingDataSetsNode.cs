using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
using XNode;
using System.Threading.Tasks;


namespace InteractML
{
    /// <summary>
    /// Holds multiple training data sets
    /// </summary>
    [CreateNodeMenuAttribute("Interact ML/DataSets/TrainingDataSets")]
    [NodeWidth(350)]

    public class TrainingDataSetsNode : IMLNode, IFeatureIML
    {
        #region Variables
        /// <summary>
        /// List of Training dataset holding all the training examples
        /// </summary>
        [Output]
        public List<List<IMLTrainingExample>> TrainingDataSets;

        /// <summary>
        /// Path of folder to search in
        /// </summary>
        public string FolderPath;

        /// <summary>
        /// When loading, are we looking only for a specific nodeID in file(s)?
        /// </summary>
        public string SpecificNodeID;

        /// <summary>
        /// Number of training examples lists loaded
        /// </summary>
        public int DataSetSize { get { return m_DataSetSize; } }
        [SerializeField]
        private int m_DataSetSize;

        // Flags for loading
        public bool LoadingStarted { get { return m_LoadingStarted; } }
        [System.NonSerialized]
        private bool m_LoadingStarted;
        public bool LoadingFinished { get { return m_LoadingFinished; } }

        public IMLBaseDataType FeatureValues => default(IMLBaseDataType);

        public bool isExternallyUpdatable => false;

        public bool isUpdated { get => true; set => throw new System.NotImplementedException(); }

        [System.NonSerialized]
        private bool m_LoadingFinished;

        #endregion

        #region xNode Messages
        // Use this for initialization
        protected override void Init()
        {
            base.Init();

            if (TrainingDataSets == null || TrainingDataSets.Count == 0)
                m_DataSetSize = 0;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            // Returns the list of training examples
            return TrainingDataSets;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads several dataset files from a folder
        /// </summary>
        /// <param name="path">path of file to load</param>
        /// <param name="specificID">When loading, are we looking only for a specific nodeID in file(s)?</param>        
        public void LoadDataSets(string path, string specificID = "")
        {
            if (m_LoadingStarted)
            {
                NodeDebug.LogWarning("Can't start loading when there is a loading in progress...", this);
                return;
            }

            if (Directory.Exists(path))
            {
                m_LoadingStarted = true;
                m_LoadingFinished = false;
                // init data set list
                if (TrainingDataSets == null)
                    TrainingDataSets = new List<List<IMLTrainingExample>>();
                else
                    TrainingDataSets.Clear();

                m_DataSetSize = 0;

                Task.Run(async () => 
                {
                    // First, find all the folders
                    // Iterate to upload all files in folder, including subdirectories
                    string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    Debug.Log($"{files.Length + 1} files found. Loading data sets, please wait...");
                    foreach (string file in files)
                    {
                        // If there is a json file, attempt to load
                        if (Path.GetExtension(file) == ".json")
                        {
                            // Are we looking for a specific ID?
                            if (!string.IsNullOrEmpty(specificID))
                            {
                                // skip if the file doesn't contain the ID we want
                                if (!file.Contains(specificID))
                                    continue;
                            }

                            // Load training data set
                            var singleDataSet = await IMLDataSerialization.LoadTrainingSetFromDiskAsync(file, ignoreDefaultLocation: true);

                            // Add to list if not null
                            if (singleDataSet != null && singleDataSet[0].Inputs != null)
                            {
                                TrainingDataSets.Add(singleDataSet);
                                m_DataSetSize++;
                            }
                        }
                    }

                    if (TrainingDataSets.Count == 0)
                    {
                        NodeDebug.LogWarning("Couldn't load folder!", this);

                    }
                    else
                    {
                        m_DataSetSize = TrainingDataSets.Count;
                        m_LoadingFinished = true;
                        m_LoadingStarted = false; // allow to re-load if user wants to
                        Debug.Log($"{TrainingDataSets.Count + 1} Data Sets Loaded!");
                    }


                });

            }
            else
            {
                NodeDebug.LogWarning("The folder doesn't exist!", this);
            }
        }

        public object UpdateFeature()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }

}
