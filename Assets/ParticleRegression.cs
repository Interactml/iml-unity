using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class ParticleRegression : MonoBehaviour
{
    [PullFromIMLGraph]
    public float emissionRate;

    public Color particleColor;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        particleColor.g = emissionRate / 100;
        var particle = this.GetComponent<ParticleSystem>().main;
        particle.startColor = particleColor;
        var emission = this.GetComponent<ParticleSystem>().emission;
        emission.rate = emissionRate;
        //emission.rateOverTime = emissionRate;
    }
}
