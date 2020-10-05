﻿//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            [CustomPropertyDrawer(typeof(EyeShapeTable))]
            public class SRanipal_EyeShapeTableDrawer : PropertyDrawer
            {
                public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
                {
                    EditorGUI.BeginProperty(position, label, property);

                    Rect newFieldPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                    newFieldPosition.height = EditorGUIUtility.singleLineHeight;
                    Rect newLabelPosition = position;
                    newLabelPosition.width -= newFieldPosition.width;

                    SerializedProperty propSkinedMesh = property.FindPropertyRelative("skinnedMeshRenderer");
                    SerializedProperty propEyeShapes = property.FindPropertyRelative("eyeShapes");
                    EditorGUI.PropertyField(newFieldPosition, propSkinedMesh, GUIContent.none);
                    newFieldPosition.y += EditorGUIUtility.singleLineHeight;

                    SkinnedMeshRenderer skinnedMesh = propSkinedMesh.objectReferenceValue as SkinnedMeshRenderer;
                    if (skinnedMesh != null && skinnedMesh.sharedMesh.blendShapeCount > 0)
                    {
                        if (propEyeShapes.arraySize != skinnedMesh.sharedMesh.blendShapeCount)
                        {
                            propEyeShapes.arraySize = skinnedMesh.sharedMesh.blendShapeCount;
                            for (int i = 0; i < skinnedMesh.sharedMesh.blendShapeCount; ++i)
                            {
                                SerializedProperty propEyeShape = propEyeShapes.GetArrayElementAtIndex(i);
                                string elementName = skinnedMesh.sharedMesh.GetBlendShapeName(i);

                                propEyeShape.intValue = (int)EyeShape.None;
                                foreach (EyeShape EyeShape in (EyeShape[])Enum.GetValues(typeof(EyeShape)))
                                {
                                    if (elementName == EyeShape.ToString())
                                        propEyeShape.intValue = (int)EyeShape;
                                }
                            }
                        }
                        for (int i = 0; i < skinnedMesh.sharedMesh.blendShapeCount; ++i)
                        {
                            SerializedProperty propEyeShape = propEyeShapes.GetArrayElementAtIndex(i);
                            newLabelPosition.y = newFieldPosition.y;
                            string elementName = skinnedMesh.sharedMesh.GetBlendShapeName(i);
                            EditorGUI.LabelField(newLabelPosition, "  " + elementName);
                            EditorGUI.PropertyField(newFieldPosition, propEyeShape, GUIContent.none);
                            newFieldPosition.y += EditorGUIUtility.singleLineHeight;
                        }
                    }
                    EditorGUI.EndProperty();
                }

                public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
                {
                    int LineCount = 1;
                    SerializedProperty propSkinedMesh = property.FindPropertyRelative("skinnedMeshRenderer");
                    SkinnedMeshRenderer skinnedMesh = propSkinedMesh.objectReferenceValue as SkinnedMeshRenderer;
                    if (skinnedMesh != null) LineCount += skinnedMesh.sharedMesh.blendShapeCount;
                    return EditorGUIUtility.singleLineHeight * LineCount;
                }
            }
        }
    }
}
#endif