using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(DirtyLensFlare))] 
public class DirtyLensFlareEditor : Editor  {
	
	SerializedObject   serObj;
	SerializedProperty lensFlareType;
	SerializedProperty useDirt;
	SerializedProperty saturation;
	SerializedProperty flareIntensity;
	SerializedProperty bloomIntensity;
	SerializedProperty threshold;
	SerializedProperty blurSpread;
	SerializedProperty blurIterations;
	SerializedProperty dirtTexture;
	SerializedProperty downsample;
	
	GUIStyle style;
	
	void OnEnable()
	{
		serObj         = new SerializedObject (target);
		lensFlareType  = serObj.FindProperty("lensFlareType");
		useDirt        = serObj.FindProperty("useDirt");
		saturation     = serObj.FindProperty("saturation");
		flareIntensity = serObj.FindProperty("flareIntensity");
		bloomIntensity = serObj.FindProperty("bloomIntensity");
		threshold      = serObj.FindProperty("threshold");
		
		blurIterations = serObj.FindProperty("iterations");
		blurSpread     = serObj.FindProperty("blurSpread");
		
		downsample     = serObj.FindProperty("downsample");
		
		dirtTexture    = serObj.FindProperty("screenDirt");
		
	}
	
	public override void OnInspectorGUI () {
        
		serObj.Update();
		
		EditorGUILayout.PropertyField (lensFlareType, new GUIContent("Lens flare type"));
		
		
		threshold.floatValue     = EditorGUILayout.Slider ("Threshold", threshold.floatValue, 0.0f, 1.0f);
		
		if( lensFlareType.enumValueIndex == 0 || lensFlareType.enumValueIndex == 2 )
		{
			saturation.floatValue     = EditorGUILayout.Slider ("Flare saturation", saturation.floatValue,     -2.0f, 2.0f);
			flareIntensity.floatValue = EditorGUILayout.Slider ("Flare intensity",  flareIntensity.floatValue,  0.0f, 10.0f);
			if( lensFlareType.enumValueIndex == 0 )
				bloomIntensity.floatValue = EditorGUILayout.Slider ("Bloom intensity",  bloomIntensity.floatValue,  0.0f, 10.0f);
		}
		else
		{
			bloomIntensity.floatValue = EditorGUILayout.Slider ("Bloom intensity",  bloomIntensity.floatValue,  0.0f, 10.0f);
		}
		
		EditorGUILayout.Separator ();
		
		blurSpread.floatValue = EditorGUILayout.Slider ("Blur spread",      blurSpread.floatValue,  0.0f, 2.0f);
		blurIterations.intValue   = EditorGUILayout.IntSlider ("Blur iterations",  blurIterations.intValue,    0, 64);
		
		EditorGUILayout.Separator ();
		
		downsample.intValue = EditorGUILayout.IntField("Downsample", downsample.intValue);
		downsample.intValue = Mathf.Clamp(downsample.intValue, 1, 12);
		
		EditorGUILayout.Separator ();
		
		EditorGUILayout.PropertyField (useDirt,       new GUIContent("Use dirt"));
		EditorGUILayout.PropertyField (dirtTexture, new GUIContent("Screen dirt texture"));
		
		serObj.ApplyModifiedProperties();
		
    }
	
}
