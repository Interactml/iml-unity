//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System.Runtime.InteropServices;
using UnityEngine;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            /// <summary>
            /// A very basic mirror.
            /// </summary>
            [RequireComponent(typeof(Camera))]
            public class SRanipal_MirrorCameraSample_Eye : MonoBehaviour
            {
                private const float Distance = 0.6f;
                private static EyeData eyeData = new EyeData();
                private bool eye_callback_registered = false;

                private void Update()
                {
                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;

                    if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
                    {
                        SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                        eye_callback_registered = true;
                    }
                    else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
                    {
                        SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }

                    //When gaze ray data is valid, place the mirror gameobject directly in front of the player camera.
                    Ray GazeRay;
                    bool get_gaze_ray;
                    if (eye_callback_registered == true)
                        get_gaze_ray = SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out GazeRay, eyeData);
                    else {
                        get_gaze_ray = SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out GazeRay);
                    }
                    if (get_gaze_ray)
                    {
                        SetMirroTransform();
                        enabled = false;
                    }
                }

                private void Release() {
                    if (eye_callback_registered == true)
                    {
                        SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }
                }
                private void SetMirroTransform()
                {
                    transform.position = Camera.main.transform.position + Camera.main.transform.forward * Distance;
                    transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);
                    transform.LookAt(Camera.main.transform);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                }
                private static void EyeCallback(ref EyeData eye_data)
                {
                    eyeData = eye_data;
                }
            }
        }
    }
}