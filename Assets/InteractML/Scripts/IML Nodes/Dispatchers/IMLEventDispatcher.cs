﻿using System.Collections;
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
        public delegate bool IMLEvent(string nodeID);
        public static IMLEvent TrainMLSCallback;

        /// <summary>
        /// Event for toggle run model
        /// </summary>
        /// <param name="nodeID">id for the model to run</param>
        /// <returns></returns>
        public static IMLEvent ToggleRunCallback;
        
        /// <summary>
        /// Event for starting to run a model
        /// </summary>
        /// <param name="nodeID">id for the model to run</param>
        /// <returns></returns>
        public static IMLEvent StartRunCallback;
        
        /// <summary>
        /// Event for stopping to run a model
        /// </summary>
        /// <param name="nodeID">id for the model to run</param>
        /// <returns></returns>
        public static IMLEvent StopRunCallback;


        /// <summary>
        /// Event for reseting the model
        /// </summary>
        /// <param name="nodeID">id foro model to reset</param>
        public static IMLEvent ResetModelCallback;

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
        public static IMLEvent RecordOneCallback;

        /// <summary>
        /// Event for when recording data starts
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public static IMLEvent ToggleRecordCallback;
        /// <summary>
        /// Event for when recording data starts
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public static IMLEvent StartRecordCallback;
        
        /// <summary>
        /// Event for when recording data starts
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public static IMLEvent StopRecordCallback; 
        
        /// <summary>
        /// Event for when recording data starts
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public delegate void TrainingExamplesUpdated(string[] nodeID);
        public static TrainingExamplesUpdated TrainingExamplesUpdatedCallback;

        // delete traiing examples events
        /// <summary>
        /// Event for deleting the last examples
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public static IMLEvent DeleteLastCallback;

        // delete traiing examples events
        /// <summary>
        /// Event for deleting the last examples
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public static IMLEvent DeleteAllCallback;

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