using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class DummyRapidlib : MonoBehaviour
{
    RapidlibModel m_Model;

    public List<RapidlibTrainingExample> m_TrainingExamples;

    public RapidlibTrainingExample m_RunningExample;

    public Transform Input;

    double[] m_InputValues;

    public double[] DesiredOutput;

    public double[] OutputFromModel;

    // Start is called before the first frame update
    void Start()
    {
        // Make a new classification model
        m_Model = new RapidlibModel(RapidlibModel.ModelType.kNN);

        // Init lists
        m_TrainingExamples = new List<RapidlibTrainingExample>();
        m_InputValues = new double[3];

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
        }

        // Train model when pressed T
        if (UnityEngine.Input.GetKeyDown(KeyCode.T))
        {
            bool isTrained = m_Model.Train(m_TrainingExamples);
            Debug.Log("Model trained? = " + isTrained);

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
    }
}
