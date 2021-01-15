using System.Collections;
using UnityEngine;
using UnityEditor;

namespace UnityEngine.XR.Interaction.Toolkit
{
    [CustomEditor(typeof(XRRig))]
    class XRRigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((XRRig)target), typeof(XRRig), false);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_RigBaseGameObject"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_CameraFloorOffsetObject"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_CameraGameObject"));
#if UNITY_2019_3_OR_NEWER
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TrackingOriginMode"));
#else
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TrackingSpace"));
#endif
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_CameraYOffset"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
