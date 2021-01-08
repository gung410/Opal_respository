using System.Linq;

namespace LearnerApp.Common.Helper
{
    public static class TransformSearchSpecialCharacters
    {
        private static char[] SpecialCharacters => new char[]
        {
            '\\', '/', '^', '~', '*', '?', ':',
            '+', '-', '=', '&', '|', '>', '<',
            '!', '(', ')', '{', '}', '[', ']',
            '"'
        };

        public static string TransformSpecialCharacterToApiSearchString(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return searchString;
            }

            searchString = searchString.Replace("“", "\"").Replace("”", "\"").Trim();
            searchString = ReplaceSpecialCharacterWithSlash(searchString, SpecialCharacters);

            return searchString;
        }

        private static string ReplaceSpecialCharacterWithSlash(string str, char[] specialCharacterSet)
        {
            if (str.First() == '\"' && str.Last() == '\"')
            {
                return str;
            }

            foreach (var character in specialCharacterSet)
            {
                str = str.Replace(character.ToString(), $"\\{character}");
            }

            return str;
        }
    }
}
