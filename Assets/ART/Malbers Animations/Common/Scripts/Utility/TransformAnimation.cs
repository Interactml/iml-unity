using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations
{
    public enum AnimCycle {None, Loop, Repeat, PingPong }

    [CreateAssetMenu(menuName = "Malbers Animations/Anim Transform")]
    public class TransformAnimation : ScriptableObject
    {
        public  enum AnimTransType { TransformAnimation, MountTriggerAdjustment }

        public AnimTransType animTrans = AnimTransType.TransformAnimation;

        static Keyframe[] K = {new Keyframe(0,0), new Keyframe(1,1) };

        public float time = 1f;
        public float delay = 1f;
        //public AnimCycle cycle;

        public bool UsePosition = false;
        public Vector3 Position;
        public AnimationCurve PosCurve = new AnimationCurve(K);

        public bool SeparateAxisPos = false;
        public AnimationCurve PosXCurve = new AnimationCurve(K);
        public AnimationCurve PosYCurve = new AnimationCurve(K);
        public AnimationCurve PosZCurve = new AnimationCurve(K);

        public bool UseRotation = false;
        public Vector3 Rotation;
        public AnimationCurve RotCurve = new AnimationCurve(K);

        public bool SeparateAxisRot = false;
        public AnimationCurve RotXCurve = new AnimationCurve(K);
        public AnimationCurve RotYCurve = new AnimationCurve(K);
        public AnimationCurve RotZCurve = new AnimationCurve(K);

        public bool UseScale = false;
        public Vector3 Scale = Vector3.one;
        public AnimationCurve ScaleCurve = new AnimationCurve(K);

    }
}