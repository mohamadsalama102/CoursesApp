using System.Text;

namespace nagiashraf.CoursesApp.Services.Catalog.Core.Extensions;

public static class StringExtensions
{
    public static string RemoveExtraWhiteSpaces(this string str)
    {
        str = str.Trim();
        var stringBuilder = new StringBuilder();
        for (int i = 0; i < str.Length; i++)
        {
            if (char.IsWhiteSpace(str[i]) && char.IsWhiteSpace(str[i + 1]))
                continue;

            stringBuilder.Append(str[i]);
        }
        return stringBuilder.ToString();
    }

    public static int GetWordsCount(this string str)
    {
        int count = 0;
        bool isWord = false;

        for (int i = 0; i < str.Length; i++)
        {
            if (!isWord && char.IsLetterOrDigit(str[i]))
            {
                isWord = true;
                count++;
            }
            else if (!char.IsLetterOrDigit(str[i]))
            {
                isWord = false;
            }
        }

        return count;
    }

    public static List<string> ToListBySeparator(this string str, string separator = "^^SEPARATOR^^")
    {
        return str.Split(separator).ToList();
    }
}
