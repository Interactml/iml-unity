using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class LightController : MonoBehaviour
{
    public Light light;

    [PullFromIMLGraph]
    public float SetLightIntensity;
    [SendToIMLGraph]
    public Vector3 lightPosition;


    // Update is called once per frame
    void Update()
    {
        //Pull data from IML Graph
        if (light != null)
            light.intensity = SetLightIntensity;

        // Send data to IML Graph  
        lightPosition = light.transform.position;
    }
}
