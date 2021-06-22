using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
    /// <summary>
    /// Used for storing the on a given time all the position, rotation, scale of an object given a transform.
    /// </summary>
    [System.Serializable]
    public struct DeltaTransform 
    {
        public  Vector3 LocalPosition;
        public  Vector3 LocalEulerAngles;
        public Vector3 Position;
        public Vector3 EulerAngles;
        public Quaternion Rotation;
        public Quaternion LocalRotation;
        public Vector3 lossyScale;
        public Vector3 LocalScale;

        /// <summary>
        /// Store all the position rotations and scale of a transform 
        /// </summary>
        public void StoreTransform(Transform transform)
        {
            if (transform == null) return;

            Position = transform.position;
            LocalPosition = transform.localPosition;
            EulerAngles = transform.eulerAngles;
            Rotation = transform.rotation;
            LocalEulerAngles = transform.localEulerAngles;
            LocalRotation = transform.localRotation;
            lossyScale = transform.lossyScale;
            LocalScale = transform.localScale;
        }

        public void RestoreTransform(Transform transform)
        {
            transform.position = Position;
            transform.rotation  = Rotation;
            transform.localScale = LocalScale;
        }

        public void RestoreLocalTransform(Transform transform)
        {
            transform.localPosition = LocalPosition;
            transform.localRotation = LocalRotation;
            transform.localScale = LocalScale;
        }

    }
}