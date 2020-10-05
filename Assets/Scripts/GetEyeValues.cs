using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;
using System.Runtime.InteropServices;

public class GetEyeValues : MonoBehaviour
{
    private FocusInfo FocusInfo;
    private readonly float MaxDistance = 20;
    private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };
    private static EyeData eyeData = new EyeData();
    private bool eye_callback_registered = false;

    public string TargetLayer = "Default";
    public Vector3 GazePoint;
    public bool IsGazing;


    private float l_openess, r_openess;
    private static VerboseData verboseData;
    public float pupilDiameterLeft, pupilDiameterRight;
    public Vector2 pupilPositionLeft, pupilPositionRight;

    public float eyeOpenLeft, eyeOpenRight;

    [Header("Options")]
    public bool DebugData;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    //void Update()
    //{
    //    FocusInfo focusInfo;
    //    Ray testRay;
    //    string currentFocus = "";

    //    if (SRanipal_Eye.Focus(GazeIndex.COMBINE, out testRay, out focusInfo))
    //    {
    //        currentFocus = focusInfo.collider.gameObject.name;

    //        Debug.Log(currentFocus);
    //    }

    //    //get eye data:
    //    SRanipal_Eye.get(ref eyeData);
    //    VerboseData verboseData;
    //    SRanipal_Eye.GetVerboseData(out verboseData,);
    //    eyeOpenLeft = eyeData.verbose_data.left.eye_openness;
    //    pupilDiameterLeft = eyeData.verbose_data.left.pupil_diameter_mm;
    //    pupilPositionLeft = eyeData.verbose_data.left.pupil_position_in_sensor_area;
    //}

    private void Update()
    {
        //cartof
        GetGazePoint();
        UpdateEyeDiameter();
        UpdateEyeOpen();
        UpdatePupilPosition();

        if (DebugData)
        {
            Debug.Log("pupilDiameterRight: " + pupilDiameterRight + " pupilPositionRight: " + pupilPositionRight + " eyeOpenRight: " + eyeOpenRight);
            Debug.Log("pupilDiameterLeft: " + pupilDiameterLeft + " pupilPositionLeft: " + pupilPositionLeft + " eyeOpenLeft: " + eyeOpenLeft);
        }

    }

    public void UpdateEyeDiameter()
    {
        SRanipal_Eye.GetVerboseData(out verboseData);
        //pupil diameter    pupilDiameterLeft = eyeData.verbose_data.left.pupil_diameter_mm;    
        pupilDiameterRight = verboseData.right.pupil_diameter_mm;
        pupilDiameterLeft = verboseData.left.pupil_diameter_mm;
    }

    public void UpdatePupilPosition()
    {
        SRanipal_Eye.GetVerboseData(out verboseData);
        pupilPositionRight = verboseData.right.pupil_position_in_sensor_area;
        pupilPositionLeft = verboseData.left.pupil_position_in_sensor_area;

    }

    public void UpdateEyeOpen()
    {
        SRanipal_Eye.GetVerboseData(out verboseData);
        eyeOpenRight = verboseData.right.eye_openness;
        eyeOpenLeft = verboseData.left.eye_openness;

    }

    public void GetGazePoint()
    {
        IsGazing = false;

        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
           SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

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

        foreach (GazeIndex index in GazePriority)
        {
            Ray GazeRay;
            int dart_board_layer_id = LayerMask.NameToLayer(TargetLayer);
            bool eye_focus;
            if (eye_callback_registered)
                eye_focus = SRanipal_Eye.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id), eyeData);
            else
                eye_focus = SRanipal_Eye.Focus(index, out GazeRay, out FocusInfo, 0, MaxDistance, (1 << dart_board_layer_id));

            if (eye_focus)
            {
                GazePoint =  FocusInfo.point;
                IsGazing = true;
            }
        }

    }

    private static void EyeCallback(ref EyeData eye_data)
    {
        eyeData = eye_data;
    }
}
