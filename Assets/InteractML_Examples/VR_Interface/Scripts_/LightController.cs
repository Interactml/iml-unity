using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class LightController : MonoBehaviour
{
    public Light m_Light;

    [PullFromIMLGraph]
    public float SetLightIntensity;

    [SendToIMLGraph]
    public Vector3 LightPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (m_Light != null)
        {
            // Pull data from IML Controller
            m_Light.intensity = SetLightIntensity;

            // Send data to IML Controller
            LightPosition = m_Light.transform.position;   

        }

    }
}
