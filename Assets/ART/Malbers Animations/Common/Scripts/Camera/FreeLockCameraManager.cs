using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations.Utilities;

namespace MalbersAnimations
{
    [CreateAssetMenu(menuName = "Malbers Animations/Camera/FreeLook Camera Manager")]
    public class FreeLockCameraManager : ScriptableObject
    {
        public float transition = 1f;                   //Transitions time from one state to another

        public FreeLookCameraState Default;
        public FreeLookCameraState AimRight;
        public FreeLookCameraState AimLeft;
        public FreeLookCameraState Mounted;

        MFreeLookCamera mCamera;
        Camera cam;

        private FreeLookCameraState NextState;
        protected FreeLookCameraState currentState;

        IEnumerator ChangeStates;

        protected Transform MountedTarget;
        protected Transform RiderTarget;

        public void SetCamera(MFreeLookCamera Freecamera)
        {
            mCamera = Freecamera;
            if (mCamera) cam = mCamera.Cam.GetComponent<Camera>();
            ChangeStates = StateTransition(transition);

            currentState = null;
            NextState = null;
            Mounted = null;
            MountedTarget = null;
        }

        public void ChangeTarget(Transform tranform)
        {
            if (mCamera == null) return;
            mCamera.SetTarget(tranform);
        }

        public void SetRiderTarget(Transform tranform)
        {
            RiderTarget = tranform;
        }

        public void SetMountedTarget(Transform tranform)
        {
            MountedTarget = tranform;
            if (mCamera == null) return;
            ChangeTarget(tranform);
        }

        public void SetMountedState(FreeLookCameraState state)
        {
            Mounted = state;
            SetCameraState(state);
        }


        void UpdateState(FreeLookCameraState state)
        {
            if (mCamera == null) return;
            if (state == null) return;

            mCamera.Pivot.localPosition = state.PivotPos;
            mCamera.Cam.localPosition = state.CamPos;
            cam.fieldOfView = state.CamFOV;
        }

        public void SetAim(int ID)
        {
            if (mCamera == null) return;

            if (ID == -1 && AimLeft)
            {
                SetCameraState(AimLeft);
                mCamera.SetTarget(RiderTarget);
            }
            else if (ID == 1 && AimRight)
            {
                SetCameraState(AimRight);
                mCamera.SetTarget(RiderTarget);
            }
            else
            {
                SetCameraState(Mounted ?? Default);
                if (MountedTarget) mCamera.SetTarget(MountedTarget);
            }
        }

        public void SetCameraState(FreeLookCameraState state)
        {
            if (mCamera == null) return;
            if (state == null) return;

            NextState = state;

            if (currentState && NextState == currentState) return;

            mCamera.StopCoroutine(ChangeStates);
            ChangeStates = StateTransition(transition);
            mCamera.StartCoroutine(ChangeStates);
        }


        IEnumerator StateTransition(float time)
        {
            float elapsedTime = 0;
            currentState = NextState;
            while (elapsedTime < time)
            {
                mCamera.Pivot.localPosition = Vector3.Lerp(mCamera.Pivot.localPosition, NextState.PivotPos, Mathf.SmoothStep(0, 1, elapsedTime / time));
                mCamera.Cam.localPosition = Vector3.Lerp(mCamera.Cam.localPosition, NextState.CamPos, Mathf.SmoothStep(0, 1, elapsedTime / time));
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, NextState.CamFOV, Mathf.SmoothStep(0, 1, elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            UpdateState(NextState);

            NextState = null;
        }
    }
}