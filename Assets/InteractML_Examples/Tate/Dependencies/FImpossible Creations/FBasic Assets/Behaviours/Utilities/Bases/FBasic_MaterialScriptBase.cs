using UnityEngine;

namespace FIMSpace.Basics
{
    /// <summary>
    /// FM: Basic script to get instance of material for editing, must be parented to work
    /// </summary>
    public abstract class FBasic_MaterialScriptBase : MonoBehaviour
    {
        protected Material RendererMaterial;
        protected Renderer ObjectRenderer;

        /// <summary>
        /// If we don't have specified material, we getting instance from renderer.
        /// </summary>
        protected Material GetRendererMaterial()
        {
            if (!Application.isPlaying)
            {
                // Refreshing if something changed
                if (ObjectRenderer != null)
                    if (ObjectRenderer.sharedMaterial != RendererMaterial)
                        RendererMaterial = null;
            }

            if (RendererMaterial == null || ObjectRenderer == null)
            {
                Renderer rend = gameObject.GetComponent<Renderer>();
                if (rend == null) rend = gameObject.GetComponentInChildren<Renderer>();
                if (rend == null)
                {
                    Debug.Log("<color=red>No renderer in " + gameObject.name + "!</color>");
                    return null;
                }

                ObjectRenderer = rend;

                if (Application.isPlaying)
                    RendererMaterial = rend.material;
                else
                    RendererMaterial = new Material(rend.sharedMaterial);
            }

            return RendererMaterial;
        }
    }
}
