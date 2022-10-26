﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace InteractML.VR
{
    /// <summary>
    /// Manages open/close of radial menu (for the moment)
    /// </summary>
    public class RadialMenuManager : MonoBehaviour
    {

        public RadialMenu radialMenu;
        public IMLComponent MLSystem;

        // actions from action-based XR controller
        public InputActionReference ToggleMenuOpen;
        public InputActionReference SelectMenu;
        public InputActionReference JoystickAxis2D;

        [Tooltip("Drag&drop which controller to use")]
        public GameObject ControllerToUse;
        private XRController DeviceXRController;
        private ActionBasedController ActionXRController;
        private bool m_ControllerPresent;

        /// <summary>
        /// If mouse can be used
        /// </summary>
        [SerializeField]
        private bool m_UseMouse;

        private void Awake()
        {
            if (ToggleMenuOpen != null)
            {
                ToggleMenuOpen.action.started += OpenMenu;
            }

            if (ControllerToUse != null)
            {
                // reparent radialMenu under controller
                radialMenu.transform.SetParent(ControllerToUse.transform);
                radialMenu.transform.localPosition = Vector3.zero;
                // Device or action based?
                DeviceXRController = ControllerToUse.GetComponent<XRController>();
                ActionXRController = ControllerToUse.GetComponent<ActionBasedController>();

                m_ControllerPresent = true;
            }

            // Assign a device to control the radialMenu witg
            if (radialMenu != null && (m_ControllerPresent || m_UseMouse))
            {
                var axisHandler2D = radialMenu.GetComponent<AxisHandler2D>();
                if (axisHandler2D != null)
                {
                    // XR Controller actions
                    if (m_ControllerPresent)
                    {
                        // one of these two will be null (because it is either device OR action based)
                        if (DeviceXRController != null)
                            axisHandler2D.deviceXRController = DeviceXRController;
                        else if (ActionXRController != null)
                        {
                            // controller
                            axisHandler2D.actionXRController = ActionXRController;
                            // Actions
                            axisHandler2D.ToggleMenuOpen = ToggleMenuOpen;
                            axisHandler2D.SelectMenu = SelectMenu;
                            axisHandler2D.JoystickAxis2D = JoystickAxis2D;
                        }
                    }
                    // Mouse actions
                    else if (m_UseMouse)
                    {
                        // enable mouse
                        axisHandler2D.UseMouse = true;
                        // Actions
                        axisHandler2D.ToggleMenuOpen = ToggleMenuOpen;
                        axisHandler2D.SelectMenu = SelectMenu;
                        axisHandler2D.JoystickAxis2D = JoystickAxis2D;
                    }
                }

            }


        }

        private void OnDestroy()
        {
            if (ToggleMenuOpen != null)
            {
                ToggleMenuOpen.action.started -= OpenMenu;
            }

        }

        public void OpenMenu(InputAction.CallbackContext context)
        {            
            Debug.Log("TriggerOpen called!");
            if (MLSystem != null)
            {
                Debug.Log("Calling select graph...");
                IMLEventDispatcher.selectGraph?.Invoke(MLSystem);
            }
        }


    }

}
