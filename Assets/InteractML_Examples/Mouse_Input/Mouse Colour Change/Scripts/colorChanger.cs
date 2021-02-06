using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class colorChanger : MonoBehaviour
{
    Renderer m_Renderer;
    Color[] colors;
   // float transitionTime = 5f;
    //float transitionRate = 0;

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
        array1 = new float[3];
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
        colors[0] = Color.green;
        colors[1] = Color.black;
        colors[2] = Color.blue;
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
                /*while (transitionRate < 1) {

                    //this next line is how we change our material color property. We Lerp between the current color and newColor

                    renderer.material.SetColor("_Color", Color.Lerp(renderer.material.color, newColor, Time.deltaTime * transitionRate));

                    transitionRate += Time.deltaTime / transitionTime; // Increment transitionRate over the length of transitionTime

                    yield return null; // wait for a frame then loop again

                }

                yield return null; // wait for a frame then loop again

        } */
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
