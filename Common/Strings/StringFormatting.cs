using System.Text;
using System.Text.RegularExpressions;

namespace Common.Strings
{
    public static class StringFormatting
    {
        public static string GetPrimaryKeyName<T>() where T : class
        {
            return string.Concat(UnCamelCaseToUnderscores(typeof(T).ToString()).ToLower(), "_id");
        }

        public static int NthIndexOf(this string target, string value, int n)
        {
            var m = Regex.Match(target, "((" + value + ").*?){" + n + "}");
            if (m.Success)
                return m.Groups[2].Captures[n - 1].Index;
            else
                return -1;
        }

        public static string UnCamelCaseToUnderscores(this string value)
        {
            return uncamelCase(value, "_");
        }

        public static string UnCamelCase(this string value)
        {
            return uncamelCase(value, " ");
        }

        private static string uncamelCase(string value, string replacementString)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var sb = new StringBuilder(value.Length * 4);//todo: this is a guess.
            var previousIsSpace = false;
            foreach (char c in value)
            {
                if (char.IsLower(c) || c == ' ')
                {
                    if (c == ' ')
                    {
                        previousIsSpace = true;
                    }
                    sb.Append(c);
                }
                else
                {
                    if (previousIsSpace)
                    {
                        sb.Append(c);
                    }
                    else
                    {
                        sb.Append(replacementString + c);
                    }
                    previousIsSpace = false;
                }
            }
            return sb.ToString().Trim();
        }
    }
}