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
        /// Event called after recording data starts (it DOESN'T trigger data collection. Use Toggle record data instead)
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public static IMLEvent StartRecordCallback;

        /// <summary>
        /// Event called after recording data stops (it DOESN'T trigger data collection. Use Toggle record data instead)
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
        /// Event for deleting the training examples from one node
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        public static IMLEvent DeleteAllExamplesInNodeCallback;
        
        
        public delegate bool DeleteAllTrainingExamplesEvent(bool deleteFromDisk);
        /// <summary>
        /// Deletes all the training exemples in the graph
        /// </summary>
        public static DeleteAllTrainingExamplesEvent DeleteAllTrainingExamplesInGraphCallback;

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

        /// <summary>
        /// The model steering iteration (collect data->train->evaluate model) is started. Node passed is which model started the iteration. Empty if started on graph open
        /// </summary>
        public static IMLEvent ModelSteeringIterationStarted;
        /// <summary>
        /// The model steering iteration (collect data->train->evaluate model) is finished. Node passed is which model finished the iteration
        /// </summary>
        public static IMLEvent ModelSteeringIterationFinished; 


        // Delete node event ???

        public delegate void GraphSelection(IMLComponent graph);
        public static GraphSelection selectGraph;
        public static GraphSelection deselectGraph;

        public delegate void EnableRaidal();
        public static EnableRaidal EnableTraining;
        public static EnableRaidal DisableTraining;

        public static IMLEvent SetUniversalTrainingID;
        public static IMLEvent SetUniversalMLSID;
        
        public static IMLEvent UnSetUniversalTrainingID;
        public static IMLEvent UnSetUniversalMLSID;

        public delegate string GetText(string id);
        public static GetText getText;
        public static IMLEvent listenText;

        public delegate void TrainingNodeChange();
        public static TrainingNodeChange tNodeChange;

        public delegate void IMLBoolChange(bool boolean);
        public static IMLBoolChange UniversalControlChange;
        
        public static IMLEvent ActivateUniversalControl;
        public static IMLEvent DisactivateUniversalControl;
        public static RunOnPlay DestroyIMLGrab;

        /// <summary>
        /// Constructor
        /// </summary>
        static IMLEventDispatcher()
        {
        }

        /// <summary>
        /// Clears all callbacks
        /// </summary>
        public static void ClearAllCallbacks()
        {
            Debug.Log("Clear All Calbakcs called!");
            TrainMLSCallback = null;
            ToggleRunCallback = null;
            StartRunCallback = null;
            StopRunCallback = null;
            ResetModelCallback = null;
            
            // Training Inputs Config Changed
            InputConfigChangeCallback = null;
            LabelsConfigChangeCallback = null;
            // Model Setup changed
            ModelSetUpChangeCallback = null;

            RecordOneCallback = null;
            ToggleRecordCallback = null;
            StartRecordCallback = null;
            StopRecordCallback = null;

            // Training Examples Updated
            TrainingExamplesUpdatedCallback = null;

            DeleteLastCallback = null;
            DeleteAllExamplesInNodeCallback = null;

            // Delete All Training Examples
            DeleteAllTrainingExamplesInGraphCallback = null;
            // Load all training examples
            LoadTrainingExamplesCallback = null;
            // Load models
            LoadModelsCallback = null;
            // Run on play
            RunOnPlayCallback = null;

            // Graph Selection
            selectGraph = null;
            deselectGraph = null;

            // Enable Radial
            EnableTraining = null;
            DisableTraining = null;

            SetUniversalTrainingID = null;
            SetUniversalMLSID = null;

            UnSetUniversalTrainingID = null;
            UnSetUniversalMLSID = null;

            // Get text
            getText = null;

            listenText = null;

            // TrainingNodeChange
            tNodeChange = null;
            // IMLBoolChange
            UniversalControlChange = null;

            ActivateUniversalControl = null;
            DisactivateUniversalControl = null;

            //RunOnPlay 
            DestroyIMLGrab = null;

            // Start of model steering iteration
            ModelSteeringIterationStarted = null;
            // End of model steering iteration
            ModelSteeringIterationFinished = null;
    }
}
}