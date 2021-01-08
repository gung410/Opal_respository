using LearnerApp.Common.DataTemplateSelector;
using LearnerApp.Models.Newsfeed;
using Xamarin.Forms;

namespace LearnerApp.Views.Home.Newsfeed
{
    public class NewsfeedDataTemplateSelector : SkeletonDataTemplateSelector
    {
        public DataTemplate PostForwardDataTemplate { get; set; }

        public DataTemplate UserPostFeeddDataTemplate { get; set; }

        public DataTemplate CommunityPostFeeddDataTemplate { get; set; }

        public DataTemplate PdpmSuggestCourseFeeddDataTemplate { get; set; }

        protected override DataTemplate GetContentTemplate(object item, BindableObject container)
        {
            var value = item as Feed;

            switch (value.Type)
            {
                case FeedType.PostForward:
                    return PostForwardDataTemplate;
                case FeedType.UserPostFeed:
                    return UserPostFeeddDataTemplate;
                case FeedType.CommunityPostFeed:
                    return CommunityPostFeeddDataTemplate;
                case FeedType.PdpmSuggestCourseFeed:
                    return PdpmSuggestCourseFeeddDataTemplate;
                default:
                    break;
            }

            return null;
        }
    }
}
