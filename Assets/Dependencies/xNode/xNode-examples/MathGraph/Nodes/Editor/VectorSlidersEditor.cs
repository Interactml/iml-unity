using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNodeEditor;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CustomNodeEditor(typeof(VectorSliders))]
public class VectorSlidersEditor : NodeEditor
{
    /// <summary> Called whenever the xNode editor window is updated </summary>
    public override void OnBodyGUI()
    {   
        // Draw the default GUI first, so we don't have to do all of that manually.
        base.OnBodyGUI();

        // `target` points to the node, but it is of type `Node`, so cast it.
        VectorSliders vectorSlidersNode = target as VectorSliders;

        // Show sliders for each input
        vectorSlidersNode.x = EditorGUILayout.Slider(vectorSlidersNode.x, vectorSlidersNode.RangeSliders.x, vectorSlidersNode.RangeSliders.y);
        vectorSlidersNode.y = EditorGUILayout.Slider(vectorSlidersNode.y, vectorSlidersNode.RangeSliders.x, vectorSlidersNode.RangeSliders.y);
        vectorSlidersNode.z = EditorGUILayout.Slider(vectorSlidersNode.z, vectorSlidersNode.RangeSliders.x, vectorSlidersNode.RangeSliders.y);
    }

}
