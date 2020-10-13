using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
using TMPro;


public class ControllerIMLSetUp : MonoBehaviour
{
    public IMLComponent IMLToControl;

    public int teachTheMachineToControl;
    public int MachineLearningSystemToControl;
    public TextMeshProUGUI debugText;

    // Start is called before the first frame update
    void Start()
    {
        
    } 
    void Update()
    { 

    }

    public void ToggleRun()
    {
        
        IMLToControl.IMLConfigurationNodesList[MachineLearningSystemToControl].ToggleRunning();
        if (debugText != null)
        {
            if (IMLToControl.IMLConfigurationNodesList[MachineLearningSystemToControl].Running)
            {
               debugText.text = "running";
            } else
            {
                debugText.text = "not running";
            }
        }
    }
    public void ToggleRecordExamples()
    {

        IMLToControl.TrainingExamplesNodesList[teachTheMachineToControl].ToggleCollectExamples();
        if (debugText != null)
        {
            if (IMLToControl.TrainingExamplesNodesList[teachTheMachineToControl].CollectingData)
            {
                debugText.text = "recording data";
            }
            else
            {
                debugText.text = "not recording";
            }
        }
    }

    public void Train()
    {

        IMLToControl.IMLConfigurationNodesList[MachineLearningSystemToControl].TrainModel();
        if (debugText != null)
        {
            if (IMLToControl.IMLConfigurationNodesList[MachineLearningSystemToControl].Trained)
            {
                debugText.text = "model trained";
            }
            else
            {
                debugText.text = "not trained";
            }
        }
    }
}
