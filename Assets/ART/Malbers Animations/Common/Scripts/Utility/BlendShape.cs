using UnityEngine;
using System.Collections.Generic;

namespace MalbersAnimations.Utilities
{
    /// <summary>
    /// Manage the Blend Shapes of a Mesh
    /// </summary>
    public class BlendShape : MonoBehaviour
    {
        public SkinnedMeshRenderer mesh;
        public SkinnedMeshRenderer[] LODs;

        [Range(0, 100)]
        public float[] blendShapes;                    //Value of the Blend Shape

        public bool random;
        public int PinnedShape;
        
        /// <summary>
        /// Does the mesh has Blend Shapes?
        /// </summary>
        public bool HasBlendShapes
        {
            get { return mesh && mesh.sharedMesh.blendShapeCount > 0; }
        }

        /// <summary>
        /// Returns the current Blend Shapes Values
        /// </summary>
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

      

        private void Awake()
        {
            if (random) RandomizeShapes();
        }


        private void Reset()
        {
            mesh = GetComponentInChildren<SkinnedMeshRenderer>();
            if (mesh)
            {
                blendShapes = new float[mesh.sharedMesh.blendShapeCount];

                for (int i = 0; i < blendShapes.Length; i++)
                {
                    blendShapes[i] = mesh.GetBlendShapeWeight(i);
                }
            }
        }

        public virtual void SetShapesCount()
        {
            if (mesh)
            {
                blendShapes = new float[mesh.sharedMesh.blendShapeCount];

                for (int i = 0; i < blendShapes.Length; i++)
                {
                    blendShapes[i] = mesh.GetBlendShapeWeight(i);
                }
            }
        }



        /// <summary>
        /// Set Random Values to the Mesh Blend Shapes
        /// </summary>
        public virtual void RandomizeShapes()
        {
            if (HasBlendShapes)
            {
                for (int i = 0; i < blendShapes.Length; i++)
                {
                    blendShapes[i] = Random.Range(0, 100);
                    mesh.SetBlendShapeWeight(i, blendShapes[i]);
                }

                UpdateLODs();
            }
        }

        public virtual void SetBlendShape(string name, float value)
        {
            if (HasBlendShapes)
            {
                PinnedShape = mesh.sharedMesh.GetBlendShapeIndex(name);
                if (PinnedShape != -1)
                {
                    mesh.SetBlendShapeWeight(PinnedShape, value);
                }
            }
        }

        public virtual void SetBlendShape(int index, float value)
        {
            if (HasBlendShapes)
            {
                mesh.SetBlendShapeWeight(PinnedShape = index, value);
            }
        }

        public virtual void _PinShape(string name)
        {
            PinnedShape = mesh.sharedMesh.GetBlendShapeIndex(name);
        }

        public virtual void _PinShape(int index)
        {
            PinnedShape = index;
        }

        public virtual void _PinnedShapeSetValue(float value)
        {
            if (PinnedShape != -1)
            {
                value = Mathf.Clamp(value, 0, 100);
                blendShapes[PinnedShape] = value;
                mesh.SetBlendShapeWeight(PinnedShape, value);
                UpdateLODs(PinnedShape);
            }
        }


        public virtual void UpdateBlendShapes()
        {
            if (mesh && blendShapes != null)
            {
                if (blendShapes.Length != mesh.sharedMesh.blendShapeCount)
                {
                    blendShapes = new float[mesh.sharedMesh.blendShapeCount];
                }

                for (int i = 0; i < blendShapes.Length; i++)
                {
                    mesh.SetBlendShapeWeight(i, blendShapes[i]);
                }

                UpdateLODs();
            }
        }

        /// <summary>
        /// Update the LODs Values
        /// </summary>
        protected virtual void UpdateLODs()
        {
            for (int i = 0; i < blendShapes.Length; i++)
            {
                UpdateLODs(i);
            }
        }

        /// <summary>
        /// Updates Only a Shape in all LODS
        /// </summary>
        protected virtual void UpdateLODs(int index)
        {
            if (LODs != null)
            {
                foreach (var lods in LODs)
                {
                    lods.SetBlendShapeWeight(index, blendShapes[index]);
                }
            }
        }
    }
}