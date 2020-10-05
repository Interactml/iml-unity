using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class OptionsMenuController : MonoBehaviour
{
    [Header("Key Configs")]
    public SteamVR_Action_Boolean grabAction; //Grab Pinch is the trigger, select from inspecter
    public SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any;//which controller
                                                                         // Use this for initialization

    [Header("Menu Object")]
    public GameObject MenuObj;
    public bool ShowMenuOnStart;

    //void OnEnable()
    //{
    //    if (grabAction != null)
    //    {
    //        grabAction.AddOnChangeListener(OnTriggerPressedOrReleased, SteamVR_Input_Sources.Any);

    //    }
    //}

    //private void OnDisable()
    //{
    //    if (grabAction != null)
    //    {
    //        grabAction.RemoveOnChangeListener(OnTriggerPressedOrReleased, inputSource);
    //    }
    //}

    private void Start()
    {
        MenuObj.SetActive(ShowMenuOnStart);
    }

    private void Update()
    {
        if (CheckGrab())
        {
            MenuObj.SetActive(!MenuObj.activeSelf);
        }

        //Quaternion rotation = transform.rotation;
        //rotation.eulerAngles
    }


    //private void OnTriggerPressedOrReleased(ISteamVR_Action_In action_In)
    //{

    //    Debug.Log("Trigger was pressed or released simple");
    //}

    private void OnTriggerPressedOrReleased(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        Debug.Log("Trigger was pressed or released FANCY");
    }

    private bool CheckGrab()
    {
        return grabAction.GetLastStateDown(inputSource);
    }

}
