using UnityEngine;

namespace MalbersAnimations
{

    public class UseTransform : MonoBehaviour
    {

        public enum UpdateMode { Update, LateUpdate, FixedUpdate }

        public Transform Reference;                       
        public UpdateMode updateMode = UpdateMode.LateUpdate;


        // Update is called once per frame
        void Update()
        {
            if (updateMode == UpdateMode.Update) SetTransformReference();      
        }

        void LateUpdate()
        {
            if (updateMode == UpdateMode.LateUpdate) SetTransformReference();       
        }

        void FixedUpdate()
        {
            if (updateMode == UpdateMode.FixedUpdate) SetTransformReference();       
        }

        private void SetTransformReference()
        {
            if (!Reference) return;
            transform.position = Reference.position;
            transform.rotation = Reference.rotation;
        }
    }
}