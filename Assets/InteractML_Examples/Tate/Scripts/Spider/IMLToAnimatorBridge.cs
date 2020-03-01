using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

/// <summary>
/// Bridges data from an IML Component to an Animator Controller
/// </summary>
public class IMLToAnimatorBridge : MonoBehaviour
{
    // IML Component to pull data from
    [Header("Components To Bridge")]
    public IMLComponent m_MLComponent;
    public Animator m_AnimComponent;
    
    [Header("Animation Vocabulary")]
    public List<IntToStringWord> m_AnimVocab;

    // Start is called before the first frame update
    void Start()
    {
        if (m_AnimVocab == null)
        {
            m_AnimVocab = new List<IntToStringWord>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        BridgeData(m_MLComponent, m_AnimComponent, m_AnimVocab);
    }

    /// <summary>
    /// Bridges data between an IMLComponent and an AnimatorComponent, specified by an animation vocabulary
    /// </summary>
    /// <param name="iMLComponent"></param>
    /// <param name="anim"></param>
    /// <param name="animVocab"></param>
    private void BridgeData(IMLComponent iMLComponent, Animator anim, List<IntToStringWord> animVocab)
    {
        // We don't run method if any of the components to bridge is null
        if ( (iMLComponent == null) || (anim == null) || (animVocab == null) )
        {
            return;
        }

        
        // Go through all the vocabulary
        for (int i = 0; i < animVocab.Count; i++)
        {
            // Check that them amount of iml outputs between bounds
            if (iMLComponent.IMLControllerOutputs.Count >= i+1)
            {
                // Check if the i iml output matches the expected values
                if ((int)iMLComponent.IMLControllerOutputs[i][0] == animVocab[i].ExpectedInteger)
                {
                    // If it does, we trigger the animation
                    anim.SetTrigger(animVocab[i].TranslatedString);
                }
            }
            
        }
    }
}