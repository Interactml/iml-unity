using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
using InteractML.ControllerCustomisers;

namespace InteractML
{
    public class VRIMLComponent : MonoBehaviour
    {
        public IMLComponent imlSystem;
        public VRControllerInput vrController;

        private void Reset()
        {
            AddIMLGrab();
        }

        private void OnEnable()
        {

#if !UNITY_EDITOR
            Initialize();

#endif
        }
        // Start is called before the first frame update
        void Start()
        {
            
                
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            // Debug.Log("validating");
            // Subscribe to the editor manager so that our update loop gets called

            Initialize();

#endif
        }
        private void Initialize()
        {
            // get reference to IMLComponent 
            imlSystem = this.GetComponent<IMLComponent>();
            if (imlSystem == null)
            {
                Debug.LogError("VRIMLComponent must be used with IMLComponent");
                return;
            }
            SubscribeToDelegates();
        }
       
        private void SubscribeToDelegates()
        {
            imlSystem.m_addDevice += AddVRControllers;
        }
        private void UnsubscribeToDelegates()
        {
            imlSystem.m_addDevice -= AddVRControllers;
        }



        private void AddIMLGrab()
        {
            IMLGrab  grab = this.GetComponentInChildren<IMLGrab>();
            if(grab != null)
            {
                grab.gameObject.AddComponent<IMLGrabVR>();
            }
            
        }
        // subscribed to addDevices - adds VR controllers to the list of input devices in iml component
        private void AddVRControllers()
        {
            Debug.Log("add vr");
            vrController = new VRControllerInput();
            imlSystem.inputTypes.Add(vrController);
            Debug.Log("inputTypes: " + imlSystem.inputTypes.Count);
        }
    }
}

