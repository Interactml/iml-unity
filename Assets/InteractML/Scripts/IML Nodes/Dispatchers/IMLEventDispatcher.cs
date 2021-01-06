using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractML
{
    public class IMLEventDispatcher
    {
        
        /// <summary>
        /// Event for training the system
        /// </summary>
        /// <param name="nodeID">id of the model to train</param>
        /// <returns></returns>
        public delegate bool TrainMLS(string nodeID);
        public static TrainMLS TrainMLSCallback;

        /// <summary>
        /// Event for starting to run a model
        /// </summary>
        /// <param name="nodeID">id for the model to run</param>
        /// <returns></returns>
        public delegate bool StartRunningModel(string nodeID);
        public static StartRunningModel ToggleRunCallback;


        /// <summary>
        /// Event for reseting the model
        /// </summary>
        /// <param name="nodeID">id foro model to reset</param>
        public delegate void ResetMLSModel(string nodeID);
        public static ResetMLSModel ResetModelCallback;

        /// <summary>
        /// Event for type / or number of inputs in training examples
        /// </summary>
        public delegate void TrainingInputsConfigChanged();
        public static TrainingInputsConfigChanged InputConfigChangeCallback;

        /// <summary>
        /// Event for type and/ or number of labels 
        /// </summary>
        public delegate void TrainingLabelConfigChanged();
        public static TrainingLabelConfigChanged LabelsConfigChangeCallback;

        /// <summary>
        /// Event for type and/ or number of labels 
        /// </summary>
        public delegate void ModelSetUpChanged();
        public static ModelSetUpChanged ModelSetUpChangeCallback;

        /// <summary>
        /// Event for recording one frame of example
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public delegate bool RecordOneExample(string nodeID);
        public static RecordOneExample RecordOneCallback;

        /// <summary>
        /// Event for when recording data starts
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public delegate bool StartRecord(string nodeID);
        public static StartRecord ToggleRecordCallback;

        // delete traiing examples events
        /// <summary>
        /// Event for deleting the last examples
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public delegate bool DeleteLast(string nodeID);
        public static DeleteLast DeleteLastCallback;

        // delete traiing examples events
        /// <summary>
        /// Event for deleting the last examples
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public delegate bool DeleteAll(string nodeID);
        public static DeleteAll DeleteAllCallback;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public delegate bool LoadTrainingExamples();
        public static LoadTrainingExamples LoadTrainingExamplesCallback;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public delegate bool LoadModels();
        public static LoadModels LoadModelsCallback;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public delegate void RunOnPlay();
        public static RunOnPlay RunOnPlayCallback;
        // Delete node event ???




    }
}