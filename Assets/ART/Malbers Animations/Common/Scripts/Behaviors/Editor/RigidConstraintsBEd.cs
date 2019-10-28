using UnityEngine;
using UnityEditor;

namespace MalbersAnimations
{
    [CustomEditor(typeof(RigidConstraintsB))]
    public class RigidConstraintsBEd : Editor
    {
        private RigidConstraintsB MJB;

        private void OnEnable()
        {
            MJB = ((RigidConstraintsB)target);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update(); 

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Modify the Rigidbody Constraints attached to this Animator",MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnEnterDrag"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            MJB.OnEnter  = EditorGUILayout.Toggle("On Enter" ,MJB.OnEnter, EditorStyles.radioButton);
            if (MJB.OnEnter)
                MJB.OnExit = false;
            else
                MJB.OnExit = true;
            MJB.OnExit = EditorGUILayout.Toggle("On Exit", MJB.OnExit, EditorStyles.radioButton);

            if (MJB.OnExit)
                MJB.OnEnter = false;
            else
                MJB.OnEnter = true;

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Constraints  ",EditorStyles.boldLabel,  GUILayout.MaxWidth(105));
            EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(15));
            EditorGUILayout.LabelField("X", EditorStyles.boldLabel, GUILayout.MaxWidth(15));
            EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(15));
            EditorGUILayout.LabelField("Y", EditorStyles.boldLabel, GUILayout.MaxWidth(15));
            EditorGUILayout.LabelField("     Z", EditorStyles.boldLabel, GUILayout.MaxWidth(35));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Position ", GUILayout.MaxWidth(105));
            EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(15));
            MJB.PosX = EditorGUILayout.Toggle(MJB.PosX, GUILayout.MaxWidth(15));
            EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(15));
            MJB.PosY = EditorGUILayout.Toggle(MJB.PosY, GUILayout.MaxWidth(15));
            EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(15));
            MJB.PosZ = EditorGUILayout.Toggle(MJB.PosZ, GUILayout.MaxWidth(15));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Rotation ", GUILayout.MaxWidth(105));
            EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(15));
            MJB.RotX = EditorGUILayout.Toggle(MJB.RotX, GUILayout.MaxWidth(15));
            EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(15));
            MJB.RotY = EditorGUILayout.Toggle(MJB.RotY, GUILayout.MaxWidth(15));
            EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(15));
            MJB.RotZ = EditorGUILayout.Toggle(MJB.RotZ, GUILayout.MaxWidth(15));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();

        }


    }
}