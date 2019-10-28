using System;

namespace MalbersAnimations
{
    public class MinMaxRangeAttribute : Attribute
    {
        public MinMaxRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float Min { get; private set; }
        public float Max { get; private set; }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public sealed class LineAttribute : Attribute
    {
        public readonly float height;

        public LineAttribute()
        {
            // By default uses 8 pixels which corresponds to EditorGUILayout.Space()
            // which reserves 6 pixels, plus the usual 2 pixels caused by the neighboring margin.
            // (Why not 2 pixels for margin both below and above?
            // Because one of those is already accounted for when the space is not there.)
            this.height = 8;
        }

        public LineAttribute(float height)
        {
            this.height = height;
        }
    }



 
}