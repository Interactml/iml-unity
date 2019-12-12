using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class SineWaveIMLBridge : MonoBehaviour
{
    /// <summary>
    /// The sinewave generator to control
    /// </summary>
    [SerializeField]
    private SineWaveExample m_SineWaveGenerator;

    /// <summary>
    /// The IML component from where we are getting the outputs
    /// </summary>
    [SerializeField]
    private IMLComponent m_MLComponent;

    [SerializeField, Range (0, 1000f)]
    private float m_IntensitySound;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_MLComponent)
        {
            if (m_SineWaveGenerator)
            {
                m_SineWaveGenerator.canPlay = true;
                // Get the first output and map it to the frequency of the sinewave
                if (m_MLComponent.IMLControllerOutputs.Count > 0)
                {
                    m_SineWaveGenerator.frequency1 = Mathf.Abs( (float)m_MLComponent.IMLControllerOutputs[0][0] ) * m_IntensitySound;
                }
            }

        }
    }
}
