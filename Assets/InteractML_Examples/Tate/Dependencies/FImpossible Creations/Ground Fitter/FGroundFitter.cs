using FIMSpace.Basics;
using UnityEngine;

namespace FIMSpace.GroundFitter
{
    /// <summary>
    /// FM: Changing transform's orientation to fit to the ground, have controll over rotation in Y axis and possibility to move forward
    /// </summary>
    public class FGroundFitter : FGroundFitter_Base_RootMotion
    {
        [Header("< Specific Parameters >")]
        /// <summary> If algorithm should be executed in LateUpdate queue </summary>
        public EFUpdateClock UpdateClock = EFUpdateClock.Update;


        protected override void Reset()
        {
            base.Reset();
            RelativeLookUp = true;
            RelativeLookUpBias = 0.25f;
        }


        #region Update Clock Methods

        void Update()
        {
            if (UpdateClock != EFUpdateClock.Update) return;
            deltaTime = Time.deltaTime;
            FitToGround();
        }

        void FixedUpdate()
        {
            if (UpdateClock != EFUpdateClock.FixedUpdate) return;
            deltaTime = Time.fixedDeltaTime;
            FitToGround();
        }

        void LateUpdate()
        {
            if (UpdateClock != EFUpdateClock.LateUpdate) return;
            deltaTime = Time.deltaTime;
            FitToGround();
        }

        public void RefreshDelta()
        {
            switch (UpdateClock)
            {
                case EFUpdateClock.Update:
                    deltaTime = Time.deltaTime;
                    break;
                case EFUpdateClock.LateUpdate:
                    deltaTime = Time.deltaTime;
                    break;
                case EFUpdateClock.FixedUpdate:
                    deltaTime = Time.fixedDeltaTime;
                    break;
            }
        }

        #endregion


        protected override void FitToGround()
        {
            HandleRootMotionSupport();
            base.FitToGround();
        }

        protected override void HandleRootMotionSupport()
        {
            base.HandleRootMotionSupport();
            if (HandleRootMotion) UpdateClock = EFUpdateClock.LateUpdate;
        }

    }


#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(FGroundFitter))]
    [UnityEditor.CanEditMultipleObjects]
    public class FGroundFitter_Editor : FGroundFitter_Base_RootMotionEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}