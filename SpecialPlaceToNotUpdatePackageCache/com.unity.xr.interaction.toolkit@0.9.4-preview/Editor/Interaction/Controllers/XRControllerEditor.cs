using System.Collections;
using UnityEngine;
using UnityEditor;

#if LIH_PRESENT
using UnityEngine.Experimental.XR.Interaction;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    [CustomEditor(typeof(XRController))]
    class XRControllerEditor : Editor
    {

        static class Styles
        {
            public static GUIContent updateTrackingTypeLabel = EditorGUIUtility.TrTextContent("Update Tracking Type");
            public static GUIContent enableInputActionsLabel = EditorGUIUtility.TrTextContent("Enable Input Actions");
            public static GUIContent controllerNodeLabel = EditorGUIUtility.TrTextContent("Controller Node");
            public static GUIContent selectUsageLabel = EditorGUIUtility.TrTextContent("Select Usage");
            public static GUIContent activateUsageLabel = EditorGUIUtility.TrTextContent("Activate Usage");
            public static GUIContent uiPressUsageLabel = EditorGUIUtility.TrTextContent("UI Press Usage");
            public static GUIContent axisToPressThreshold = EditorGUIUtility.TrTextContent("Axis To Press Threshold");
            public static GUIContent modelPrefabLabel = EditorGUIUtility.TrTextContent("Model Prefab");
            public static GUIContent modelTransformLabel = EditorGUIUtility.TrTextContent("Model Transform");
            public static GUIContent animateModelLabel = EditorGUIUtility.TrTextContent("Animate Model");
            public static GUIContent modelSelectTransitionLabel = EditorGUIUtility.TrTextContent("Model Select Transition");
            public static GUIContent modelDeSelectTransitionLabel = EditorGUIUtility.TrTextContent("Model DeSelect Transition");


#if LIH_PRESENT
            public static GUIContent poseProviderLabel = EditorGUIUtility.TrTextContent("Pose Provider");
            public static readonly string poseProviderWarning = "This XR Controller is using an external pose provider for tracking.  This takes priority over the Controller Node Setting.";
#endif
        }


        SerializedProperty m_UpdateTrackingType = null;
        SerializedProperty m_EnableInputTracking = null;
        SerializedProperty m_EnableInputActions = null;
        SerializedProperty m_ControllerNode = null;
        SerializedProperty m_SelectUsage = null;
        SerializedProperty m_ActivateUsage = null;
        SerializedProperty m_UiPressUsage = null;
        SerializedProperty m_AxisToPressThreshold = null;
        SerializedProperty m_ModelPrefab = null;
        SerializedProperty m_ModelTransform = null;
        SerializedProperty m_AnimateModel = null;
        SerializedProperty m_ModelSelectTransition = null;
        SerializedProperty m_ModelDeSelectTransition = null;

#if LIH_PRESENT
        SerializedProperty m_PoseProvider = null;
#endif

        private void OnEnable()
        {

            m_UpdateTrackingType = serializedObject.FindProperty("m_UpdateTrackingType");            
            m_EnableInputActions = serializedObject.FindProperty("m_EnableInputActions");
            m_ControllerNode = serializedObject.FindProperty("m_ControllerNode");
            m_SelectUsage = serializedObject.FindProperty("m_SelectUsage");
            m_ActivateUsage = serializedObject.FindProperty("m_ActivateUsage");
            m_UiPressUsage = serializedObject.FindProperty("m_UIPressUsage");
            m_AxisToPressThreshold = serializedObject.FindProperty("m_AxisToPressThreshold");
            m_ModelPrefab = serializedObject.FindProperty("m_ModelPrefab");
            m_ModelTransform = serializedObject.FindProperty("m_ModelTransform");
            m_AnimateModel = serializedObject.FindProperty("m_AnimateModel");
            m_ModelSelectTransition = serializedObject.FindProperty("m_ModelSelectTransition");
            m_ModelDeSelectTransition = serializedObject.FindProperty("m_ModelDeSelectTransition");

#if LIH_PRESENT
            m_PoseProvider = serializedObject.FindProperty("m_PoseProvider");
#endif


        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((XRController)target), typeof(XRController), false);
            GUI.enabled = true;


            EditorGUILayout.PropertyField(m_UpdateTrackingType, Styles.updateTrackingTypeLabel);            

            EditorGUILayout.PropertyField(m_EnableInputActions, Styles.enableInputActionsLabel);

#if LIH_PRESENT
            EditorGUILayout.PropertyField(m_PoseProvider, Styles.poseProviderLabel);
            if (m_PoseProvider.objectReferenceValue != null)            
            {
                EditorGUILayout.HelpBox(Styles.poseProviderWarning, MessageType.Info, true);
            }
#endif 

            EditorGUILayout.PropertyField(m_ControllerNode, Styles.controllerNodeLabel);     
            EditorGUILayout.PropertyField(m_SelectUsage, Styles.selectUsageLabel);
            EditorGUILayout.PropertyField(m_ActivateUsage, Styles.activateUsageLabel);
            EditorGUILayout.PropertyField(m_UiPressUsage, Styles.uiPressUsageLabel);
            EditorGUILayout.PropertyField(m_AxisToPressThreshold, Styles.axisToPressThreshold);

            // Header - Model

            EditorGUILayout.PropertyField(m_ModelPrefab, Styles.modelPrefabLabel);
            EditorGUILayout.PropertyField(m_ModelTransform, Styles.modelTransformLabel);
            EditorGUILayout.PropertyField(m_AnimateModel, Styles.animateModelLabel);
            EditorGUILayout.PropertyField(m_ModelSelectTransition, Styles.modelSelectTransitionLabel);
            EditorGUILayout.PropertyField(m_ModelDeSelectTransition, Styles.modelDeSelectTransitionLabel);            
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
