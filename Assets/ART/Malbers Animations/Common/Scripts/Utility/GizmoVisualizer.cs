using UnityEngine;
using System.Collections;

public class GizmoVisualizer : MonoBehaviour
{
#if UNITY_EDITOR
    public enum GizmoType
    {
        Cube,
        Sphere,
    }
    public bool UseColliders;
    public GizmoType gizmoType;
    [Range(0,1f)]
    public float alpha = 1;
    public float debugSize = 0.03f;
    public Color DebugColor = Color.blue;
    public bool DrawAxis;
    public float AxisSize = 0.65f;

    private Collider _collider;

    Collider _Collider
    {
        get
        {
            if (_collider == null)
            {
                _collider = GetComponent<Collider>();
            }
            return _collider;
        }
    }
  

  void OnDrawGizmos()
    {
        if (DrawAxis)
        {
            Color NewColor = Color.blue;
            NewColor.a = alpha;
            UnityEditor.Handles.color = NewColor;

            UnityEditor.Handles.ArrowHandleCap(0, transform.position, transform.rotation, AxisSize, EventType.Repaint);
        }

        Gizmos.color = DebugColor;
        Gizmos.matrix = transform.localToWorldMatrix;

        if (_Collider && UseColliders)
        {
            UsesColliders(false);
            return;
        }

        switch (gizmoType)
        {
            case GizmoType.Cube:
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(debugSize, debugSize, debugSize));
                Gizmos.color = new Color(DebugColor.r, DebugColor.g, DebugColor.b, alpha);
                Gizmos.DrawCube(Vector3.zero, Vector3.one * debugSize);
                break;
            case GizmoType.Sphere:
               
                Gizmos.DrawWireSphere(Vector3.zero, debugSize);
                Gizmos.color = new Color(DebugColor.r, DebugColor.g, DebugColor.b, alpha);
                Gizmos.DrawSphere(Vector3.zero, debugSize);
                break;
            default:
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1,1,0,1);
        Gizmos.matrix = transform.localToWorldMatrix;

        if (UseColliders && _Collider)
        {
            UsesColliders(true);
            return;
        }
        switch (gizmoType)
        {
            case GizmoType.Cube:
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one * debugSize);       
                break;
            case GizmoType.Sphere:
                Gizmos.DrawWireSphere(Vector3.zero, debugSize);       
                break;
        }
    }

    void UsesColliders(bool sel)
    {
        if (_Collider is BoxCollider)
        {
            BoxCollider _C = _Collider as BoxCollider;
            if (!_C.enabled) return;
            var sizeX = transform.lossyScale.x * _C.size.x;
            var sizeY = transform.lossyScale.y * _C.size.y;
            var sizeZ = transform.lossyScale.z * _C.size.z;
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(_C.bounds.center, transform.rotation, new Vector3(sizeX, sizeY, sizeZ));

            Gizmos.matrix = rotationMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            if (!sel)
            {
                Gizmos.color = new Color(DebugColor.r, DebugColor.g, DebugColor.b, alpha);
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }

        }
        else if (_Collider is SphereCollider)
        {
            SphereCollider _C = _Collider as SphereCollider;

            if (!_C.enabled) return;

            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireSphere(Vector3.zero + _C.center, _C.radius);
            if (!sel)
            {
                Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, alpha);
                Gizmos.DrawSphere(Vector3.zero + _C.center, _C.radius);
            }
        }
    }
#endif
}
