using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace InteractML {
    public class AxisHandler2D : MonoBehaviour, ISerializationCallbackReceiver
    {
        public enum Axis2d
        {
            None,
            Primary2DAxis,
            Secondary2DAxis
        }
        
        public XRController controller;
        
        public delegate void ValueChange(XRController controller, Vector2 value);
        public event ValueChange OnValueChange;

        public Axis2d axis = Axis2d.None;

        private InputFeatureUsage<Vector2> inputFeature;
        private Vector2 previousValue = Vector2.zero;

        public RadialMenu innerMenu = null;
        public void Update()
        {
            HandleState();
        }
        public void OnAfterDeserialize()
        {
            inputFeature = new InputFeatureUsage<Vector2>();
        }

        public void OnBeforeSerialize()
        {

        }

      
        public void HandleState()
          {
            

              Vector2 value = GetValue(controller);
              if (value != previousValue)
              {
                  previousValue = value;
                  OnValueChange?.Invoke(controller, value);
              }
            GetClick();
          }

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

        private void GetClick()
        {

            if (controller.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out bool click))
            {
                if (click)
                {
                    innerMenu.ActivateHighlightedSection();
                }
            }


            if (controller.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out bool click2))
            {
                if (click2)
                {
                    innerMenu.ActivateHighlightedSection();
                }
            }
        }
    }
}

