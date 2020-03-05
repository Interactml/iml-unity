using UnityEngine;

/// <summary>
/// FMoeglich: Class with methods which can be helpful in using Unity Console.
/// Recommending to use some console extensions like Console Enchanced or other.
/// </summary>
public static class FDebug
{
    public static void Log(string log)
    {
        Debug.Log("LOG: " + log);
    }

    public static void Log(string log, string category)
    {
        Debug.Log(MarkerColor("#1A6600") + "[" + category + "]" + EndColorMarker() + " " + log);
    }

    public static void LogRed(string log)
    {
        Debug.Log(MarkerColor("red") + log + EndColorMarker());
    }

    public static void LogOrange(string log)
    {
        Debug.Log(MarkerColor("#D1681D") + log + EndColorMarker());
    }

    public static void LogYellow(string log)
    {
        Debug.Log(MarkerColor("#E0D300") + log + EndColorMarker());
    }

    /// <summary>
    /// Rich text marker for color
    /// </summary>
    public static string MarkerColor(string color)
    {
        return "<color='" + color + "'>";
    }

    /// <summary>
    /// close rich text marker for color
    /// </summary>
    public static string EndColorMarker()
    {
        return "</color>";
    }
}
