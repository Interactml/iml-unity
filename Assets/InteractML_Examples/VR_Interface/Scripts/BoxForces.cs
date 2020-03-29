using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using OVR;

/// <summary>
/// Handles behaviour of pushing boxes with telekinesis
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BoxForces : MonoBehaviour
{
    // Force vars
    public float PushStrength;
    public Vector3 PushDirection;
    
    // Memory on when can push
    private bool m_CanPush;
    private bool m_LastKnownCanPush;

    // Is the box being looked at from the player?
    public bool IsBoxInSight;

    // Rigidbody components
    private Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        // Get reference to rigidbody
        if (m_Rigidbody == null)
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }   
    }

    // Update is called once per frame
    void Update()
    {
        // Update oculus input system
        OVRInput.Update();

    }

    // Called at a constant rate
    void FixedUpdate()
    {
        // Update oculus input system
        OVRInput.FixedUpdate();

        // Get any button
        m_CanPush = OVRInput.Get(OVRInput.Button.Any);

        // Emulate GetDown (only allow one true per frame)
        if (m_LastKnownCanPush && m_CanPush)
        {
            m_CanPush = false;
        }
        else if (m_LastKnownCanPush && !m_CanPush)
        {
            // allow to change state of last known
            m_LastKnownCanPush = false;
        }

        // Trigger activates the push
        if (m_CanPush && IsBoxInSight)
        {
            // Add a bit of noise to the direction
            Vector3 pushRandom = new Vector3(Random.Range(-0.5f, 1), Random.Range(-0.5f, 1), Random.Range(0, 1));
            // Push box 
            m_Rigidbody.AddForce(pushRandom * PushStrength, ForceMode.Impulse);

            Debug.Log("Force applied");

            m_LastKnownCanPush = m_CanPush;

        }
    }

    void LateUpdate()
    {
        // Sight flag is always false at the end of the frame
        IsBoxInSight = false;
    }
}
