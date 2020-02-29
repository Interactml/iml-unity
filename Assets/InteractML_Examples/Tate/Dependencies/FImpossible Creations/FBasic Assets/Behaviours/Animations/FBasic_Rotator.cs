using UnityEngine;

namespace FIMSpace.Basics
{
    /// <summary>
    /// Basic script to rotate transform in choosed axis
    /// </summary>
    public class FBasic_Rotator : MonoBehaviour
    {
        public Vector3 RotationAxis = new Vector3(0f, 1f, 0f);

        /// <summary> Multiplies deltaTime </summary>
        public float RotationSpeed = 100f;

        /// <summary> If animator should go on for example during game pause (useful for UI) </summary>
        public bool UnscaledDeltaTime = false;

        protected virtual void Update()
        {
            float delta;
            if (UnscaledDeltaTime) delta = Time.unscaledDeltaTime; else delta = Time.deltaTime;

            transform.localRotation *= Quaternion.AngleAxis(delta * RotationSpeed, RotationAxis);
        }
    }
}