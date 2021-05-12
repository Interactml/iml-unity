using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MalbersAnimations
{
    [CustomEditor(typeof(TransformAnimation))]
    public class TransformAnimationEditor : Editor
    {
        TransformAnimation My;
        private MonoScript script;

        void OnEnable()
        {
            My = (TransformAnimation)target;
            script = MonoScript.FromScriptableObject(My);
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Use to animate a transform to this values", MessageType.None);
            EditorGUILayout.EndVertical();


            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUI.BeginDisabledGroup(true);
                script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("animTrans"), new GUIContent("Used for"));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUIUtility.labelWidth = 50;
                var type = My.animTrans == TransformAnimation.AnimTransType.TransformAnimation;
                My.time = EditorGUILayout.FloatField(new GUIContent(type ? "Time" : "Mount", type ? "Duration of the animation" :"Scale for the Mount Animation on the MountTriger Script"), My.time);
                My.delay = EditorGUILayout.FloatField(new GUIContent(type ? "Delay" : "Dismount", type? "Delay before the animation is started" : "Scale for the Dismount Animation on the MountTriger Script"), My.delay);
                EditorGUIUtility.labelWidth = 0;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                
                My.UsePosition = GUILayout.Toggle(My.UsePosition, new GUIContent("Position"), EditorStyles.miniButton);
                My.UseRotation = GUILayout.Toggle(My.UseRotation, new GUIContent("Rotation"), EditorStyles.miniButton);
                My.UseScale = GUILayout.Toggle(My.UseScale, new GUIContent("Scale"), EditorStyles.miniButton);
                EditorGUILayout.EndHorizontal();

                if (My.UsePosition)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal();
                    My.SeparateAxisPos = GUILayout.Toggle(My.SeparateAxisPos, new GUIContent("|","Separated Axis"),EditorStyles.miniButton);
                    EditorGUILayout.LabelField("Position", GUILayout.MinWidth(50), GUILayout.MaxWidth(100));
                    My.Position = EditorGUILayout.Vector3Field("", My.Position, GUILayout.MinWidth(120));
                    if (!My.SeparateAxisPos) My.PosCurve = EditorGUILayout.CurveField(My.PosCurve, GUILayout.MinWidth(30));
                    EditorGUILayout.EndHorizontal();

                    if (My.SeparateAxisPos)
                    {
                        EditorGUILayout.BeginHorizontal();
                        My.PosXCurve = EditorGUILayout.CurveField(My.PosXCurve, Color.red, new Rect());
                        My.PosYCurve = EditorGUILayout.CurveField(My.PosYCurve, Color.green, new Rect());
                        My.PosZCurve = EditorGUILayout.CurveField(My.PosZCurve, Color.blue, new Rect());
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }

                if (My.UseRotation)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal();
                    My.SeparateAxisRot = GUILayout.Toggle(My.SeparateAxisRot, new GUIContent("|", "Separated Axis"), EditorStyles.miniButton);
                    EditorGUILayout.LabelField("Rotation", GUILayout.MinWidth(50), GUILayout.MaxWidth(100));
                    My.Rotation = EditorGUILayout.Vector3Field("", My.Rotation, GUILayout.MinWidth(120));
                    if (!My.SeparateAxisRot) My.RotCurve = EditorGUILayout.CurveField(My.RotCurve, GUILayout.MinWidth(30));
                    EditorGUILayout.EndHorizontal();

                    if (My.SeparateAxisRot)
                    {
                        EditorGUILayout.BeginHorizontal();
                        My.RotXCurve = EditorGUILayout.CurveField(My.RotXCurve, Color.red, new Rect());
                        My.RotYCurve = EditorGUILayout.CurveField(My.RotYCurve, Color.green, new Rect());
                        My.RotZCurve = EditorGUILayout.CurveField(My.RotZCurve, Color.blue, new Rect());
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }

                if (My.UseScale)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField("Scale", GUILayout.MinWidth(50), GUILayout.MaxWidth(100));
                    My.Scale = EditorGUILayout.Vector3Field("", My.Scale, GUILayout.MinWidth(120));
                    My.ScaleCurve = EditorGUILayout.CurveField(My.ScaleCurve, GUILayout.MinWidth(50));
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
            serializedObject.ApplyModifiedProperties();
        }

    }
}