using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class colorChanger : MonoBehaviour
{
    Renderer m_Renderer;
    Color[] colors;

    // IML values classification
    [PullFromIMLGraph]
    public int SetAbsoluteColour;

    // Start is called before the first frame update
    void Start()
    {
        m_Renderer = GetComponent<Renderer>(); // grab the renderer component

        colors = new Color[3]; // We will randomize through this array

        //initialize our array indexes with colors
        colors[0] = Color.green;
        colors[1] = Color.black;
        colors[2] = Color.blue;
    }

    void Update()
    {
        // Depending on the value from IML Controller, we have a different predefined colour
        switch (SetAbsoluteColour)
        {
            case 1:
                m_Renderer.material.SetColor("_Color", colors[1]);
                break;
            case 2:
                m_Renderer.material.SetColor("_Color", colors[2]);
                break;
            case 3:
                m_Renderer.material.SetColor("_Color", colors[0]);
                break;
            default:
                break;
        }
       
    }
}
