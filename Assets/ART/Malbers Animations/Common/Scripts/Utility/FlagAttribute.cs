using UnityEngine;

namespace MalbersAnimations.Utilities
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class FlagAttribute : PropertyAttribute
    {
        public string enumName;

        public FlagAttribute() { }

        public FlagAttribute(string name)
        {
            enumName = name;
        }
    }
}