using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class LightController : MonoBehaviour
{
    public Light lightToChange;

    [PullFromIMLGraph]
    public float SetLightIntensity;
    [SendToIMLGraph]
    public Vector3 lightPosition;


    // Update is called once per frame
    void Update()
    {
        //Pull data from IML Graph
        if (lightToChange != null)
            lightToChange.intensity = SetLightIntensity;

        // Send data to IML Graph  
        lightPosition = lightToChange.transform.position;
    }
}
