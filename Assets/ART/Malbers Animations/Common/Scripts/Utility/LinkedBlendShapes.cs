using UnityEngine;

namespace MalbersAnimations.Utilities
{
    [ExecuteInEditMode]
    public class LinkedBlendShapes : MonoBehaviour
    {
        public SkinnedMeshRenderer master;
        public SkinnedMeshRenderer slave;

        private void Start()
        {
            enabled = false;
        }

        void Update()
        {
            UpdateSlaveBlendShapes();
        }

     public virtual void UpdateSlaveBlendShapes()
        { 
            if (master && slave && slave.sharedMesh)
            {
                for (int i = 0; i < slave.sharedMesh.blendShapeCount; i++)
                {
                    slave.SetBlendShapeWeight(i, master.GetBlendShapeWeight(i));
                }
            }
        }
    }
}