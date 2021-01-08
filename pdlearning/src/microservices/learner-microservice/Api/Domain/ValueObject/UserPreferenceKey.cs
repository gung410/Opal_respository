using System.Collections.Generic;

namespace Microservice.Learner.Domain.ValueObject
{
    public class UserPreferenceKey
    {
        public const string HomeMyLearning = "Home.MyLearning.Show";
        public const string HomeRecommendForU = "Home.RecommendForU.Show";
        public const string HomeRecommendForOrg = "Home.RecommendForOrg.Show";
        public const string HomeBookmark = "Home.Bookmark.Show";
        public const string HomeNewsFeed = "Home.NewsFeed.Show";
        public const string HomeOutstanding = "Home.Outstanding.Show";
        public const string HomeShare = "Home.Share.Show";
        public const string HomeCalendar = "Home.Calendar.Show";
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class UserPreferenceConfiguration
#pragma warning restore SA1402 // File may only contain a single type
    {
        public UserPreferenceValueType Type { get; internal set; }

        public string DefaultValueString { get; internal set; }
    }

#pragma warning disable SA1402 // File may only contain a single type
    public class UserPreferenceKeyMapConfig
#pragma warning restore SA1402 // File may only contain a single type
    {
        public static readonly IReadOnlyDictionary<string, UserPreferenceConfiguration> PredefinedConfiguration = new Dictionary<string, UserPreferenceConfiguration>
        {
            { UserPreferenceKey.HomeMyLearning, new UserPreferenceConfiguration { Type = UserPreferenceValueType.Boolean, DefaultValueString = "true" } },
            { UserPreferenceKey.HomeRecommendForU, new UserPreferenceConfiguration { Type = UserPreferenceValueType.Boolean, DefaultValueString = "true" } },
            { UserPreferenceKey.HomeRecommendForOrg, new UserPreferenceConfiguration { Type = UserPreferenceValueType.Boolean, DefaultValueString = "true" } },
            { UserPreferenceKey.HomeBookmark, new UserPreferenceConfiguration { Type = UserPreferenceValueType.Boolean, DefaultValueString = "true" } },
            { UserPreferenceKey.HomeNewsFeed, new UserPreferenceConfiguration { Type = UserPreferenceValueType.Boolean, DefaultValueString = "true" } },
            { UserPreferenceKey.HomeOutstanding, new UserPreferenceConfiguration { Type = UserPreferenceValueType.Boolean, DefaultValueString = "true" } },
            { UserPreferenceKey.HomeShare, new UserPreferenceConfiguration { Type = UserPreferenceValueType.Boolean, DefaultValueString = "true" } },
            { UserPreferenceKey.HomeCalendar, new UserPreferenceConfiguration { Type = UserPreferenceValueType.Boolean, DefaultValueString = "true" } },
        };

        public static UserPreferenceValueType GetValueTypeOfKey(string key)
        {
            return PredefinedConfiguration.ContainsKey(key) ? PredefinedConfiguration[key].Type : UserPreferenceValueType.None;
        }

        public static string GetDefaultValueString(string key)
        {
            return PredefinedConfiguration.ContainsKey(key) ? PredefinedConfiguration[key].DefaultValueString : null;
        }
    }
}
