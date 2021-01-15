using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEditor.XR.Interaction.Toolkit
{
    [CustomEditor(typeof(SnapTurnProvider))]
    internal class SnapTurnProviderEditor : Editor
    {
        SerializedProperty m_LocomotionSystem;
        SerializedProperty m_TurnUsage;
        SerializedProperty m_Controllers;
        SerializedProperty m_TurnAmount;
        SerializedProperty m_Duration;
        SerializedProperty m_DeadZone;
        SerializedProperty m_ActivateTimeout;

        static class Tooltips
        {
            public static readonly GUIContent locomotionSystem = new GUIContent(
                "System",
                "The locomotion system that the snap turn provider will interface with");

            public static readonly GUIContent controllers = new GUIContent(
               "Controllers",
               "XRControllers that allow for snap turning.");

            public static readonly GUIContent turnUsage = new GUIContent(
                "Turn Input Source",
                "The Input axis to use to begin a snap turn");

            public static readonly GUIContent turnAmount = new GUIContent(
                "Turn Amount",
                "the number of degrees to turn around the Y axis when performing a right handed snap turn. This will automatically be negated for left turns.");

            public static readonly GUIContent activateTimeout = new GUIContent(
                "Activation Timeout",
                "how long between a successful snap turn does the use need to wait before being able to perform a subsequent snap turn");

            public static readonly GUIContent deadZone = new GUIContent(
                "Dead Zone",
                "Minimum distance of axis travel before performing a snap turn");
        }


        void OnEnable()
        {
            m_LocomotionSystem = serializedObject.FindProperty("m_System");
            m_TurnUsage = serializedObject.FindProperty("m_TurnUsage");
            m_Controllers = serializedObject.FindProperty("m_Controllers");
            m_TurnAmount = serializedObject.FindProperty("m_TurnAmount");          
            m_DeadZone = serializedObject.FindProperty("m_DeadZone");
            m_ActivateTimeout = serializedObject.FindProperty("m_DebounceTime");
        }

        public override void OnInspectorGUI()
        {

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((SnapTurnProvider)target), typeof(SnapTurnProvider), false);
            GUI.enabled = true;

            serializedObject.Update();
            
            EditorGUILayout.PropertyField(m_LocomotionSystem, Tooltips.locomotionSystem);

            EditorGUILayout.PropertyField(m_TurnUsage, Tooltips.turnUsage);
            EditorGUILayout.PropertyField(m_Controllers, Tooltips.controllers);

            EditorGUILayout.PropertyField(m_TurnAmount, Tooltips.turnAmount);
            EditorGUILayout.PropertyField(m_DeadZone, Tooltips.deadZone);
            EditorGUILayout.PropertyField(m_ActivateTimeout, Tooltips.activateTimeout);

            serializedObject.ApplyModifiedProperties();
        }
    }
}