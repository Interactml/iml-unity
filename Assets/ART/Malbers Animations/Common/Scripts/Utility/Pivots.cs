using UnityEngine;


namespace MalbersAnimations
{
    /// <summary>
    /// This Class is used to find all game objects using Pivots as a component
    /// </summary>
    public class Pivots : MonoBehaviour
    {
        public float multiplier = 1;
        public bool debug = true;
        public float debugSize = 0.03f;
        public Color DebugColor = Color.blue;
        public bool drawRay = true;

        public Vector3 GetPivot
        {
            get { return transform.position; }
        }

        public float Y
        {
            get { return transform.position.y; }
        }

        void OnDrawGizmos()
        {
            if (debug)
            {
                Gizmos.color = DebugColor;
                Gizmos.DrawWireSphere(GetPivot, debugSize);
                if (drawRay)
                {
                    Gizmos.DrawRay(GetPivot, -transform.up * multiplier * transform.root.localScale.y);
                }
            }
        }
    }
}