using UnityEngine;
using System.Collections.Generic;

namespace MalbersAnimations.Utilities
{
    [System.Serializable]
    public class MeshBlendShapes
    {
        public string NameID;
        public SkinnedMeshRenderer mesh;
        [Range(0, 100)]
        public float[] blendShapes;     //Value of the Blend Shape

        public bool HasBlendShapes
        {
            get { return mesh && mesh.sharedMesh.blendShapeCount > 0; }
        }
    
        public virtual void UpdateBlendShapes()
        {
            if (mesh != null && blendShapes != null)
            {
                if (NameID == string.Empty) NameID = mesh.name;

                if (blendShapes.Length != mesh.sharedMesh.blendShapeCount)
                {
                    blendShapes = new float[mesh.sharedMesh.blendShapeCount];
                }

                for (int i = 0; i < blendShapes.Length; i++)
                {
                    mesh.SetBlendShapeWeight(i, blendShapes[i]);
                }
            }
        }

        public virtual float[] GetBlendShapeValues()
        {
            if (HasBlendShapes)
            {
                float[] BS = new float[mesh.sharedMesh.blendShapeCount];

                for (int i = 0; i < BS.Length; i++)
                {
                    BS[i] = mesh.GetBlendShapeWeight(i);
                }
                return BS;
            }
            return null;
        }

        public void SetRandom()
        {
            if (HasBlendShapes)
            {
                for (int i = 0; i < blendShapes.Length; i++)
                {
                    blendShapes[i] = Random.Range(0, 100);
                    mesh.SetBlendShapeWeight(i, blendShapes[i]);
                }
            }
        }

        public void SetBlendShape(string name, float value)
        {
            if (HasBlendShapes)
            {
              int index =  mesh.sharedMesh.GetBlendShapeIndex(name);
                if (index != -1)
                {
                    mesh.SetBlendShapeWeight(index, value);
                }
            }
        }

        public void SetBlendShape(int index, float value)
        {
            if (HasBlendShapes)
            {
                mesh.SetBlendShapeWeight(index, value);
            }
        }

    }

    public class BlendShapes : MonoBehaviour
    {
        [SerializeField]
        public List<MeshBlendShapes> Shapes;
        public bool random;

        private void Awake()
        {
            if (random) RandomShapes();
        }

        /// <summary>
        /// Set Random Values to the Mesh Blend Shapes
        /// </summary>
        public virtual void RandomShapes()
        {
            foreach (MeshBlendShapes item in Shapes)
            {
                item.SetRandom();
            }
        }

        public virtual void UpdateBlendShapes()
        {
            foreach (MeshBlendShapes item in Shapes)
            {
                item.UpdateBlendShapes();
            }
        }

        public virtual void SetBlendShape(string name, float value)
        {
            foreach (MeshBlendShapes item in Shapes)
            {
                item.SetBlendShape(name, value);
            }
        }

        public virtual void SetBlendShape(int index, float value)
        {
            foreach (MeshBlendShapes item in Shapes)
            {
                item.SetBlendShape(name, value);
            }
        }


    }
}