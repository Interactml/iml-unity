using UnityEngine;
using System.Collections;


namespace MalbersAnimations
{
    /// <summary>
    /// Simple Script to Activate water on the animals
    /// </summary>
    public class EnterWater : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            other.transform.root.SendMessage("EnterWater", true, SendMessageOptions.DontRequireReceiver);
        }


        void OnTriggerExit(Collider other)
        {
            other.transform.root.SendMessage("EnterWater", false, SendMessageOptions.DontRequireReceiver);
        }
    }
}
