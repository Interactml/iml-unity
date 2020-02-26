using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class ParticleSystemIMLOutput : MonoBehaviour
{
    // iml learning type 
    public IMLSpecifications.LearningType m_ModelType;

    [Range(0f, 100f)]
    public float RegressionScaler;

    /// <summary>
    /// The InteractML component with all the models to output
    /// </summary>
    [Header("InteractML Components")]
    public IMLComponent m_InteractMLComponent;

    [Tooltip("Add here all the Particle Systems to effect")]
    public List<ParticleSystem> ParticleSystems;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        // Go through all the particle systems
        foreach (ParticleSystem ps in ParticleSystems)
        {
            int i = 0;
            Debug.Log(m_InteractMLComponent.IMLControllerOutputs.Count);
            // We go through each of the IML outputs referenced in InteractML
            foreach (var IMLOutput in m_InteractMLComponent.IMLControllerOutputs)
            {
                    var em = ps.emission;
                    em.rateOverTime = (float)IMLOutput[i] * RegressionScaler;
                    Debug.Log((float)IMLOutput[i] * RegressionScaler);
            }
        }
    }
}