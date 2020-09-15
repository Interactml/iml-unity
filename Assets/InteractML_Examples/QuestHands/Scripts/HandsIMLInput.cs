using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
using OVR;

public class HandsIMLInput : MonoBehaviour
{

    [SendToIMLController]
    public float[] LeftHandRotation;
    [SendToIMLController]
    public float[] RightHandRotation;

    public OVRHand leftHand;
    public OVRHand rightHand;

    // Start is called before the first frame update
    void Start()
    {
        LeftHandRotation = new float[3*(int)OVRPlugin.BoneId.Hand_MaxSkinnable];
        RightHandRotation = new float[3*(int)OVRPlugin.BoneId.Hand_MaxSkinnable];
    }

    // Update is called once per frame
    void Update()
    {
        LeftHandRotation = HandRotation(leftHand);
        RightHandRotation = HandRotation(rightHand);

    }

    private float[] HandRotation(OVRSkeleton.IOVRSkeletonDataProvider hand)
    {
        float[] handRotation = new float[3 * (int)OVRPlugin.BoneId.Hand_MaxSkinnable];
        if(hand != null)
        {
            var handPose = hand.GetSkeletonPoseData();
            OVRPlugin.Quatf[] boneRotations = handPose.BoneRotations;
            if(handPose.IsDataValid)
            {
                for (int i = 0; i < (int)OVRPlugin.BoneId.Hand_MaxSkinnable; i++)
                {
                    Quaternion q = new Quaternion(boneRotations[i].x, boneRotations[i].y, boneRotations[i].z, boneRotations[0].w);
                    handRotation[3 * i] = q.eulerAngles[0];
                    handRotation[3 * i + 1] = q.eulerAngles[1];
                    handRotation[3 * i + 2] = q.eulerAngles[2];
                }
            }
            
        }
        
        return handRotation;
    }
}
