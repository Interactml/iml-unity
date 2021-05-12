#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System;
using System.Threading;

public class Utilities : EditorWindow
{
    public string ChooseePath(string path)
    {
        return EditorUtility.OpenFolderPanel("Chosee Path", path, "*.*");
    }
}

public enum Options
{
    Crop = 0,
    Resize = 1
}

public enum RectOptions
{
    Center = 0,
    BottomRight = 1,
    TopRight = 2,
    BottomLeft = 3,
    TopLeft = 4,
    //Top = 5,
    //Left = 6,
    //Right = 7,
    //Bottom = 8,
    Custom = 9
}

public class ResizeTexture2D : Utilities
{
    Vector2 scrollPositionTexture = Vector2.zero;
    private Options optionsChoose;
    private RectOptions rectOptions;
    static EditorWindow window = null;
    List<string> listString = new List<string>();
    List<Texture2D> listTexture2D = new List<Texture2D>();
    private string stringFileInfo = "";
    private int widthCrop;
    private int heightCrop;
    private int widthResize = 1024, heightResize = 1024;
    private Rect RectInput;
    private int percent = 100;
    private Texture2D texture = null;
    private List<Texture2D> listTexturesOrigin = new List<Texture2D>();
    private int xMod, yMod, defaultView;

    [MenuItem("Utilities/Texture2D _F1")]
    static void Init()
    {
        window = GetWindow<ResizeTexture2D>();
    }


    void OnGUI()
    {
        EditorGUILayout.LabelField("Click 1 ( or mutil Texture or choose the folder have them if them same size) to Edit"); EditorGUILayout.LabelField("Change value Width, Height to crop");
        EditorGUILayout.LabelField("Resize return near to POT value of Texture");
        EditorGUILayout.LabelField("Edit Texture");
        optionsChoose = (Options)EditorGUILayout.EnumPopup("Choose type: ", optionsChoose);
        ChooseOptions(optionsChoose);
        if (optionsChoose == Options.Crop)
            defaultView = EditorGUILayout.IntSlider("Size View: ", defaultView, 200, 1000);
        scrollPositionTexture = EditorGUILayout.BeginScrollView(scrollPositionTexture, GUILayout.Width(position.width), GUILayout.Height(600));
        GUILayout.TextArea(stringFileInfo);
        if (optionsChoose == Options.Crop)
        {
            for (int i = 0; i < listTexture2D.Count; i++)
            {
                texture = listTexture2D[i];
                if (texture != null)
                {
                    DrawTexture(listTexturesOrigin[i], i, 25, ": Origin size: ");
                    DrawTexture(texture, i, defaultView + 55, "Preview size: ");
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    void Review()
    {
        listTexture2D.Clear();
        listString.Clear();
        var selectedObjects = Selection.GetFiltered(typeof(UnityEngine.Texture2D), SelectionMode.DeepAssets);
        if (selectedObjects != null)
        {
            if (optionsChoose == Options.Crop)
            {
                if ((widthCrop == 0 || heightCrop == 0))
                {
                    return;
                }
            }
            else
            {
                if ((widthResize == 0 || heightResize == 0))
                {
                    return;
                }
            }
            for (int index = 0; index < selectedObjects.Length; index++)
            {
                Texture2D newTexture2D = selectedObjects[index] as Texture2D;
                listString.Add(AssetDatabase.GetAssetPath(selectedObjects[index]));
                if (newTexture2D != null)
                {

                    if (optionsChoose == Options.Crop)
                    {
                        Texture2D newTexture = null;
                        newTexture = new Texture2D(newTexture2D.width, newTexture2D.height, newTexture2D.format, false);
                        newTexture.LoadRawTextureData(newTexture2D.GetRawTextureData());
                        newTexture.name = newTexture2D.name;
                        newTexture.Apply();
                        listTexture2D.Add(newTexture);
                    }
                    else
                    {
                        Texture2D newTexture = null;
                        byte[] bytes = File.ReadAllBytes(AssetDatabase.GetAssetPath(selectedObjects[index]));
                        newTexture = new Texture2D(newTexture2D.width, newTexture2D.height, TextureFormat.RGBA32, false);
                        newTexture.LoadImage(bytes);
                        newTexture.name = newTexture2D.name;
                        newTexture.Apply();
                        listTexture2D.Add(newTexture);
                    }
                }
            }
            LoadInfoFile();
        }
    }

    void DrawTexture(Texture2D texture, int index, int posX, string LabelTitle)
    {
        float ratio = (float)texture.width / texture.height;
        float widthView = texture.width < defaultView ? texture.width : defaultView;
        float heigthView = widthView / ratio > defaultView ? defaultView : widthView / ratio;
        widthView = heigthView >= defaultView ? heigthView * ratio : widthView;
        float posLabelY = 35 + index * (defaultView + 25);
        float posTextureDrawY = posLabelY + 20;

        Rect rect = GUILayoutUtility.GetRect(position.width, position.width);
        rect = new Rect(posX, posTextureDrawY, widthView, heigthView);

        EditorGUI.LabelField(new Rect(posX, posLabelY, position.width / 2, 25), texture.name + LabelTitle + texture.width + "x" + texture.height);

        EditorGUI.DrawTextureTransparent(rect, texture);
    }

    void ChooseOptions(Options options)
    {

        switch (options)
        {
            case Options.Resize:
                widthResize = EditorGUILayout.IntField("Width:", widthResize);
                heightResize = EditorGUILayout.IntField("Height:", heightResize);
                EditorGUILayout.Space();
                if (listTexturesOrigin.Count > 0)
                {
                    widthResize = widthResize <= 0 ? 0 : widthResize;
                    heightResize = heightResize <= 0 ? 0 : heightResize;
                }
                int oldwidthResize = 0;
                int oldheightResize = 0;
                if (oldwidthResize != widthResize || oldheightResize != heightResize)
                {
                    Review();
                }
                oldheightResize = heightResize;
                oldwidthResize = widthResize;
                if (GUILayout.Button("Resize Texture2D"))
                {
                    ResizeTexture();
                }
                break;

            case Options.Crop:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.EndDisabledGroup();
                rectOptions = (RectOptions)EditorGUILayout.EnumPopup("Position", rectOptions);
                widthCrop = EditorGUILayout.IntField("   Width: ", widthCrop);
                heightCrop = EditorGUILayout.IntField("   Height: ", heightCrop);
                if (listTexturesOrigin.Count > 0)
                {
                    widthCrop = listTexturesOrigin[0].width < widthCrop ? listTexturesOrigin[0].width : widthCrop <= 0 ? 0 : widthCrop;
                    heightCrop = listTexturesOrigin[0].height < heightCrop ? listTexturesOrigin[0].height : heightCrop <= 0 ? 0 : heightCrop;
                }
                int oldwidthCrop = 0;
                int oldheightCrop = 0;
                if (oldwidthCrop != widthCrop || oldheightCrop != heightCrop)
                {
                    Review();
                }
                oldheightCrop = heightCrop;
                oldwidthCrop = widthCrop;
                if (rectOptions == RectOptions.Custom)
                {
                    xMod = EditorGUILayout.IntField("   xMod: ", xMod);
                    yMod = EditorGUILayout.IntField("   yMod: ", yMod);
                    if (listTexturesOrigin.Count > 0)
                    {
                        int tempxMod = listTexturesOrigin[0].width - widthCrop;
                        int tempyMod = listTexturesOrigin[0].height - heightCrop;
                        xMod = xMod <= 0 ? 0 : xMod >= tempxMod ? tempxMod : xMod;
                        yMod = yMod <= 0 ? 0 : yMod >= tempyMod ? tempyMod : yMod;
                    }
                    int oldxMod = 0;
                    int oldyMod = 0;
                    if (oldxMod != xMod || oldyMod != yMod)
                    {
                        Review();
                    }
                    oldxMod = xMod;
                    oldyMod = yMod;
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Crop Texture"))
                {
                    CropTexture(rectOptions);
                }
                break;
        }

    }

    private void ResizeTexture()
    {
        Debug.Log("resize");
        for (int index = 0; index < listTexture2D.Count; index++)
        {
            Debug.Log(listString[index].Replace(Application.dataPath, "Assets"));
            TextureScale.Bilinear(listTexture2D[index], widthResize, heightResize);
            byte[] bytes = listTexture2D[index].EncodeToPNG();
            File.WriteAllBytes(listString[index].Replace(Application.dataPath, "Assets"), bytes);
        }
        AssetDatabase.Refresh();
    }

    void CropTexture(RectOptions rectOptions)
    {
        Debug.Log("crop");
        for (int i = 0; i < listTexture2D.Count; i++)
        {
            Debug.Log(listString[i].Replace(Application.dataPath, "Assets"));
            RectInput.width = widthCrop;
            RectInput.height = heightCrop;

            listTexture2D[i] = CropWithRect(rectOptions, listTexture2D[i], RectInput, xMod, yMod);
            byte[] bytes = listTexture2D[i].EncodeToPNG();
            File.WriteAllBytes(listString[i].Replace(Application.dataPath, "Assets"), bytes);
        }
        AssetDatabase.Refresh();
    }

    private static Texture2D CropWithRect(RectOptions rectOptions, Texture2D texture, Rect r, int xMod, int yMod)
    {
        if (r.height < 0 || r.width < 0)
        {
            return texture;
        }
        Texture2D result = new Texture2D((int)r.width, (int)r.height);
        if (r.width != 0 && r.height != 0)
        {
            float xRect = r.x;
            float yRect = r.y;
            float widthRect = r.width;
            float heightRect = r.height;

            switch (rectOptions)
            {
                case RectOptions.Center:
                    xRect = (texture.width - r.width) / 2;
                    yRect = (texture.height - r.height) / 2;
                    break;

                case RectOptions.BottomRight:
                    xRect = texture.width - r.width;
                    break;

                case RectOptions.BottomLeft:
                    break;

                case RectOptions.TopLeft:
                    yRect = texture.height - r.height;
                    break;

                case RectOptions.TopRight:
                    xRect = texture.width - r.width;
                    yRect = texture.height - r.height;
                    break;

                case RectOptions.Custom:
                    float tempWidth = texture.width - r.width - xMod;
                    float tempHeight = texture.height - r.height - yMod;
                    xRect = tempWidth > texture.width ? 0 : tempWidth;
                    yRect = tempHeight > texture.height ? 0 : tempHeight;
                    break;

            }

            if (texture.width < r.x + r.width || texture.height < r.y + r.height || xRect > r.x + texture.width || yRect > r.y + texture.height || xRect < 0 || yRect < 0 || r.width < 0 || r.height < 0)
            {
                //EditorUtility.DisplayDialog("Set value crop", "Set value crop (Width and Height > 0) less than origin texture size \n" + texture.name + " wrong size", "ReSet");
                return texture;
            }
            result.SetPixels(texture.GetPixels(Mathf.FloorToInt(xRect), Mathf.FloorToInt(yRect), Mathf.FloorToInt(widthRect), Mathf.FloorToInt(heightRect)));
            result.Apply();
        }
        return result;
    }

    private void LoadInfoFile()
    {
        listTexturesOrigin.Clear();
        listTexturesOrigin.AddRange(listTexture2D);
        stringFileInfo = "Total files: " + listTexture2D.Count + "\n";
        for (int i = 0; i < listTexture2D.Count; i++)
        {
            switch (optionsChoose)
            {
                case Options.Crop:

                    RectInput.width = widthCrop;
                    RectInput.height = heightCrop;
                    listTexture2D[i] = CropWithRect(rectOptions, listTexture2D[i], RectInput, xMod, yMod);
                    break;

                case Options.Resize:
                    //TextureScale.Bilinear(listTexture2D[i], widthResize, heightResize);
                    break;
            }
        }
    }

}



public class TextureScale
{
    public class ThreadData
    {
        public int start;
        public int end;
        public ThreadData(int s, int e)
        {
            start = s;
            end = e;
        }
    }

    private static Color[] texColors;
    private static Color[] newColors;
    private static int w;
    private static float ratioX;
    private static float ratioY;
    private static int w2;
    private static int finishCount;
    private static Mutex mutex;

    public static void Point(Texture2D tex, int newWidth, int newHeight)
    {
        ThreadedScale(tex, newWidth, newHeight, false);
    }

    public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
    {
        ThreadedScale(tex, newWidth, newHeight, true);
    }

    private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
    {
        texColors = tex.GetPixels();
        newColors = new Color[newWidth * newHeight];
        if (useBilinear)
        {
            ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
            ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
        }
        else
        {
            ratioX = ((float)tex.width) / newWidth;
            ratioY = ((float)tex.height) / newHeight;
        }
        w = tex.width;
        w2 = newWidth;
        var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
        var slice = newHeight / cores;

        finishCount = 0;
        if (mutex == null)
        {
            mutex = new Mutex(false);
        }
        if (cores > 1)
        {
            int i = 0;
            ThreadData threadData;
            for (i = 0; i < cores - 1; i++)
            {
                threadData = new ThreadData(slice * i, slice * (i + 1));
                ParameterizedThreadStart ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
                Thread thread = new Thread(ts);
                thread.Start(threadData);
            }
            threadData = new ThreadData(slice * i, newHeight);
            if (useBilinear)
            {
                BilinearScale(threadData);
            }
            else
            {
                PointScale(threadData);
            }
            while (finishCount < cores)
            {
                Thread.Sleep(1);
            }
        }
        else
        {
            ThreadData threadData = new ThreadData(0, newHeight);
            if (useBilinear)
            {
                BilinearScale(threadData);
            }
            else
            {
                PointScale(threadData);
            }
        }

        tex.Resize(newWidth, newHeight);
        tex.SetPixels(newColors);
        tex.Apply();

        texColors = null;
        newColors = null;
    }

    public static void BilinearScale(System.Object obj)
    {
        ThreadData threadData = (ThreadData)obj;
        for (var y = threadData.start; y < threadData.end; y++)
        {
            int yFloor = (int)Mathf.Floor(y * ratioY);
            var y1 = yFloor * w;
            var y2 = (yFloor + 1) * w;
            var yw = y * w2;

            for (var x = 0; x < w2; x++)
            {
                int xFloor = (int)Mathf.Floor(x * ratioX);
                var xLerp = x * ratioX - xFloor;
                newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp),
                                                       ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp),
                                                       y * ratioY - yFloor);
            }
        }

        mutex.WaitOne();
        finishCount++;
        mutex.ReleaseMutex();
    }

    public static void PointScale(System.Object obj)
    {
        ThreadData threadData = (ThreadData)obj;
        for (var y = threadData.start; y < threadData.end; y++)
        {
            var thisY = (int)(ratioY * y) * w;
            var yw = y * w2;
            for (var x = 0; x < w2; x++)
            {
                newColors[yw + x] = texColors[(int)(thisY + ratioX * x)];
            }
        }

        mutex.WaitOne();
        finishCount++;
        mutex.ReleaseMutex();
    }

    private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
    {
        return new Color(c1.r + (c2.r - c1.r) * value,
                          c1.g + (c2.g - c1.g) * value,
                          c1.b + (c2.b - c1.b) * value,
                          c1.a + (c2.a - c1.a) * value);
    }
}

public class TextureTools
{

    public static Texture2D ResampleAndCrop(Texture2D source, int targetWidth, int targetHeight)
    {
        int sourceWidth = source.width;
        int sourceHeight = source.height;
        float sourceAspect = (float)sourceWidth / sourceHeight;
        float targetAspect = (float)targetWidth / targetHeight;
        int xOffset = 0;
        int yOffset = 0;
        float factor = 1;
        if (sourceAspect > targetAspect)
        { // crop width
            factor = (float)targetHeight / sourceHeight;
            xOffset = (int)((sourceWidth - sourceHeight * targetAspect) * 0.5f);
        }
        else
        { // crop height
            factor = (float)targetWidth / sourceWidth;
            yOffset = (int)((sourceHeight - sourceWidth / targetAspect) * 0.5f);
        }
        Color32[] data = source.GetPixels32();
        Color32[] data2 = new Color32[targetWidth * targetHeight];
        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                float xPos = xOffset + x / factor;
                float yPos = yOffset + y / factor;
                if (xPos > sourceWidth - 1)
                    xPos = sourceWidth - 1;
                if (yPos > sourceHeight - 1)
                    yPos = sourceHeight - 1;
                int x1 = (int)xPos; int x2 = x1 + 1;
                int y1 = (int)yPos; int y2 = y1 + 1;
                float fx = xPos - x1;
                float fy = yPos - y1;
                y1 *= sourceWidth;
                y2 *= sourceWidth;
                var c11 = data[x1 + y1];
                var c12 = data[x1 + y2];
                var c21 = data[x2 + y1];
                var c22 = data[x2 + y2];
                float f11 = (1 - fx) * (1 - fy);
                float f12 = (1 - fx) * fy;
                float f21 = fx * (1 - fy);
                float f22 = fx * fy;
                float r = c11.r * f11 + c12.r * f12 + c21.r * f21 + c22.r * f22;
                float g = c11.g * f11 + c12.g * f12 + c21.g * f21 + c22.g * f22;
                float b = c11.b * f11 + c12.b * f12 + c21.b * f21 + c22.b * f22;
                float a = c11.a * f11 + c12.a * f12 + c21.a * f21 + c22.a * f22;
                int index = x + y * targetWidth;

                data2[index].r = (byte)r;
                data2[index].g = (byte)g;
                data2[index].b = (byte)b;
                data2[index].a = (byte)a;
            }
        }

        var tex = new Texture2D(targetWidth, targetHeight);
        tex.SetPixels32(data2);
        tex.Apply(true);
        return tex;
    }
}

#endif