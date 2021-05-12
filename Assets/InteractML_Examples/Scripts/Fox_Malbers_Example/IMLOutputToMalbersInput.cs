﻿using System.Collections;
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
    public IMLSpecifications.LearningType m_ModelType;
    [Range(0f, 1f)]
    public float m_ToleranceRegressionCheck;

    /// <summary>
    /// Flag that controls if InteractML is going to be used or not
    /// </summary>
    private bool UseInteractML = true;

    /// <summary>
    /// List of birds to control
    /// </summary>
    public List<MalbersInput> m_BirdsInputScripts;

    [PullFromIMLGraph]
    public int motionController;

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
                    if (motionController == entry.ExpectedIMLValue)
                    {
                        activateInput = true;
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
