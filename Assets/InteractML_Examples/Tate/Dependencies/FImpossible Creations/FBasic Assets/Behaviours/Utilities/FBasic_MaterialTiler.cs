using UnityEngine;

namespace FIMSpace.Basics
{
    /// <summary>
    /// FM: Basic script to make looped material fit in tiling to scale of object.
    /// </summary>
    public class FBasic_MaterialTiler : FBasic_MaterialScriptBase
    {
        /// Info about triggering OnValidate()
        [Header("When you scale object change")]
        [Header("something in script to apply")]
        [Space(10)]
        [Tooltip("Texture identificator in shader")]
        public string TextureProperty = "_MainTex";

        [Tooltip("How much tiles should be multiplied according to gameObject's scale")]
        public Vector2 ScaleValues = new Vector2(1f,1f);

        [Tooltip("When scale on Y should be same as X")]
        public bool EqualDimensions = false;

        /// <summary>
        /// Method executed every time something is changed in inspector of this component
        /// </summary>
        private void OnValidate()
        {
            GetRendererMaterial();

            if (EqualDimensions) ScaleValues.y = ScaleValues.x;

            TileMaterialToScale();
        }

        /// <summary>
        /// Tiling material to given parameters and scale of game object's transform.
        /// </summary>
        private void TileMaterialToScale()
        {
            if (RendererMaterial == null || ObjectRenderer == null) return;

            Vector2 newScale = ScaleValues;
            newScale.x *= transform.localScale.x;
            newScale.y *= transform.localScale.z;

            RendererMaterial.SetTextureScale("_MainTex", newScale);
            ObjectRenderer.material = RendererMaterial;
        }
    }
}