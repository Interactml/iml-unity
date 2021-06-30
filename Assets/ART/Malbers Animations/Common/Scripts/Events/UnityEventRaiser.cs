using UnityEngine;
using UnityEngine.Events;
namespace MalbersAnimations.Events
{
    /// <summary>
    /// Simple Event Raiser On Enable
    /// </summary>
    public class UnityEventRaiser : MonoBehaviour
    {
        public UnityEvent OnEnableEvent;

        public void OnEnable() { OnEnableEvent.Invoke(); }
    }
}