using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using OVR;
using InteractML;

/// <summary>
/// Handles behaviour of pushing boxes with telekinesis
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BoxForces : MonoBehaviour
{
    // IML 
    [Header("IML Setup")]
    public IMLComponent IMLSystem;
    
    // Force vars
    public float PushStrength;
    [PullFromIMLGraph]
    public Vector3 PushDirection;
    
    // Memory on when can push
    private bool m_CanPush;
    private bool m_LastKnownCanPush;

    // Is the box being looked at from the player?
    public bool IsBoxInSight;

    // Rigidbody components
    private Rigidbody m_Rigidbody;
    [SerializeField]
    private Material m_OriginalMat;
    [SerializeField]
    private Material m_HighlightMat;

    // Start is called before the first frame update
    void Start()
    {
        // Get reference to rigidbody
        if (m_Rigidbody == null)
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        m_OriginalMat = this.GetComponent<Renderer>().material;

        if (IMLSystem)
            IMLSystem.SubscribeToIMLController(this, controlClones: true);
    }

    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.XR.XRSettings.enabled)
            // Update oculus input system
            OVRInput.Update();

    }

    // Called at a constant rate
    void FixedUpdate()
    {
        // If we are in VR...
        if (UnityEngine.XR.XRSettings.enabled)
        {
            // Update oculus input system
            OVRInput.FixedUpdate();

            // Get any button
            m_CanPush = OVRInput.Get(OVRInput.Button.Any);

        }
        // If we are on standalone...
        else
        {
            m_CanPush = Input.GetMouseButtonDown(0);
        }

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
            Vector3 pushRandom = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 1), Random.Range(-1, 1));
            // Push box 
            m_Rigidbody.AddForce(PushDirection * PushStrength, ForceMode.Impulse);

            Debug.Log("Force applied");

            m_LastKnownCanPush = m_CanPush;

        }
    }

    void LateUpdate()
    {
        // If we enter late update not in sight already, we make sure to not highlight the box
        if (!IsBoxInSight)
        {
            HighlightBox(false);
        }
        // Sight flag is always false at the end of the frame
        IsBoxInSight = false;
    }

    /// <summary>
    /// Highlights or not the box
    /// </summary>
    /// <param name="option"></param>
    public void HighlightBox(bool option)
    {
        if (option)
        {
            // Avoid applying if the material is already applied
            if (this.GetComponent<Renderer>().material != m_HighlightMat)
            {
                this.GetComponent<Renderer>().material = m_HighlightMat;
            }
        }
        else if (!option)
        {
            // Avoid applying if the material is already applied
            if (this.GetComponent<Renderer>().material != m_OriginalMat)
            {
                this.GetComponent<Renderer>().material = m_OriginalMat;
            }
        }
    }
}
