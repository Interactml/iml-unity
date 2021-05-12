using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Events
{
    [System.Serializable]
    public class MEventItemListener
    {
        public MEvent Event;


        [HideInInspector]
        public bool useInt = false, useFloat = false, useVoid = true, useString = false, useBool = false, useGO = false, useTransform = false, useVector = false;

        public UnityEvent Response = new UnityEvent();
        public FloatEvent ResponseFloat = new FloatEvent();
        public IntEvent ResponseInt = new IntEvent();
        public BoolEvent ResponseBool = new BoolEvent();
        public StringEvent ResponseString = new StringEvent();
        public GameObjectEvent ResponseGO = new GameObjectEvent();
        public TransformEvent ResponseTransform = new TransformEvent();
        public Vector3Event ResponseVector = new Vector3Event();

        public virtual void OnEventInvoked() { Response.Invoke(); }
        public virtual void OnEventInvoked(string value) { ResponseString.Invoke(value); }
        public virtual void OnEventInvoked(float value) { ResponseFloat.Invoke(value); }
        public virtual void OnEventInvoked(int value) { ResponseInt.Invoke(value); }
        public virtual void OnEventInvoked(bool value) { ResponseBool.Invoke(value); }
        public virtual void OnEventInvoked(GameObject value) { ResponseGO.Invoke(value); }
        public virtual void OnEventInvoked(Transform value) { ResponseTransform.Invoke(value); }
        public virtual void OnEventInvoked(Vector3 value) { ResponseVector.Invoke(value); }

        public MEventItemListener()
        {
            useVoid = true;
            useInt = useFloat = useString = useBool =  useGO = useTransform = useVector = false;
        }
    }


    ///<summary>
    /// Listener to use with the GameEvents
    /// Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple
    /// </summary>
    public class MEventListener : MonoBehaviour
    {
//#if UNITY_EDITOR
//        [TextArea(3, 20)]
//        public string Description = "";
//#endif
        public List<MEventItemListener> Events = new List<MEventItemListener>();

        private void OnEnable()
        {
            foreach (var item in Events)
            {
                if (item.Event) item.Event.RegisterListener(item);
            }
        }

        private void OnDisable()
        {
            foreach (var item in Events)
            {
                if (item.Event) item.Event.UnregisterListener(item);
            }
        }
    }
}