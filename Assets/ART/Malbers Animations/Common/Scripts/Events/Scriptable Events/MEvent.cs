using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Events
{
    ///<summary>
    /// The list of listeners that this event will notify if it is Invoked. 
    /// Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple
    /// </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Event" , fileName = "New Event Asset")]
    public class MEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<MEventItemListener> eventListeners = new List<MEventItemListener>();
        //private readonly List<MEventListener> eventListeners = new List<MEventListener>();

#if UNITY_EDITOR
        [TextArea(2,10)]
        public string Description;
#endif

        public virtual void Invoke()
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked();
        }

        public virtual void Invoke(float value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);
        }

        public virtual void Invoke(bool value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);
        }

        public virtual void Invoke(string value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);
        }

        public virtual void Invoke(int value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);
        }

        public virtual void Invoke(GameObject value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);
        }

        public virtual void Invoke(Transform value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);
        }

        public virtual void Invoke(Vector3 value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);
        }

        public virtual void RegisterListener(MEventItemListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public virtual void UnregisterListener(MEventItemListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }

        //#region Debuging Tools
        ////This is for Debugin porpuses
        public virtual void DebugLog(string text)
        {
            Debug.Log(text);
        }
        //public virtual void DebugLog(bool value)
        //{
        //    Debug.Log(value);
        //}
        //public virtual void DebugLog(int value)
        //{
        //    Debug.Log(value);
        //}
        //public virtual void DebugLog(float value)
        //{
        //    Debug.Log(value);
        //}
        //public virtual void DebugLog(object value)
        //{
        //    Debug.Log(value);
        //}
        //public virtual void DebugLog(MonoBehaviour value)
        //{
        //    Debug.Log(value.name);
        //}
        //#endregion
    }
}
