using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReusableMethods;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif
namespace InteractML
{
    [CustomNodeEditor(typeof(MLSystem))]
    public class MLSystemEditor : IMLNodeEditor
    {
       
    }

}
