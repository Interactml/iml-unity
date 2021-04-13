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

    // IML values classification
    [SendToIMLGraph]
    public float[] array1;

    // IML values classification
    [SendToIMLGraph]
    public float[] array2;

    int count;

    // Start is called before the first frame update
    void Start()
    {
        count = 100;
        array1 = new float[34];
        array1[0] = 100;
        array1[1] = 100;
        array1[2] = 100;

        array2 = new float[5];
        array2[0] = 100;
        array2[1] = 100;
        array2[2] = 100;
        array2[3] = 100;
        array2[4] = 100;

        m_Renderer = GetComponent<Renderer>(); // grab the renderer component

        colors = new Color[3]; // We will randomize through this array

        //initialize our array indexes with colors
        colors[0] = Color.black;
        colors[1] = Color.blue;
        colors[2] = Color.green;
    }

    void Update()
    {
        array1[0] = count;
        count++;
        if (count > 200)
            count = 100;

        // Depending on the value from IML Controller, we have a different predefined colour
        switch (SetAbsoluteColour)
        {
            case 1:
                m_Renderer.material.SetColor("_Color", colors[0]);
                break;
            case 2:
                m_Renderer.material.SetColor("_Color", colors[1]);
                break;
            case 3:
                m_Renderer.material.SetColor("_Color", colors[2]);
                break;
            default:
                break;
        }
       
    }
}
