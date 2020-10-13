using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InteractML;

public class KeyBoardControlIML : MonoBehaviour
{
    public IMLComponent IMLSystem;
    public bool controlAllTeachtheMachine;
    public int[] teachTheMachineToControl;
    public bool controlAllMachineLearning;
    public int[] MachineLearningSystemToControl;
    public KeyCode recordOne;
    public KeyCode toggleRecording;
    public KeyCode train;
    public KeyCode toggleRunning;
    public Text debugText;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(recordOne))
        {
            if (debugText != null)
                debugText.text = "Record One";

            if (controlAllTeachtheMachine)
            {
                foreach (TrainingExamplesNode tNode in IMLSystem.TrainingExamplesNodesList)
                {
                    if(tNode.GetType().ToString() != "InteractML.DTWIMLConfiguration")
                    {
                        tNode.AddSingleTrainingExample();
                    }
                        
                }
            }
            else
            {
                for (int i = 0; i < teachTheMachineToControl.Length; i++)
                {
                    if (IMLSystem.TrainingExamplesNodesList[teachTheMachineToControl[i]].GetType().ToString() != "InteractML.DTWIMLConfiguration")
                    {
                        IMLSystem.TrainingExamplesNodesList[teachTheMachineToControl[i]].AddSingleTrainingExample();
                        Debug.Log("here");
                    }
                        
                }
            }
        }
        if (Input.GetKeyDown(toggleRecording))
        {
            if(debugText != null)
            {
                if (debugText.text == "start record")
                {
                    debugText.text = "stop record";
                }
                else
                {
                    debugText.text = "start record";
                }
            }
            
            if (controlAllTeachtheMachine)
            {
                foreach (TrainingExamplesNode tNode in IMLSystem.TrainingExamplesNodesList)
                {
                    tNode.ToggleCollectExamples();
                }
            }
            else
            {
                for (int i = 0; i < teachTheMachineToControl.Length; i++)
                {
                    IMLSystem.TrainingExamplesNodesList[teachTheMachineToControl[i]].ToggleCollectExamples();
                }
            }
        }
        if (Input.GetKeyDown(train))
        {
            if (debugText != null)
                debugText.text = "train";
            if (controlAllTeachtheMachine)
            {
                foreach (IMLConfiguration mNode in IMLSystem.IMLConfigurationNodesList)
                {
                    mNode.TrainModel();
                }
            }
            else
            {
                for (int i = 0; i < MachineLearningSystemToControl.Length; i++)
                {
                    IMLSystem.IMLConfigurationNodesList[MachineLearningSystemToControl[i]].TrainModel();
                    Debug.Log(MachineLearningSystemToControl[i]);
                }
            }
        }
        if (Input.GetKeyDown(toggleRunning))
        {
            if (debugText != null)
            {
                if (debugText.text == "start running")
                {
                    debugText.text = "stop running";
                }
                else
                {
                    debugText.text = "start running";
                }
            }
            
            if (controlAllMachineLearning)
            {
                foreach (IMLConfiguration mNode in IMLSystem.IMLConfigurationNodesList)
                {
                    mNode.ToggleRunning();
                }
            }
            else
            {
                for (int i = 0; i < MachineLearningSystemToControl.Length; i++)
                {
                    IMLSystem.IMLConfigurationNodesList[MachineLearningSystemToControl[i]].ToggleRunning();
                }
            }
        }

    }
}
