using Newtonsoft.Json.Serialization;

namespace Communication.Business.Models
{
    public class CustomNamingStrategy : NamingStrategy
    {
        public CustomNamingStrategy()
        {
            ProcessDictionaryKeys = true;
            ProcessExtensionDataNames = true;
        }
        protected override string ResolvePropertyName(string name)
        {
            //we will keep long name as it is
            return name.Length < 36 ? ToPascalCase(name) : name;
        }
        private string ToPascalCase(string s)
        {
            if (string.IsNullOrEmpty(s) || char.IsUpper(s[0]))
            {
                return s;
            }

            char[] chars = s.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 1 && char.IsLower(chars[i]))
                {
                    break;
                }
                bool hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    // if the next character is a space, which is not considered upperCase 
                    // (otherwise we wouldn't be here...)
                    // we want to ensure that the following:
                    // 'fOO bar' is rewritten as 'foo bar', and not as 'FoO bar'
                    // The code was written in such a way that the first word in uppercase
                    // ends when if finds an uppercase letter followed by a lowercase letter.
                    // now a ' ' (space, (char)32) is considered not upper
                    // but in that case we still want our current character to become lowercase
                    if (char.IsSeparator(chars[i + 1]))
                    {
                        chars[i] = char.ToLowerInvariant(chars[i]);
                    }

                    break;
                }

                chars[i] = char.ToUpperInvariant(chars[i]);
            }

            return new string(chars);
        }
    }
}