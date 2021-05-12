using UnityEngine;
using System.Collections.Generic;
using System;
namespace MalbersAnimations
{
    /// <summary>
    /// Used To change States from a camera to another
    /// </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Camera/FreeLook Camera State")]
    public class FreeLookCameraState : ScriptableObject
    {
        public Vector3 PivotPos;
        public Vector3 CamPos;
        public float CamFOV = 45;

        public FreeLookCameraState()
        {
            this.CamFOV = 45;
            this.PivotPos = new Vector3(0, 1f, 0);
            this.CamPos = new Vector3(0, 0, -4.45f);
        }
    }
}
