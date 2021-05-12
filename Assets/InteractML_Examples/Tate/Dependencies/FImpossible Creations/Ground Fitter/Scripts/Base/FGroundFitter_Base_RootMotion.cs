using UnityEngine;

namespace FIMSpace.GroundFitter
{
    /// <summary>
    /// FM: Base class for ground fitter components with root motion methods implementation
    /// </summary>
    public abstract class FGroundFitter_Base_RootMotion : FGroundFitter_Base
    {
        [Tooltip("Making ground fitter translate with root motion")]
        [HideInInspector]
        public bool HandleRootMotion = false;
        [SerializeField]
        [HideInInspector]
        protected Transform parentTransform;
        [SerializeField]
        [HideInInspector]
        protected CharacterController optionalCharContr;
        [SerializeField]
        [HideInInspector]
        protected bool rootMotionRotation = true;

        protected Animator rootMAnimator;

        protected override void Reset()
        {
            base.Reset();
            parentTransform = transform;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected virtual void HandleRootMotionSupport()
        {
            if (HandleRootMotion)
            {
                if (!rootMAnimator) rootMAnimator = GetComponentInChildren<Animator>();
                if (rootMAnimator.gameObject != gameObject) if (!rootMAnimator.applyRootMotion)
                        if (!rootMAnimator.GetComponent<FGroundFitter_RootMotionHelper>()) rootMAnimator.gameObject.AddComponent<FGroundFitter_RootMotionHelper>().OptionalFitter = this;

                //UpdateClock = EFUpdateClock.LateUpdate;
                rootMAnimator.applyRootMotion = true;
            }
        }


        internal virtual void OnAnimatorMove()
        {
            if (!rootMAnimator) return;


            if (optionalCharContr)
            {
                if (rootMAnimator.deltaPosition != Vector3.zero)
                {
                    if (TransformToRotate != transform)
                        optionalCharContr.Move(TransformToRotate.rotation * rootMAnimator.deltaPosition);
                    else
                        optionalCharContr.Move(rootMAnimator.deltaPosition);
                }

                rootMAnimator.rootPosition = TransformToRotate.position;
            }
            else
            {
                // Change position with Root Motion support
                if (TransformToRotate != transform)
                    parentTransform.position += (TransformToRotate.rotation * rootMAnimator.deltaPosition);
                else
                    parentTransform.position += (rootMAnimator.deltaPosition);
            }

            rootMAnimator.rootPosition = TransformToRotate.position;
            rootMAnimator.rootRotation = LastRotation;

            // Change rotation with Root Motion support
            if (rootMotionRotation)
            {
                rootMAnimator.rootRotation = LastRotation;

                float angleInDegrees; Vector3 rotationAxis;
                rootMAnimator.deltaRotation.ToAngleAxis(out angleInDegrees, out rotationAxis);

                float rotationValue = (rotationAxis * angleInDegrees * Mathf.Deg2Rad).y;
                UpAxisRotation += rotationValue * 57.290154327f;
            }
        }
    
    }

    #region Custom Inspector

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(FGroundFitter_Base_RootMotion))]
    [UnityEditor.CanEditMultipleObjects]
    public class FGroundFitter_Base_RootMotionEditor : FGroundFitter_BaseEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            FEditor.FEditor_StylesIn.DrawUILine(Color.black * 0.24f);
            DrawRootMotionParameters();
            GUILayout.Space(5);
        }

        protected void DrawRootMotionParameters()
        {
            UnityEditor.EditorGUILayout.BeginVertical(FEditor.FEditor_StylesIn.LGrayBackground);

            FGroundFitter_Base_RootMotion targetScript = (FGroundFitter_Base_RootMotion)target;
            UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("HandleRootMotion"));

            if (targetScript.HandleRootMotion)
            {
                Color preC = GUI.color;
                GUI.color = new Color(preC.r, preC.g, preC.b, 0.625f);
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("TransformToRotate"));
                GUI.color = preC;

                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("parentTransform"));
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("optionalCharContr"));
                UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("rootMotionRotation"));
            }

            UnityEditor.EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    #endregion

}