using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace InteractML {
   /* public class AxisHandler : InputHandler, ISerializationCallbackReceiver
    {
        XRController controller;
        public enum Axis
        {
            None,
            Trigger,
            Grip
        }

        public delegate void ValueChange(XRController controller, float value);
        public event ValueChange OnValueChange;

        public Axis axis = Axis.None;

        private InputFeatureUsage<float> inputFeature;
        private float previousValue = 0.0f;
        public void OnAfterDeserialize()
        {
            inputFeature = new InputFeatureUsage<float>(axis.ToString());
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
            float value = GetValue(controller);
            if (value != previousValue)
            {
                previousValue = value;
                OnValueChange?.Invoke(controller, value);
            }
        }

        public float GetValue(XRController controller)
        {
            if (controller.inputDevice.TryGetFeatureValue(inputFeature, out float value))
                return value;

            return 0.0f;
        }
    }*/
}

