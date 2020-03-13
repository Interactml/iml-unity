using FIMSpace.Basics;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.GroundFitter
{
    /// <summary>
    /// FM: Changing transform's orientation to fit to the ground, have controll over rotation in Y axis and possibility to move forward
    /// </summary>
    public class FSimpleFitter : FGroundFitter_Base_RootMotion
    {
        protected override void Reset()
        {
            base.Reset();
            RelativeLookUp = false;
            RelativeLookUpBias = 0f;
        }

        void LateUpdate()
        {
            deltaTime = Time.deltaTime;
            FitToGround();
        }

        protected override void FitToGround()
        {
            HandleRootMotionSupport();
            base.FitToGround();
        }

        protected override void HandleRootMotionSupport()
        {
            base.HandleRootMotionSupport();
        }

    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(FSimpleFitter))]
    [UnityEditor.CanEditMultipleObjects]
    public class FSimpleFitter_Editor : FGroundFitter_Base_RootMotionEditor
    {
        public override void OnInspectorGUI()
        {
            FGroundFitter_Base targetScript = (FGroundFitter_Base)target;
            List<string> exclude = new List<string>();

            exclude.Add("drawDebug");
            exclude.Add("drawGizmo");
            exclude.Add("RelativeLookUp");
            exclude.Add("RelativeLookUpBias");

            if (targetScript.LookAheadRaycast == 0f)
            {
                exclude.Add("AheadBlend");
            }

            if (!targetScript.ZoneCast)
            {
                exclude.Add("ZoneCastDimensions");
                exclude.Add("ZoneCastOffset");
                exclude.Add("ZoneCastBias");
                exclude.Add("ZoneCastPrecision");
            }

            DrawPropertiesExcluding(serializedObject, exclude.ToArray());

            FEditor.FEditor_StylesIn.DrawUILine(Color.black * 0.24f);
            DrawRootMotionParameters();
            GUILayout.Space(5);
            //serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}