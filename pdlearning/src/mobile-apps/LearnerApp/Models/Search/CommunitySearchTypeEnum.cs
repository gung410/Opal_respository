using System.Collections.Generic;
using System.Linq;

namespace LearnerApp.Models.Search
{
    public enum CommunitySearchTypeEnum
    {
        Restricted,
        Open,
    }

    public static class CommunitySearchTypeEnumHelpers
    {
        private static readonly Dictionary<CommunitySearchTypeEnum, string> _apiValueDictionary = new Dictionary<CommunitySearchTypeEnum, string>()
        {
            { CommunitySearchTypeEnum.Restricted, "application" },
            { CommunitySearchTypeEnum.Open, "free" },
        };

        private static readonly Dictionary<CommunitySearchTypeEnum, string> _friendlyStringValueDictionary = new Dictionary<CommunitySearchTypeEnum, string>()
        {
            { CommunitySearchTypeEnum.Restricted, "Restricted" },
            { CommunitySearchTypeEnum.Open, "Open" },
        };

        public static CommunitySearchTypeEnum? ToCommunitySearchTypeEnum(this string category)
        {
            if (category == null)
            {
                return null;
            }

            if (_apiValueDictionary.ContainsValue(category) == false)
            {
                return null;
            }

            return _apiValueDictionary.FirstOrDefault(x => x.Value == category).Key;
        }

        public static string ToApiString(this CommunitySearchTypeEnum category)
        {
            if (_apiValueDictionary.ContainsKey(category) == false)
            {
                return null;
            }

            return _apiValueDictionary[category];
        }

        public static string ToFriendlyString(this CommunitySearchTypeEnum category)
        {
            if (_friendlyStringValueDictionary.ContainsKey(category) == false)
            {
                return null;
            }

            return _friendlyStringValueDictionary[category];
        }
    }
}
