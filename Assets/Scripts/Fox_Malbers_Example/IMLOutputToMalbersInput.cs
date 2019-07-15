using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations;
using System.Linq;
using ReusableMethods;
using System;
using InteractML;

[Serializable]
public class TranslationBirdsInputIMLOutput
{
    public string BirdInput;
    public float ExpectedIMLValue;
}

/// <summary>
/// Acts as a middle-man between the birds inputs and the IML Models
/// </summary>
public class IMLOutputToMalbersInput : MonoBehaviour
{
    public RapidLib.LearningType m_ModelType;
    [Range(0f, 1f)]
    public float m_ToleranceRegressionCheck;

    /// <summary>
    /// Flag that controls if InteractML is going to be used or not
    /// </summary>
    [Header("IML System Selection")]
    public bool UseInteractML;



    /// <summary>
    /// List of birds to control
    /// </summary>
    public List<MalbersInput> m_BirdsInputScripts;


    /// <summary>
    /// List of IML controllers to get the output from
    /// </summary>
    [Header("RapidLib Controllers")]
    public List<RapidLib> m_RapidLibControllers;

    /// <summary>
    /// The InteractML component with all the models to output
    /// </summary>
    [Header("InteractML Components")]
    public IMLComponent m_InteractMLComponent;

    /// <summary>
    /// List of inputs to affect in the birds framework and which expected IML output they have
    /// </summary>
    [Header("Input/Output Mapping")]
    public List<TranslationBirdsInputIMLOutput> m_BirdsInputIMLOutput;    

    // Start is called before the first frame update
    void Start()
    {
        // Initialise lists
        m_BirdsInputScripts = FindObjectsOfType<MalbersInput>().ToList<MalbersInput>();
    }

    // Update is called once per frame
    void Update()
    {
        // Go through all the birds in list
        foreach (var inputScript in m_BirdsInputScripts)
        {
            // Go through all the inputs to affect per bird
            foreach (var entry in m_BirdsInputIMLOutput)
            {
                bool activateInput = false;

                // Check if we want to use InteractML (Develop2019 demo) or RapidLib (GDC2019 demo)
                if (UseInteractML)
                {
                    // We go through each of the IML outputs referenced in InteractML
                    foreach (var IMLOutput in m_InteractMLComponent.IMLControllerOutputs)
                    {
                        // In classification...
                        if (m_ModelType == RapidLib.LearningType.Classification)
                        {
                            // As soon as one of the outputs matches our expected value, we set the flag to true
                            if (IMLOutput[0] == entry.ExpectedIMLValue)
                            {
                                activateInput = true;
                                break;
                            }

                        }
                        // In regression...
                        else if (m_ModelType == RapidLib.LearningType.Regression)
                        {
                            // As soon as one of the outputs is similar to our expected value, we set the flag to true
                            if (Mathf.Abs((float)(IMLOutput[0] - entry.ExpectedIMLValue)) < m_ToleranceRegressionCheck)
                            {
                                activateInput = true;
                                break;
                            }

                        }
                    }

                }
                else
                {
                    // We go through each of the IML Controllers (RapidLib) referenced
                    foreach (var IMLController in m_RapidLibControllers)
                    {
                        // In classification...
                        if (m_ModelType == RapidLib.LearningType.Classification)
                        {
                            // As soon as one of the outputs matches our expected value, we set the flag to true
                            if (IMLController.outputs[0] == entry.ExpectedIMLValue)
                            {
                                activateInput = true;
                                break;
                            }

                        }
                        // In regression...
                        else if (m_ModelType == RapidLib.LearningType.Regression)
                        {
                            // As soon as one of the outputs is similar to our expected value, we set the flag to true
                            if (Mathf.Abs((float)(IMLController.outputs[0] - entry.ExpectedIMLValue)) < m_ToleranceRegressionCheck)
                            {
                                activateInput = true;
                                break;
                            }

                        }
                    }

                }

                // If it is one of the special cases, we handled them manually
                if (entry.BirdInput == "Idle" && activateInput)
                {
                    inputScript.InjectingAxisExternally = activateInput;
                    inputScript.HorizontalAxis = 0;
                    inputScript.VerticalAxis = 0;
                }
                else if (entry.BirdInput == "Left" && activateInput)
                {
                    inputScript.InjectingAxisExternally = activateInput;
                    inputScript.HorizontalAxis = -1;
                }
                else if (entry.BirdInput == "Right" && activateInput)
                {
                    inputScript.InjectingAxisExternally = activateInput;
                    inputScript.HorizontalAxis = 1;
                }
                else if (entry.BirdInput == "Backward" && activateInput)
                {
                    inputScript.InjectingAxisExternally = activateInput;
                    inputScript.VerticalAxis = -1;
                }
                else if (entry.BirdInput == "Forward" && activateInput)
                {
                    inputScript.InjectingAxisExternally = activateInput;
                    inputScript.VerticalAxis = 1;
                }
                // If it is not one of the special cases, we fall back to the generic solution
                else
                {
                    inputScript.InjectingAxisExternally = false;
                    // We activate the input depending on whether or not the expected IML output matched
                    inputScript.mCharacter.SetInput(entry.BirdInput, activateInput);

                }

            }



        }
    }
}
