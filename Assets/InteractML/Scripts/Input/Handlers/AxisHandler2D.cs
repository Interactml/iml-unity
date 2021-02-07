using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace InteractML {
    /*public class AxisHandler2D : InputHandler, ISerializationCallbackReceiver
    {
        public enum Axis2d
        {
            None,
            Primary2DAxis,
            Secondary2DAxis
        }
        XRController controller;
        public delegate void ValueChange(XRController controller, Vector2 value);
        public event ValueChange OnValueChange;

        public Axis2d axis = Axis2d.None;

        private InputFeatureUsage<Vector2> inputFeature;
        private Vector2 previousValue = Vector2.zero;
        public void OnAfterDeserialize()
        {
            inputFeature = new InputFeatureUsage<Vector2>();
        }

        public void OnBeforeSerialize()
        {

        }

        public override void SetButton()
        {
            throw new System.NotImplementedException();
        }


        public override void HandleState()
          {
            
              Vector2 value = GetValue(controller);
              if (value != previousValue)
              {
                  previousValue = value;
                  OnValueChange?.Invoke(controller, value);
              }
          }

        public Vector2 GetValue(XRController controller)
        {
            if (controller.inputDevice.TryGetFeatureValue(inputFeature, out Vector2 value))
                return value;

            return Vector2.zero;
        }
    }*/
}

