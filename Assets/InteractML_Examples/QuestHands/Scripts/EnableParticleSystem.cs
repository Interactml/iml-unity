using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class EnableParticleSystem : MonoBehaviour
{
    public ParticleSystem pS;

    [PullFromIMLController]
    public float EmissionRate;

    public float multiplier; 

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        var em = pS.emission;
        em.rateOverTime = EmissionRate * multiplier;
    }
}
