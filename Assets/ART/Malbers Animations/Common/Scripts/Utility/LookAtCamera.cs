using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations
{
    public class LookAtCamera : MonoBehaviour
    {
        public bool justY = true;

        public Vector3 Offset;

        Transform cam;

        private void Start()
        {
            cam = Camera.main.transform;
        }

        void Update()
        {
            var lookPos = cam.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);

            transform.eulerAngles = (new Vector3(justY ? 0 : rotation.eulerAngles.x, rotation.eulerAngles.y, 0) + Offset);
        }
    }
}