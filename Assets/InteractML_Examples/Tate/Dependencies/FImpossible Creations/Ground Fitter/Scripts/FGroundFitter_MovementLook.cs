using UnityEngine;

namespace FIMSpace.GroundFitter
{
    /// <summary>
    /// FM: Example component to use groud fitter for movement controller
    /// </summary>
    public class FGroundFitter_MovementLook : FGroundFitter_Movement
    {
        [Header("Movement Look Options")]
        public Transform targetOfLook;
        [Range(0f, 1f)]
        public float FollowSpeed = 1f;
        public bool localOffset = false;

        private Vector3 targetPos;

        protected override void HandleTransforming()
        {
            base.HandleTransforming();

            if (MoveVector != Vector3.zero) SetLookAtPosition(transform.position + Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y + RotationOffset, 0f) * Vector3.forward * 10f);

            if (targetOfLook)
            {
                Vector3 pos = targetPos;
                if (localOffset) pos = transform.TransformPoint(targetPos);

                if (FollowSpeed >= 1f)
                    targetOfLook.position = pos;
                else
                {
                    targetOfLook.position = Vector3.Lerp(targetOfLook.position, pos, Mathf.Lerp(1f, 30f, FollowSpeed) * Time.deltaTime);
                }
            }
        }

        private void SetLookAtPosition(Vector3 tPos)
        {
            if (!localOffset)
                targetPos = tPos + Vector3.up;
            else
                targetPos = transform.InverseTransformPoint(tPos + Vector3.up);
        }
    }
}