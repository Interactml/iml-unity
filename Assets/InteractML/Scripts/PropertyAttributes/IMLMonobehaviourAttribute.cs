using System;
using UnityEngine;

namespace InteractML
{
    /// <summary>
    /// Will draw monobehaviours for the IML interface
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class IMLMonobehaviourAttribute : PropertyAttribute
    {
        public IMLMonobehaviourAttribute()
        {
        }
    }

}
