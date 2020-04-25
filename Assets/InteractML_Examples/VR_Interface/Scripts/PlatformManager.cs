using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Will handle which platform the game is running on and will adapt Player controllers (VR/nonVR)
/// </summary>
[DefaultExecutionOrder(-100)]
public class PlatformManager : MonoBehaviour
{
    public bool EnableVR;

    [Header("Standalone Setup"), Tooltip("Will activate components depending on the platform")]
    public GameObject[] StandaloneComponents;

    [Header("VR Setup"), Tooltip("Place here all gameObjects that should be active during VR")]
    public GameObject[] VRComponents;

    // Called before Start
    private void Awake()
    {
        if (XRSettings.enabled != EnableVR)
        {
            // Set XR settings config
            XRSettings.enabled = EnableVR;
        }

#if UNITY_STANDALONE

        // Loop through VR components
        for (int i = 0; i < VRComponents.Length; i++)
        {
            // Activate/deactivate them based on xr setting enabled flafg
            VRComponents[i].SetActive(XRSettings.enabled);
        }

        // Loop through Standalone components
        for (int i = 0; i < StandaloneComponents.Length; i++)
        {
            // The standalone components will online be active if the xr settings are disabled
            StandaloneComponents[i].SetActive(!XRSettings.enabled);
        }

#endif
    }

}
