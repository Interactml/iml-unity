// We set opencv preprocessor directive to false
#undef ENABLE_OPENCV 

// The class will only be compiled if using opencv is true
#if ENABLE_OPENCV
// Using statements
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnityExample;

[Serializable]
public class FaceInfo
{
    public Vector3 Position;
    public double Width;

    /// <summary>
    /// Init to 0
    /// </summary>
    public FaceInfo()
    {
        Position = Vector3.zero;
        Width = 0f;
    }
}

/// <summary>
/// Data about the face being tracked
/// </summary>
public class FaceTracked : MonoBehaviour
{
        [SerializeField]
    private FaceDetectionWebCamTextureExample exampleFaceTracker;

    public FaceInfo Face;

    /// <summary>
    /// Image to draw on top of face
    /// </summary>
    public GameObject SpriteOnFace;

    [SerializeField, Range(0f, 1f)]
    private float m_SizeSprite;
    [SerializeField]
    private float m_CorrectionSpritePos;

    /// <summary>
    /// Info about the faces in Rectangular format
    /// </summary>
    private OpenCVForUnity.CoreModule.Rect[] rects;

    [SerializeField]
    private Camera m_CameraToRender;


    // Start is called before the first frame update
    void Start()
    {
        // Init face info
        Face = new FaceInfo();
        // If we don't have a reference to the camera, we use the main one
        if (m_CameraToRender == null)
            m_CameraToRender = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (exampleFaceTracker != null)
        {
            rects = exampleFaceTracker.faces.toArray();
            for (int i = 0; i < rects.Length; i++)
            {
                // Gather raw positions from webcam
                Face.Position.x = rects[i].x + (rects[i].width * 0.5f);
                Face.Position.y = rects[i].y + (rects[i].height * 0.5f);
                Face.Width = rects[i].width;
                // Convert them into world values
                Face.Position = m_CameraToRender.ScreenToWorldPoint(Face.Position);
                // Invert Y pos
                Face.Position.y *= -1;
                // Remove any extra depth and assign the quads depth
                Face.Position.z = this.transform.position.z;
                // Draw Sprite on face
                if (SpriteOnFace != null)
                {
                    SpriteOnFace.SetActive(true);
                    SpriteOnFace.transform.position = Face.Position * (1f - m_CorrectionSpritePos);
                    SpriteOnFace.transform.localScale = Vector3.one * (float) Face.Width * m_SizeSprite ;
                }
            }

            // If there are no faces...
            if (rects.Length == 0)
            {
                if (SpriteOnFace!= null)
                {
                    SpriteOnFace.SetActive(false);
                }
            }
        }
    }

}

#endif

