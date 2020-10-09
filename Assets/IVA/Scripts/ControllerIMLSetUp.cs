using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerIMLSetUp : MonoBehaviour
{
    public SteamVR_Input_Sources handType; // 1
    public SteamVR_Action_Boolean teleportAction; // 2
    public SteamVR_Action_Boolean grabAction;
    // Start is called before the first frame update
    void Start()
    {
        //SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(grabAction.onStateDown, SteamVR_Input_Sources.Any);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
