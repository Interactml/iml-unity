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
        public static StartRunningModel StartRunCallback;

        /// <summary>
        /// Event for stopping run the model
        /// </summary>
        /// <param name="nodeID">id for the model to stop runnning</param>
        /// <returns></returns>
        public delegate bool StopRunningModel(string nodeID);
        public static StopRunningModel StopRunCallback;

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
        public static TrainingInputsConfigChanged InputConfigChange;

        /// <summary>
        /// Event for type and/ or number of labels 
        /// </summary>
        public delegate void TrainingLabelConfigChanged();
        public static TrainingLabelConfigChanged LabelsConfigChange;

        /// <summary>
        /// Event for type and/ or number of labels 
        /// </summary>
        public delegate void ModelSetUpChanged();
        public static ModelSetUpChanged SetUpChange;

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
        public static StartRecord RecordStartCallback;

        /// <summary>
        /// Event for when recording data stops 
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public delegate bool StopRecord(string nodeID);
        public static StopRecord RecordStopCallback;

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

        // Delete node event ???



    }
}