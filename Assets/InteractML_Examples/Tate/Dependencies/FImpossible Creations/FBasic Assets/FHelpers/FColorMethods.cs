using UnityEngine;

/// <summary>
/// FMoeglich: Class with methods which can be helpful when using Colors
/// </summary>
public static class FColorMethods
{
    /// <summary>
    /// Changing color's alpha value
    /// </summary>
    public static Color ChangeColorAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    /// <summary>
    /// Changing color brightness or darkness value (adding / subliming .r .g .b) -1 to 1 value
    /// </summary>
    public static Color ChangeColorsValue(Color color, float brightenOrDarken = 0f)
    {
        return new Color(color.r + brightenOrDarken, color.g + brightenOrDarken, color.b + brightenOrDarken, color.a);
    }

    /// <summary>
    /// Converting colors from hexadecimal to rgba color
    /// </summary>
    public static Color32 HexToColor(string hex)
    {
        if (string.IsNullOrEmpty(hex))
        {
            FDebug.LogRed("Trying convert from hex to color empty string!");
            return Color.white;
        }

        uint rgba = 0x000000FF;

        hex = hex.Replace("#", "");
        hex = hex.Replace("0x", "");

        if (! uint.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out rgba))
        {
            Debug.Log("Error during converting hex string.");
            return Color.white;
        }

        return new Color32((byte)((rgba & -16777216) >> 0x18),
                              (byte)((rgba & 0xff0000) >> 0x10),
                              (byte)((rgba & 0xff00) >> 8),
                              (byte)(rgba & 0xff));
    }

    /// <summary>
    /// Coverting color32 to hex string
    /// </summary>
    public static string ColorToHex(Color32 color, bool addHash = true)
    {
        string hex = "";

        if (addHash) hex = "#";

        hex += System.String.Format("{0}{1}{2}{3}"
           , color.r.ToString("X").Length == 1 ? System.String.Format("0{0}", color.r.ToString("X")) : color.r.ToString("X")
           , color.g.ToString("X").Length == 1 ? System.String.Format("0{0}", color.g.ToString("X")) : color.g.ToString("X")
           , color.b.ToString("X").Length == 1 ? System.String.Format("0{0}", color.b.ToString("X")) : color.b.ToString("X")
           , color.a.ToString("X").Length == 1 ? System.String.Format("0{0}", color.a.ToString("X")) : color.a.ToString("X"));

        return hex;
    }

    /// <summary>
    /// Coverting color to hex string
    /// </summary>
    public static string ColorToHex(Color color, bool addHash = true)
    {
        Color32 col32 = new Color32((byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255), (byte)(color.a * 255));
        return ColorToHex(col32, addHash);
    }

    /// <summary>
    /// Doing linear interpolation with deltaTime to change material color smoothly
    /// </summary>
    public static void LerpMaterialColor(Material mat, string property, Color targetColor, float deltaMultiplier = 8f)
    {
        if (mat == null) return;

        if (!mat.HasProperty(property))
        {
            Debug.LogError("Material " + mat.name + " don't have property '" + property + "' " + " in shader " + mat.shader.name);
            return;
        }

        Color currentColor = mat.GetColor(property);
        mat.SetColor(property, Color.Lerp(currentColor, targetColor, Time.deltaTime * deltaMultiplier));
    }
}
