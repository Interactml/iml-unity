using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InteractML.VR
{
    /// <summary>
    /// Manages open/close of radial menu (for the moment)
    /// </summary>
    public class RadialMenuManager : MonoBehaviour
    {

        public RadialMenu radialMenu;
        public IMLComponent MLSystem;

        public InputActionReference ToggleMenuOpen;

        private void Awake()
        {
            if (ToggleMenuOpen != null)
            {
                ToggleMenuOpen.action.started += OpenMenu;
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
