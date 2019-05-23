using System.Text.RegularExpressions;

namespace Common.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string x)
        {
            return string.IsNullOrEmpty(x);
        }
    }
}