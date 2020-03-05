using UnityEngine;

namespace FIMSpace.GroundFitter
{
    public class FGroundFitter_Input : FGroundFitter_InputBase
    {
        protected virtual void Update()
        {

            if (Input.GetKeyDown(KeyCode.Space)) TriggerJump();

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                if (Input.GetKey(KeyCode.LeftShift)) Sprint = true; else Sprint = false;

                RotationOffset = 0f;
                if (Input.GetKey(KeyCode.A)) RotationOffset = -90;
                if (Input.GetKey(KeyCode.D)) RotationOffset = 90;
                if (Input.GetKey(KeyCode.S)) RotationOffset = 180;

                MoveVector = Vector3.forward;
            }
            else
            {
                Sprint = false;
                MoveVector = Vector3.zero;
            }

            if (Input.GetKey(KeyCode.X)) MoveVector -= Vector3.forward;
            if (Input.GetKey(KeyCode.Q)) MoveVector += Vector3.left;
            if (Input.GetKey(KeyCode.E)) MoveVector += Vector3.right;

            MoveVector.Normalize();

            controller.Sprint = Sprint;
            controller.MoveVector = MoveVector;
            controller.RotationOffset = RotationOffset;
        }
    }
}