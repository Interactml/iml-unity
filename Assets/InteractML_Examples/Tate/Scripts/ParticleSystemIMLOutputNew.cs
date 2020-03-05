using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public enum PSSetting {emissionRateSet, burstRateSet};

public class ParticleSystemIMLOutputNew : MonoBehaviour
{
    [Serializable]
    public struct PSOutput
    {
        public ParticleSystem ps;
        public bool psRateOverTime;
        public bool psSimulationSpeed;
        public bool psAngle;
    }

    [Range(0f, 10f)]
    public float SimulationSpeedScaler;
    [Range(0f, 100f)]
    public float RateTimeScaler;
    [Range(0f, 360f)]
    public float AngleScaler;

    /// <summary>
    /// The InteractML component with all the models to output
    /// </summary>
    [Header("InteractML Components")]
    public IMLComponent m_InteractMLComponent;

    [Header("IML Controller Realtime Outputs")]
    public List<double[]> p_IMLControllerOutputs;

    [Header("Particle Systems")]
    [Tooltip("Add here all the Particle Systems to effect")]
    public List<PSOutput> ParticleSystems;

    

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
        foreach (PSOutput pso in ParticleSystems)
        {
            int i = 0;
            // We go through each of the IML outputs referenced in InteractML
            foreach (var IMLOutput in m_InteractMLComponent.IMLControllerOutputs)
            {
                if (IMLOutput.Length > 0)
                {
                    var em = pso.ps.emission;
                    em.rateOverTime = (float)IMLOutput[i] * RateTimeScaler;
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
                PSOutput pso = ParticleSystems[i];
               if (pso.psSimulationSpeed)
                {
                   var main = pso.ps.main;
                    main.simulationSpeed = (float)m_InteractMLComponent.IMLControllerOutputs[i][0] * SimulationSpeedScaler;
                }

                if (pso.psRateOverTime) {

                    var em = pso.ps.emission;
                    em.rateOverTime = (float)m_InteractMLComponent.IMLControllerOutputs[i][0] * RateTimeScaler;

                }

                if (pso.psAngle)
                {
                    var shape = pso.ps.shape;

                    shape.angle = (float)m_InteractMLComponent.IMLControllerOutputs[i][0] * AngleScaler;

                }

            } else
			{
                Debug.Log("less particle systems then outputs");
			}

		}
	}
}