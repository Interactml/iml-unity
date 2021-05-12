using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class ParticleChanger : MonoBehaviour
{
    /// <summary>
    /// The IML component from where we are getting the outputs
    /// </summary>
    [SerializeField]
    public IMLComponent m_MLComponent;

    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();// access prticle system
        var main = ps.main;


        /*colors = new Color[3]; // We will randomize through this array

        //initialize our array indexes with colors
        colors[0] = Color.green;
        colors[1] = Color.black;
        colors[2] = Color.blue;*/
    }

    void Update()
    {
        if (m_MLComponent)
        {
            if (m_MLComponent.IMLControllerOutputs.Count > 0)
            {
                switch (m_MLComponent.IMLControllerOutputs[0][0])
                {
                    case 1:
                        var main = ps.main;
                        main.gravityModifier = 1;
                        break;
                    case 2:
                        var main2 = ps.main;
                        main2.gravityModifier = 0;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
