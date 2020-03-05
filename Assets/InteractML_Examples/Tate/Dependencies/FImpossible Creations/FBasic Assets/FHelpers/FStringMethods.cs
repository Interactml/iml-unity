/// <summary>
/// FM: Class which contains many helpful methods which operates 
/// </summary>
public static class FStringMethods
{
    /// <summary>
    /// Converting int to strig
    /// </summary>
    /// <param name="value">value to show</param>
    /// <param name="signs">How many signs, value = 8, signs = 3 -> 008</param>
    public static string IntToString(int value, int signs)
    {
        string output = value.ToString();

        int missingZeros = signs - output.Length;

        if (missingZeros > 0)
        {
            string missing = "0";

            for (int i = 1; i < missingZeros; i++)
            {
                missing += 0;
            }

            output = missing + output;
        }

        return output;
    }

    /// <summary>
    /// Chanigng first letter to uppercase and rest to lowercase
    /// </summary>
    public static string CapitalizeOnlyFirstLetter(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        return text[0].ToString().ToUpper() + (text.Length > 1 ? text.Substring(1) : "");
    }

    /// <summary>
    /// Chanigng first letter to uppercase and rest without changes
    /// </summary>
    public static string CapitalizeFirstLetter(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        return text[0].ToString().ToUpper() + text.Substring(1);
    }

    /// <summary>
    /// Changing space signs to underline signs
    /// </summary>
    public static string ReplaceSpacesWithUnderline(string text)
    {
        if (text.Contains(" "))
        {
            text = text.Replace(" ", "_");
        }

        return text;
    }

    /// <summary>
    /// Returning string from end to the separator, for GetEndOfStringFromSeparator("ask/ddd/aaa", new char[] { '\\', '/' }) will return "aaa"
    /// </summary>
    /// <param name="source"> Base string, from which you need only part of it </param>
    /// <param name="separators"> separators array to define, when stop checking new char[] { '\\', '/' } </param>
    /// <param name="which"> how many occurences of separator should happen </param>
    /// <param name="fromEnd"> start checking from end or from start of base string </param>
    /// <returns></returns>
    public static string GetEndOfStringFromSeparator(string source, char[] separators, int which = 1, bool fromEnd = false)
    {
        bool separated = false;
        int counter = 0;
        int steps = 0;

        int i = 0;

        for (i = source.Length - 1; i >= 0; i--)
        {
            steps++;

            for (int c = 0; c < separators.Length; c++)
            {
                if (source[i] == separators[c])
                {
                    counter++;

                    if (counter == which)
                    {
                        i++;

                        separated = true;
                        break;
                    }
                }
            }

            if (separated) break;
        }

        if (separated)
        {
            if (!fromEnd)
            {
                return source.Substring(0, source.Length - (steps));
            }
            else
            {
                return source.Substring(i, source.Length - i);
            }
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// Same as above GetEndOfStringFromSeparator() but here you define separators as strings, so for GetEndOfStringFromStringSeparator("ask/ddd/aaa", new string[] { "ask" }) will return "/ddd/aaa"
    /// </summary>
    public static string GetEndOfStringFromStringSeparator(string source, string[] separators, int which = 1, bool rest = false)
    {
        bool separated = false;
        int counter = 0;
        int steps = 0;

        int i = 0;
        for (i = 0; i < source.Length; i++)
        {
            steps++;

            for (int c = 0; c < separators.Length; c++)
            {
                if (i + separators[c].Length > source.Length) break;

                if (source.Substring(i, separators[c].Length) == separators[c])
                {
                    counter++;

                    if (counter == which)
                    {
                        i++;

                        i += separators[c].Length - 1;

                        separated = true;
                        break;
                    }
                }
            }

            if (separated) break;
        }

        if (separated)
        {
            if (rest)
            {
                return source.Substring(0, source.Length - (steps));
            }
            else
            {
                return source.Substring(i, source.Length - i);
            }
        }
        else
        {
            return "";
        }
    }
}
