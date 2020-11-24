using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class colorChanger : MonoBehaviour
{
    Renderer renderer;
    Color[] colors;
    float transitionTime = 5f;
    float transitionRate = 0;

    // IML values classification
    [PullFromIMLGraph]
    public int SetAbsoluteColour;

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
