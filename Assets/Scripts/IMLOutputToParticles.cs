using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReusableMethods;

public class IMLOutputToParticles : MonoBehaviour
{

    /// <summary>
    /// List of Particles to control
    /// </summary>
    public List<ParticleSystem> m_Particles;
    /// <summary>
    /// List of IML Controllers to get the output from
    /// </summary>
    public List<RapidLib> m_IMLControllers;

    [Header("Particles Parameters")]
    public bool ControlNumParticles;
    public bool ControlSimulationSpeedParticles;
    public bool ControlStartSpeedParticles;
    public bool ControlColorParticles;

    private int[] originalNumParticles;
    private float[] originalSimSpeed;
    private float[] originalStartSpeed;
    private ParticleSystem.MinMaxGradient[] originalColor;

    // Start is called before the first frame update
    void Start()
    {
        if (!Lists.IsNullOrEmpty(ref m_Particles))
        {
            // Get the original values of num particles and speed
            originalNumParticles = new int[m_Particles.Count];
            originalSimSpeed = new float[m_Particles.Count];
            originalStartSpeed = new float[m_Particles.Count];
            originalColor = new ParticleSystem.MinMaxGradient[m_Particles.Count];

            for (int i = 0; i < m_Particles.Count; i++)
            {
                originalNumParticles[i] = m_Particles[i].main.maxParticles;
                originalSimSpeed[i] = m_Particles[i].main.simulationSpeed;
                originalStartSpeed[i] = m_Particles[i].main.startSpeedMultiplier;
                originalColor[i] = m_Particles[i].main.startColor;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Go through all the inputs to affect per particle system
        for (int i = 0; i < m_Particles.Count; i++)
        {
            // We go through each of the IML Controllers referenced
            foreach (var IMLController in m_IMLControllers)
            {
                // Get reference to the main component of the particle system
                var particleSystemMain = m_Particles[i].main;
                var particleSystemEmission = m_Particles[i].emission;


                // If we want to control the number of particles to emit
                if (ControlNumParticles)
                {
                    int numParticles = originalNumParticles[i] * ((int)IMLController.outputs[0]) ;
                    particleSystemMain.maxParticles = numParticles;
                    particleSystemEmission.rateOverTimeMultiplier = (int)IMLController.outputs[0];
                }
                else
                {
                    particleSystemMain.maxParticles = originalNumParticles[i];

                }

                // Speed simulation
                if (ControlSimulationSpeedParticles)
                {
                    float speedParticles = (float)IMLController.outputs[0];
                    particleSystemMain.simulationSpeed = speedParticles;                    
                }
                else
                {
                    particleSystemMain.simulationSpeed = originalSimSpeed[i];
                }

                // Start Speed
                if (ControlStartSpeedParticles)
                {
                    float speedParticles = (float)IMLController.outputs[0];
                    particleSystemMain.startSpeed = speedParticles;
                }
                else
                {
                    particleSystemMain.startSpeed = originalStartSpeed[i];
                }

                // Color
                if (ControlColorParticles)
                {
                    Color newColorParticles = new Color();
                    // If the model outputs 3 channels
                    if (IMLController.outputs.Length == 3)
                    {
                        newColorParticles.r = (float)IMLController.outputs[0];
                        newColorParticles.b = (float)IMLController.outputs[1];
                        newColorParticles.g = (float)IMLController.outputs[2];
                        newColorParticles.a = 1f;
                    }
                    // If the model outputs 4 channels
                    else if (IMLController.outputs.Length == 4)
                    {
                        newColorParticles.r = (float)IMLController.outputs[0];
                        newColorParticles.b = (float)IMLController.outputs[1];
                        newColorParticles.g = (float)IMLController.outputs[2];
                        newColorParticles.a = (float)IMLController.outputs[3];

                    }
                    // If the model outputs anything else, we only use the first one based on the initial color
                    else
                    {
                        newColorParticles.r = Mathf.Clamp( (float)IMLController.outputs[0] * originalColor[i].color.r , 0f, 1f);
                        newColorParticles.b = Mathf.Clamp((float)IMLController.outputs[0] * originalColor[i].color.b, 0f, 1f);
                        newColorParticles.g = Mathf.Clamp((float)IMLController.outputs[0] * originalColor[i].color.g, 0f, 1f);
                        newColorParticles.a = 1f;

                    }
                    // Asign it to the particle system
                    var colorParticles = new ParticleSystem.MinMaxGradient(newColorParticles);
                    particleSystemMain.startColor = newColorParticles;
                }
                else
                {
                    particleSystemMain.startColor = originalColor[i];
                }
            }

        }
    }
}
