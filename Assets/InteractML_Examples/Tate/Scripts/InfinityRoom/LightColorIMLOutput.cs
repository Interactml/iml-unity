using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class LightColorIMLOutput : MonoBehaviour
{

    /// <summary>
    /// The InteractML component with all the models to output
    /// </summary>
    [Header("InteractML Components")]
    public IMLComponent m_InteractMLComponent;

    [Header("IML Controller Realtime Outputs")]
    public List<double[]> p_IMLControllerOutputs;

    [Header("Choose Output Color 1")]
    public Color color1;

    [Header("Choose Output Color 2")]
    public Color color2;

    [Header("Choose Output Color 3")]
    public Color color3;


    // Start is called before the first frame update
    void Start()
    {
        p_IMLControllerOutputs = m_InteractMLComponent.IMLControllerOutputs;
    }


    // Update is called once per frame
    void Update()
    {
        if (m_InteractMLComponent)
        {
            if (p_IMLControllerOutputs.Count > 0)
            {
                Light[] lights = FindObjectsOfType<Light>();
                switch (p_IMLControllerOutputs[0][0])
                {  
                    case 1:
                        
                        foreach (Light l in lights)
                        {
                            l.color = color1;
                        }
                        break;
                    case 2:
                        foreach (Light l in lights)
                        {
                            l.color = color2;
                        }
                        break;
                    case 3:
                        foreach (Light l in lights)
                        {
                            l.color = color3;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
 
}

