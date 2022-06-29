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
    Renderer rendererToChange;

    // Start is called before the first frame update
    void Start()
    {
        rendererToChange = GetComponent<Renderer>(); // grab the renderer component
    }

    void Update()
    {
        switch (colour)
        {
            case 0:
                if (rendererToChange != null) rendererToChange.material.SetColor("_Color", Color.white);
                break;
            case 1:
                if (rendererToChange != null) rendererToChange.material.SetColor("_Color", Color.green);
                break;
            case 2:
                if (rendererToChange != null) rendererToChange.material.SetColor("_Color", Color.black);
                break;
            case 3:
                if (rendererToChange != null) rendererToChange.material.SetColor("_Color", Color.blue);
                break;
            case 4:
                if (rendererToChange != null) rendererToChange.material.SetColor("_Color", Color.yellow);
                break;
            case 5:
                if (rendererToChange != null) rendererToChange.material.SetColor("_Color", Color.magenta);
                break;
            default:
                break;
        }
    }
}
