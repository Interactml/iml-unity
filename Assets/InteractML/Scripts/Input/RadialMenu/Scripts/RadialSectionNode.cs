using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class RadialSectionNode : RadialSection
{
    public enum nodeTypes
    {
        mlsNode,
        trainingNode
    }
    public nodeTypes nodeType;
}
