using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
   public interface IUIInteractable : ILineRenderable
    {
        void UpdateUIModel(ref TrackedDeviceModel model);
        bool TryGetUIModel(out TrackedDeviceModel model);
    }

    public class XRUIInputModule : UIInputModule
    {
        const float k_DefaultMaxTrackedDeviceRaycastDistance = 1000.0f;

        struct RegisteredInteractable
        {
            public RegisteredInteractable(IUIInteractable interactable, int deviceIndex)
            {
                this.interactable = interactable;
                model = new TrackedDeviceModel(deviceIndex);
            }

            public IUIInteractable interactable;
            public TrackedDeviceModel model;
        }

        struct RegisteredTouch
        {
            public RegisteredTouch(Touch touch, int deviceIndex)
            {
                touchId = touch.fingerId;
                model = new TouchModel(deviceIndex);
                isValid = true;
            }

            public bool isValid;
            public int touchId;
            public TouchModel model;

        }

        [SerializeField]
        [Tooltip("The maximum distance to raycast with tracked devices to find hit objects.")]
        float m_MaxTrackedDeviceRaycastDistance = k_DefaultMaxTrackedDeviceRaycastDistance;
        public float maxRaycastDistance { get { return m_MaxTrackedDeviceRaycastDistance;} set { m_MaxTrackedDeviceRaycastDistance = value;} }

        [SerializeField]
        [Tooltip("If true, will forward 3D tracked device data to UI elements")]
        bool m_EnableXRInput = true;
        [SerializeField]
        [Tooltip("If true, will forward 2D mouse data to UI elements")]
        bool m_EnableMouseInput = true;
        [SerializeField]
        [Tooltip("If true, will forward 2D touch data to UI elements.")]
        bool m_EnableTouchInput = true;

        MouseModel m_Mouse;
        List<RegisteredTouch> m_RegisteredTouches;
        int m_RollingInteractableIndex;
        List<RegisteredInteractable> m_RegisteredInteractables;

        void EnsureInitialized()
        {
            if (m_RegisteredInteractables == null)
                m_RegisteredInteractables = new List<RegisteredInteractable>();

            if (m_RegisteredTouches == null)
                m_RegisteredTouches = new List<RegisteredTouch>();
        }

        protected override void Awake()
        {
            base.Awake();
            EnsureInitialized();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Mouse = new MouseModel(m_RollingInteractableIndex++);
        }

        /// <summary>Register an Interactable with the UI system.  Calling this will enable it to start interacting with UI.</summary>
        /// <param name="interactable">The IUIInteractable to use.</param>
        public void RegisterInteractable(IUIInteractable interactable)
        {
            EnsureInitialized();

            for (int i = 0; i < m_RegisteredInteractables.Count; i++)
            {
                if (m_RegisteredInteractables[i].interactable == interactable)
                    return;
            }

            m_RegisteredInteractables.Add(new RegisteredInteractable(interactable, m_RollingInteractableIndex++));
        }

        /// <summary>Unregisters an Interactable with the UI system.  This cancels all UI Interaction and makes the IUIInteractable no longer able to affect UI.</summary>
        /// <param name="interactable">The IUIInteractable to stop using.</param>
        public void UnregisterInteractable(IUIInteractable interactable)
        {
            EnsureInitialized();

            for (int i = 0; i < m_RegisteredInteractables.Count; i++)
            {
                if (m_RegisteredInteractables[i].interactable == interactable)
                {
                    RegisteredInteractable registeredInteractable = m_RegisteredInteractables[i];
                    registeredInteractable.interactable = null;
                    m_RegisteredInteractables[i] = registeredInteractable;
                    return;
                }
            }
        }

        /// <summary>Retrieves the UI Model for a selected Interactable.</summary>
        /// <param name="interactable">The Interactable you want the model for.</param>
        /// <param name="model">The returned model that reflects the UI state of the interactable.</param>
        /// <returns>True if the model was able to retrieved, false otherwise.</returns>
        public bool GetTrackedDeviceModel(IUIInteractable interactable, out TrackedDeviceModel model)
        {
            EnsureInitialized();

            for (int i = 0; i < m_RegisteredInteractables.Count; i++)
            {
                if (m_RegisteredInteractables[i].interactable == interactable)
                {
                    model = m_RegisteredInteractables[i].model;
                    return true;
                }
            }

            model = new TrackedDeviceModel(-1);
            return false;
        }

        protected override void DoProcess()
        {
            EnsureInitialized();

            if (m_EnableXRInput)
            {
                for (int i = 0; i < m_RegisteredInteractables.Count; i++)
                {
                    RegisteredInteractable registeredInteractable = m_RegisteredInteractables[i];

                    //Update the raycast distance in case it's changed between frames
                    registeredInteractable.model.maxRaycastDistance = m_MaxTrackedDeviceRaycastDistance;

                    //If device is removed, we send a default state to unclick any UI
                    if (registeredInteractable.interactable == null)
                    {
                        registeredInteractable.model.Reset(false);
                        registeredInteractable.model.maxRaycastDistance = 0;
                        ProcessTrackedDevice(ref registeredInteractable.model, true);
                        m_RegisteredInteractables.RemoveAt(i--);
                    }
                    else
                    {
                        registeredInteractable.interactable.UpdateUIModel(ref registeredInteractable.model);
                        ProcessTrackedDevice(ref registeredInteractable.model);
                        m_RegisteredInteractables[i] = registeredInteractable;
                    }
                }
            }

            if (m_EnableMouseInput)
                ProcessMouse();

            if (m_EnableTouchInput)
                ProcessTouches();
        }

        void ProcessMouse()
        {
            if(Input.mousePresent)
            {
                m_Mouse.position = Input.mousePosition;
                m_Mouse.scrollPosition = Input.mouseScrollDelta;
                m_Mouse.leftButtonPressed = Input.GetMouseButton(0);
                m_Mouse.rightButtonPressed = Input.GetMouseButton(1);
                m_Mouse.middleButtonPressed = Input.GetMouseButton(2);

                ProcessMouse(ref m_Mouse);
            }
        }

        void ProcessTouches()
        {
            if(Input.touchCount > 0)
            {
                Touch[] touches = Input.touches;
                for (int i = 0; i < touches.Length; i++)
                {
                    Touch touch = touches[i];
                    int registeredTouchIndex = -1;

                    //Find if touch already exists
                    for (int j = 0; j < m_RegisteredTouches.Count; j++)
                    {
                        if (touch.fingerId == m_RegisteredTouches[j].touchId)
                        {
                            registeredTouchIndex = j;
                            break;
                        }
                    }

                    if (registeredTouchIndex < 0)
                    {
                        //Not found, search empty pool
                        for (int j = 0; j < m_RegisteredTouches.Count; j++)
                        {
                            if (!m_RegisteredTouches[j].isValid)
                            {
                                //Re-use the Id
                                int pointerId = m_RegisteredTouches[j].model.pointerId;
                                m_RegisteredTouches[j] = new RegisteredTouch(touch, pointerId);
                                registeredTouchIndex = j;
                                break;
                            }
                        }

                        if (registeredTouchIndex < 0)
                        {
                            //No Empty slots, add one
                            registeredTouchIndex = m_RegisteredTouches.Count;
                            m_RegisteredTouches.Add(new RegisteredTouch(touch, m_RollingInteractableIndex++));
                        }
                    }

                    RegisteredTouch registeredTouch = m_RegisteredTouches[registeredTouchIndex];
                    registeredTouch.model.selectPhase = touch.phase;
                    registeredTouch.model.position = touch.position;
                    m_RegisteredTouches[registeredTouchIndex] = registeredTouch;
                }

                for (int i = 0; i < m_RegisteredTouches.Count; i++)
                {
                    RegisteredTouch registeredTouch = m_RegisteredTouches[i];
                    ProcessTouch(ref registeredTouch.model);
                    if (registeredTouch.model.selectPhase == TouchPhase.Ended || registeredTouch.model.selectPhase == TouchPhase.Canceled)
                        registeredTouch.isValid = false;
                    m_RegisteredTouches[i] = registeredTouch;
                }
            }       
        }
    }
}
