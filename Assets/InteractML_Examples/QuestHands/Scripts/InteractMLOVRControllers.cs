using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InteractML;

public class InteractMLOVRControllers : MonoBehaviour
{
    
    [PullFromIMLController]
    public int command;

    int accuracyCommand;
    int accuracy;
    int oldCommand;


    //public OVRInput.RawButton toggleRecord;
    public Text test;
    public IMLComponent IMLSystem;
    public bool controlAllTeachtheMachine;
    public int[] TeachTheMachineToControl;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(command == 0)
        {
            test.text += "waiting";
        }
      
        if (command == 1 && oldCommand != command)
        {
            Debug.Log("record one");
            test.text = "record one";
           /* if (controlAllTeachtheMachine)
            {
                foreach (TrainingExamplesNode tNode in IMLSystem.TrainingExamplesNodesList)
                {
                    tNode.ToggleCollectExamples();
                }
            } else
            {
                for (int i =0; i < TeachTheMachineToControl.Length; i++)
                {
                    IMLSystem.TrainingExamplesNodesList[TeachTheMachineToControl[i]].ToggleCollectExamples();
                }
            }*/
        }
        if (command == 2 && oldCommand != command)
        {
            if (test.text == "start record")
            {
                test.text = "stop record";
            }
            else
            {
                test.text = "start record";
            }
        }
        if (command == 3 && oldCommand != command)
            test.text = "train";
        if (command == 4 && oldCommand != command)
        {
            if (test.text == "start run")
            {
                test.text = "stop run";
            }
            else
            {
                test.text = "start run";
            }
        }
        if(accuracyCommand == command)
        {
            accuracy++;
        }
        if(accuracy > 5)
        {
            oldCommand = command;
        }
        accuracyCommand = command;
    }
}
