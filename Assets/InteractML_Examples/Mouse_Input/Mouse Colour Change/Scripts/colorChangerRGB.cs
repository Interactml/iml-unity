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
    [PullFromIMLGraph]
    public float SetR;
    [PullFromIMLGraph]
    public float SetG;
    [PullFromIMLGraph]
    public float SetB;

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
        regressionColor.r = SetR;
        regressionColor.g = SetG;
        regressionColor.b = SetB;

        m_Renderer.material.SetColor("_Color", regressionColor);
        
    }
}
