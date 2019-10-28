using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MalbersAnimations.Utilities
{
    /// <summary>
    /// For when someone with LookAt enters it will set this transform as the target
    /// </summary>
    public class LookAtTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger) return;

            LookAt lookAt = other.GetComponentInParent<LookAt>();

            if (!lookAt) return;
            lookAt.Active = true;
            lookAt.Target = transform;
        }

        void OnTriggerExit(Collider other)
        {
            if (other.isTrigger) return;

            LookAt lookAt = other.GetComponentInParent<LookAt>();

            if (!lookAt) return;

            lookAt.Target = null;

        }
    }
}