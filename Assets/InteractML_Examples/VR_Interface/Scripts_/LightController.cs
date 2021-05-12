using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class LightController : MonoBehaviour
{
    public Light light;

    [PullFromIMLController]
    public float SetLightIntensity;

    [SendToIMLController]
    public Vector3 LightPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (light != null)
        {
            // Pull data from IML Controller
            light.intensity = SetLightIntensity;

            // Send data to IML Controller
            LightPosition = light.transform.position;   

        }

    }
}
