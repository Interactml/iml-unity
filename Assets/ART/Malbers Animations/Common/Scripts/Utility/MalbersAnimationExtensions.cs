using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public static class MalbersAnimationsExtensions
{
    /// <summary>
    /// Find the first transform grandchild with this name inside this transform
    /// </summary>
    public static Transform FindGrandChild(this Transform aParent, string aName)
    {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindGrandChild(aName);
            if (result != null)
                return result;
        }
        return null;
    }

    /// <summary>
    /// returns the delta position from a rotation.
    /// </summary>
    public static Vector3 DeltaPositionFromRotate(this Transform transform, Vector3 point, Vector3 axis, float deltaAngle)
    {
        var pos = transform.position;
        var direction = pos - point;
        var rotation = Quaternion.AngleAxis(deltaAngle, axis);
        direction = rotation * direction;

        pos = point + direction - pos;
        pos.y = 0;                                                      //the Y is handled by the Fix Position method

        return pos;
    }
    /// <summary>
    /// Invoke with Parameters
    /// </summary>
    public static void InvokeWithParams(this MonoBehaviour sender, string method, object args)
    {
        Type argType = null;
        if (args != null)
        {
            argType = args.GetType();
        }
        MethodInfo methodPtr = null;

      

        if (argType != null)
        {
            methodPtr = sender.GetType().GetMethod(method, new Type[] { argType });
        }
        else
        {
            methodPtr = sender.GetType().GetMethod(method);
        }

        if (methodPtr != null)
        {
            if (args != null)
            {
                var arguments = new object[1] { args };
                methodPtr.Invoke(sender, arguments);
            }
            else
            {
                methodPtr.Invoke(sender, null);
            }
            return;
        }

        PropertyInfo property = sender.GetType().GetProperty(method);

        if (property != null)
        {
            property.SetValue(sender, args, null);
        }

    }


    /// <summary>
    /// Invoke with Parameters and Delay
    /// </summary>
    public static void InvokeDelay(this MonoBehaviour behaviour, string method, object options, YieldInstruction wait)
    {
        behaviour.StartCoroutine(_invoke(behaviour, method, wait, options));
    }

    private static IEnumerator _invoke(this MonoBehaviour behaviour, string method, YieldInstruction wait, object options)
    {
        yield return wait;

        Type instance = behaviour.GetType();
        MethodInfo mthd = instance.GetMethod(method);
        mthd.Invoke(behaviour, new object[] { options });

        yield return null;
    }


    /// <summary>
    /// Invoke with Parameters
    /// </summary>
    public static void Invoke(this ScriptableObject sender, string method, object args)
    {
        var methodPtr = sender.GetType().GetMethod(method);

        if (methodPtr != null)
        {
            if (args != null)
            {
                var arguments = new object[1] { args };
                methodPtr.Invoke(sender, arguments);
            }
            else
            {
                methodPtr.Invoke(sender, null);
            }
        }
    }


    public static void SetLayer(this GameObject parent, int layer, bool includeChildren = true)
    {
        parent.layer = layer;
        if (includeChildren)
        {
            foreach (Transform trans in parent.transform.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layer;
            }
        }
    }


    //public static void ToggleMultiplayerSymbol()
    //{
    //    var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
    //    if (symbols.Contains(s_MultiplayerSymbol + ";"))
    //    {
    //        symbols = symbols.Replace(s_MultiplayerSymbol + ";", "");
    //    }
    //    else if (symbols.Contains(s_MultiplayerSymbol))
    //    {
    //        symbols = symbols.Replace(s_MultiplayerSymbol, "");
    //    }
    //    else
    //    {
    //        symbols += (";" + s_MultiplayerSymbol);
    //    }
    //    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
    //    AssetDatabase.Refresh();
    //}
}