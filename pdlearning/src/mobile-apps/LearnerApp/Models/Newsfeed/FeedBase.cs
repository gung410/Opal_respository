using System;
using System.Linq;
using System.Text.RegularExpressions;
using LearnerApp.Common;
using LearnerApp.Models.UserOnBoarding;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace LearnerApp.Models.Newsfeed
{
    public class FeedBase
    {
        private string _postContent;
        private string _userId;

        public FeedBase()
        {
            var accountProperties = Application.Current.Properties.GetAccountProperties();

            if (accountProperties != null)
            {
                _userId = accountProperties.User.Sub;
            }
        }

        public string PostContent
        {
            get
            {
                return _postContent;
            }

            set
            {
                _postContent = value.Replace("<csl>", string.Empty).Replace("</csl>", string.Empty);

                var urls = ExtractUrlFromContent.ExtractImageUrl(_postContent);
                if (!urls.IsNullOrEmpty())
                {
                    string url = urls.First();
                    if (url.StartsWith(GlobalSettings.WebViewUrlSocial))
                    {
                        PostContentImageUrl = $"{url}&username={_userId}";
                        _postContent = Regex.Replace(_postContent, "<img.+?>", string.Empty);
                    }
                }
            }
        }

        public string PostedBy { get; set; }

        public string PostType { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ChangedDate { get; set; }

        public Forward PostForward { get; set; }

        public FeedType Type { get; set; }

        public string Description { get; set; }

        public string CommunityName { get; set; }

        [JsonIgnore]
        public UserInformation PostToInfo { get; set; }

        [JsonIgnore]
        public UserInformation PostedByInfo { get; set; }

        [JsonIgnore]
        public string PostContentImageUrl { get; set; }
    }
}
