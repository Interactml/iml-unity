using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
using System;

/// <summary>
/// Dummy class to test that Rapidlib functions are working
/// </summary>
public class DummyRapidlib : MonoBehaviour
{
    RapidlibModel m_Model;
    RapidlibModel m_ModelDTW;

    public List<RapidlibTrainingExample> m_TrainingExamples;
    public List<RapidlibTrainingSerie> m_TrainingSeries;
    private RapidlibTrainingSerie m_TrainingSerie; // Used to add serie to bigger list, clear after added

    public RapidlibTrainingExample m_RunningExample;
    public RapidlibTrainingSerie m_RunningSerie;

    public Transform Input;

    double[] m_InputValues;

    public double[] DesiredOutput;

    public double[] OutputFromModel;

    [Header("DTW Settings")]
    public string DesiredOutputDTW;
    public string OutputFromDTW;

    // Start is called before the first frame update
    void Start()
    {
        // Make a new classification model
        m_Model = new RapidlibModel(RapidlibModel.ModelType.NeuralNetwork);

        // Make a new dtw model
        m_ModelDTW = new RapidlibModel(RapidlibModel.ModelType.DTW);

        // Init lists
        m_TrainingExamples = new List<RapidlibTrainingExample>();
        m_InputValues = new double[3];
        m_TrainingSeries = new List<RapidlibTrainingSerie>();

    }

    // Update is called once per frame
    void Update()
    {
        // Add input to example in memory
        m_InputValues[0] = Input.transform.position.x;
        m_InputValues[1] = Input.transform.position.y;
        m_InputValues[2] = Input.transform.position.z;


        // Collect examples while holding space
        if (UnityEngine.Input.GetKey(KeyCode.Space))
        {
            m_RunningExample.OverrideExample(m_InputValues, DesiredOutput);

            // Add example to list
            m_TrainingExamples.Add(m_RunningExample);

            // Add example to training serie
            m_TrainingSerie.AddTrainingExample(m_RunningExample.Input, DesiredOutputDTW);
        }
        // If space is up, we send serie to training series list
        if (UnityEngine.Input.GetKeyUp(KeyCode.Space))
        {
            m_TrainingSeries.Add(new RapidlibTrainingSerie(m_TrainingSerie));
            // We clear the training serie after it was added
            m_TrainingSerie.ClearSerie();
        }

        // Train model when pressed T
        if (UnityEngine.Input.GetKeyDown(KeyCode.T))
        {
            bool isTrained = m_Model.Train(m_TrainingExamples);
            Debug.Log("Model trained? = " + isTrained);
            bool isTrainedDTW = m_ModelDTW.Train(m_TrainingSeries);
            Debug.Log("Model DTW trained? = " + isTrainedDTW);
        }

        // Run model constantly
        if (m_Model.ModelStatus == IMLSpecifications.ModelStatus.Trained || m_Model.ModelStatus == IMLSpecifications.ModelStatus.Running)
        {
            if (OutputFromModel.Length != DesiredOutput.Length)
            {
                OutputFromModel = new double[DesiredOutput.Length];
            }
            m_Model.Run(m_InputValues, ref OutputFromModel);
            Debug.Log("Output model: " + OutputFromModel[0]);
        }

        // Collect serie for running DTW when mouse is down
        if (UnityEngine.Input.GetKey(KeyCode.Mouse0))
        {
            m_RunningSerie.AddTrainingExample(m_InputValues);
        }
        // Run model DTW when mouse down is being released
        else if (UnityEngine.Input.GetKeyUp(KeyCode.Mouse0))
        {
            bool canRun = false;
            if (m_ModelDTW.ModelStatus == IMLSpecifications.ModelStatus.Running)
            {
                canRun = true;
            }
            else if (m_ModelDTW.ModelStatus == IMLSpecifications.ModelStatus.Trained)
            {
                canRun = true;
            }
            // Only run if model is trained or running
            if (canRun)
            {
                /* DEBUG CODE */
                //IntPtr trainingSetDTW = RapidlibLinkerDLL.CreateTrainingSet();

                //for (int i = 0; i < m_RunningSerie.ExampleSerie.Count; i++)
                //{

                //    RapidlibLinkerDLL.AddTrainingExample(trainingSetDTW,
                //        m_RunningSerie.ExampleSerie[i], m_RunningSerie.ExampleSerie.Count,
                //        m_RunningSerie.ExampleSerie[i], m_RunningSerie.ExampleSerie.Count);
                //}

                //OutputFromDTW = RapidlibLinkerDLL.RunSeriesClassification(m_ModelDTW.ModelAddress, trainingSetDTW).ToString();

                //RapidlibLinkerDLL.DestroyTrainingSet(trainingSetDTW);

                /* END OF DEBUG CODE */

                OutputFromDTW = m_ModelDTW.Run(m_RunningSerie);
            }

            // Clear serie for next run
            m_RunningSerie.ClearSerie();
        }
    }
}
