using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

namespace InteractML
{
    public class VRIMLComponent : MonoBehaviour
    {
        private void Reset()
        {
            AddIMLGrab();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void AddIMLGrab()
        {
            IMLGrab  grab = this.GetComponentInChildren<IMLGrab>();
            if(grab != null)
            {
                grab.gameObject.AddComponent<IMLGrabVR>();
            }
            
        }
    }
}

