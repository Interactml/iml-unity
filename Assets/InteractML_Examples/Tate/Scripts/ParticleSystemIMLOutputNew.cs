using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public enum PSSetting {emissionRateSet, burstRateSet};

public class ParticleSystemIMLOutputNew : MonoBehaviour
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

    [Header("IML Controller Realtime Outputs")]
    public List<double[]> p_IMLControllerOutputs;

    [Header("Particle Systems")]
    [Tooltip("Add here all the Particle Systems to effect")]
    public List<ParticleSystem> ParticleSystems;

    [Tooltip("Add here all the parameters you want to effect")]
    public bool psRateOverTime;
    public bool psSimulationSpeed;

    public bool effectAll; 


    // Start is called before the first frame update
    void Start()
    {
        p_IMLControllerOutputs = m_InteractMLComponent.IMLControllerOutputs;
    }

    void Init()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (effectAll)
		{
            EffectAllPS();
		} else
		{
            EffectEachPS();
		}
    }

    void EffectAllPS()
	{
        // Go through all the particle systems
        foreach (ParticleSystem ps in ParticleSystems)
        {
            int i = 0;
            // We go through each of the IML outputs referenced in InteractML
            foreach (var IMLOutput in m_InteractMLComponent.IMLControllerOutputs)
            {
                Debug.Log("here");
                if (IMLOutput.Length > 0)
                {
                    Debug.Log(ps.name);
                    var em = ps.emission;
                    em.rateOverTime = (float)IMLOutput[i] * RegressionScaler;
                }

            }
        }
    }

    void EffectEachPS()
	{
        for (int i = 0; i < ParticleSystems.Count; i++)
		{
            if (m_InteractMLComponent.IMLControllerOutputs.Count > i)
			{
                ParticleSystem ps = ParticleSystems[i];
                if (psSimulationSpeed)
                {
                    var em = ps.emission;
                    em.rateOverTime = (float)m_InteractMLComponent.IMLControllerOutputs[i][0] * RegressionScaler;
                }

                if (psRateOverTime) { 

                    var main = ps.main;
                    main.simulationSpeed = (float)m_InteractMLComponent.IMLControllerOutputs[i][0] * RegressionScaler;
                }
               
            } else
			{
                Debug.Log("less particle systems then outputs");
			}

		}
	}
}