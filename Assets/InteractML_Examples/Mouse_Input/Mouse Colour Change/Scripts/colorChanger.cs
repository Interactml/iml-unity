using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class colorChanger : MonoBehaviour
{
    /// <summary>
    /// The IML component from where we are getting the outputs
    /// </summary>

    [PullFromIMLGraph]
    public int colour; 
    Renderer renderer;
    Color[] colors;
    float transitionTime = 5f;
    float transitionRate = 0;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>(); // grab the renderer component

        colors = new Color[3]; // We will randomize through this array

        //initialize our array indexes with colors
        colors[0] = Color.green;
        colors[1] = Color.black;
        colors[2] = Color.blue;
    }

    void Update()
    {
        switch (colour)
        {
            case 1:
                renderer.material.SetColor("_Color", colors[1]);
                break;
            case 2:
                renderer.material.SetColor("_Color", colors[2]);
                break;
            case 3:
                renderer.material.SetColor("_Color", colors[0]);
                break;
            default:
                break;
        }
    }
}
