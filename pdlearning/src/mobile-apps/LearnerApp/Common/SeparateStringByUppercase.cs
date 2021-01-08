using System.Text.RegularExpressions;

namespace LearnerApp.Common
{
    public static class SeparateStringByUppercase
    {
        public static string Convert(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : Regex.Replace(value, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }
    }
}
