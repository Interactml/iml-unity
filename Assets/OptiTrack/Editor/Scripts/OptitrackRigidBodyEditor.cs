using System.Collections;
using UnityEditor;
using UnityEngine;


[CustomEditor( typeof( OptitrackRigidBody ) )]
public class OptitrackRigidBodyEditor : Editor
{
    Material m_cachedMarkerMaterial = null;
    Material m_markerMaterial
    {
        get
        {
            if ( m_cachedMarkerMaterial )
                return m_cachedMarkerMaterial;

            m_cachedMarkerMaterial = AssetDatabase.LoadAssetAtPath<Material>( "Assets/OptiTrack/Editor/Materials/MarkerMaterial.mat" );
            return m_cachedMarkerMaterial;
        }
    }

    Mesh m_cachedMarkerMesh = null;
    Mesh m_markerMesh
    {
        get
        {
            if ( m_cachedMarkerMesh )
                return m_cachedMarkerMesh;

            m_cachedMarkerMesh = AssetDatabase.LoadAssetAtPath<Mesh>( "Assets/OptiTrack/Editor/Meshes/MarkerMesh.fbx" );
            return m_cachedMarkerMesh;
        }
    }


    /// <summary>
    /// Draws marker visualizations in the editor viewport for any selected OptitrackRigidBody component.
    /// </summary>
    void OnSceneGUI()
    {
        OptitrackRigidBody rb = target as OptitrackRigidBody;

        if ( !rb || rb.StreamingClient == null )
        {
            return;
        }

        rb.StreamingClient._EnterFrameDataUpdateLock();

        try
        {
            OptitrackRigidBodyState rbState = rb.StreamingClient.GetLatestRigidBodyState( rb.RigidBodyId );

            if ( rbState != null && rbState.Markers != null )
            {
                for ( int iMarker = 0; iMarker < rbState.Markers.Count; ++iMarker )
                {
                    OptitrackMarkerState marker = rbState.Markers[iMarker];
                    Vector3 markerPos = marker.Position;

                    if ( rb.transform.parent )
                    {
                        markerPos = rb.transform.parent.TransformPoint( markerPos );
                    }

                    Matrix4x4 markerTransform = Matrix4x4.TRS( markerPos, Quaternion.identity, new Vector3( marker.Size, marker.Size, marker.Size ) );
                    Graphics.DrawMesh( m_markerMesh, markerTransform, m_markerMaterial, 0, camera: null, submeshIndex: 0, properties: null, castShadows: false, receiveShadows: false );
                }
            }
        }
        finally
        {
            rb.StreamingClient._ExitFrameDataUpdateLock();
        }
    }
}
