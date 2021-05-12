using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

/// <summary>
/// Example script to control the colour of a plane with regression
/// </summary>
public class colorChangerHSV : MonoBehaviour
{
    Renderer m_Renderer;
    private Color regressionColor;
    // IML values regression
    [PullFromIMLGraph]
    public float H, S, V;

    // Start is called before the first frame update
    void Start()
    {
        m_Renderer = GetComponent<Renderer>(); // grab the renderer component
        if (regressionColor == null)
            new Color();
        S = 0.5f;
        V = 0.5f;
    }
    // Update is called once per frame
    void Update()
    {
        H = (H / 360);
        if (H > 1)
        {
            H = 1;
        }
        else if (H < 0)
        {
            H = 0;
        }
        // Get the values from the iml controller
        regressionColor = Color.HSVToRGB(H, S, V);
        m_Renderer.material.SetColor("_Color", regressionColor);
    }
}
