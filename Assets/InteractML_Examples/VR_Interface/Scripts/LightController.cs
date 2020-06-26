using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class LightController : MonoBehaviour
{
    public Light light;

    [PullFromIMLController]
    public Vector3 SetLightRotation;

    [PullFromIMLController]
    public float LightIntensity;

    public bool controlLightIntensity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (light != null)
        {
            var rotation = light.transform.rotation;
            rotation.eulerAngles = SetLightRotation;
            light.transform.rotation = rotation;

            if (controlLightIntensity)
                light.intensity = LightIntensity;
        }
    }
}
