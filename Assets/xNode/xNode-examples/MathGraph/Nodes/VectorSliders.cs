using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using InteractML;

public class VectorSliders : Node {

    public Vector2 RangeSliders;

    [Input]
    public float x, y, z;
    [Output] public Vector3 vector;

    public override object GetValue(XNode.NodePort port)
    {
        vector.x = GetInputValue<float>("x", this.x);
        vector.y = GetInputValue<float>("y", this.y);
        vector.z = GetInputValue<float>("z", this.z);
        return vector;
    }
}