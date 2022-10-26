using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace InteractML.VR
{
    public class AxisHandler2D : MonoBehaviour, ISerializationCallbackReceiver
    {
        public enum Axis2d
        {
            None,
            Primary2DAxis,
            Secondary2DAxis
        }

        // device-based controller
        public XRController deviceXRController;
        // Actions from action-based controller
        public ActionBasedController actionXRController;
        public InputActionReference ToggleMenuOpen;
        public InputActionReference SelectMenu;
        public InputActionReference JoystickAxis2D;
        // Controller mode (device, action)
        public enum XRControllerModeEnum { device, action }
        public XRControllerModeEnum XRControllerMode;

        // Actions from mouse&keyboard allowed
        public bool UseMouse;

        public delegate void ValueChange(XRController controller, Vector2 value);
        public event ValueChange OnValueChange;

        public Axis2d axis = Axis2d.None;

        private InputFeatureUsage<Vector2> inputFeature;
        private Vector2 previousValue = Vector2.zero;
        [HideInInspector]
        public RadialMenu innerMenu;

        void Start()
        {
            // Accommodating either device based xr rig or action based xr rig
            if (deviceXRController == null) deviceXRController = this.GetComponentInParent<XRController>();
            if (actionXRController == null) actionXRController = this.GetComponentInParent<ActionBasedController>();
            if (deviceXRController != null) XRControllerMode = XRControllerModeEnum.device;
            else if (actionXRController != null || UseMouse)
            {
                // Assign actions methods
                if (SelectMenu != null)
                {
                    XRControllerMode = XRControllerModeEnum.action; // still using actions even if we only have a mouse
                    SelectMenu.action.started += PullActionSelect;
                }

                if (JoystickAxis2D != null)
                {
                    JoystickAxis2D.action.performed += PullActionAxis2D;
                }

            }
            // throw error if no xrcontroller found
            if (deviceXRController == null && (actionXRController == null && !UseMouse)) Debug.LogError("XRController or Mouse not found!");

            innerMenu = this.GetComponent<RadialMenu>();           
        }


        public void Update()
        {
            // only device-based
            HandleDeviceState();
        }

        public void OnAfterDeserialize()
        {
            inputFeature = new InputFeatureUsage<Vector2>();
        }

        public void OnBeforeSerialize()
        {

        }

      
        /// <summary>
        /// Pull inputs from device-based xrcontroller
        /// </summary>
        public void HandleDeviceState()
        {
            if (deviceXRController != null)
            {
                Vector2 value = GetValue(deviceXRController);
                if (value != previousValue)
                {
                    previousValue = value;
                    OnValueChange?.Invoke(deviceXRController, value);
                }
                GetClick();
            }
        }

        /// <summary>
        /// Device joystick value (device-based)
        /// </summary>
        public Vector2 GetValue(XRController controller)
        {

            if (controller.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 value))
            {
                innerMenu.SetTouchPosition(value);
                return value;
            }
            
            
            if (controller.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondary2DAxis, out Vector2 value2))
            {
                innerMenu.SetTouchPosition(value);
                return value2;
            }

            return new Vector2(0, 0);
            
            
        }

        /// <summary>
        /// Device click (device-based)
        /// </summary>
        private void GetClick()
        {

            if (deviceXRController.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out bool click))
            {

                if (click)
                {
                    innerMenu.ActivateHighlightedSection();
                }
            }


            if (deviceXRController.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out bool click2))
            {
                if (click2)
                {
                    innerMenu.ActivateHighlightedSection();
                }
            }
        }
        /// <summary>
        /// Equivalent action to joystick value (action-based)        
        /// </summary>
        private void PullActionAxis2D(InputAction.CallbackContext context)
        {
            var axisValue = context.ReadValue<Vector2>();
            if (axisValue != previousValue)
            {
                previousValue = axisValue;
            }
            innerMenu.SetTouchPosition(axisValue);
        }

        /// <summary>
        /// Equivalent action to device click (action-based)
        /// </summary>
        private void PullActionSelect(InputAction.CallbackContext context)
        {
            innerMenu.ActivateHighlightedSection();
        }

    }
}

