using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

/// <summary>
/// Example script to control the colour of a plane with regression
/// </summary>
public class colorChangerRGB : MonoBehaviour
{
    Renderer m_Renderer;

    private Color regressionColor;

    // IML values regression
    [PullFromIMLGraph, Range(0, 1)]
    public Vector3 RGBColour;


    // Start is called before the first frame update
    void Start()
    {
        m_Renderer = GetComponent<Renderer>(); // grab the renderer component

        if (regressionColor == null)
            new Color();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the values from the iml controller
        regressionColor.r = RGBColour.x;
        regressionColor.g = RGBColour.y;
        regressionColor.b = RGBColour.z;

        m_Renderer.material.SetColor("_Color", regressionColor);
        
    }
}
