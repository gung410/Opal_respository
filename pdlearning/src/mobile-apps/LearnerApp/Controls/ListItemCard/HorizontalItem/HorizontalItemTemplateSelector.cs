using LearnerApp.Common.DataTemplateSelector;
using LearnerApp.Models;
using LearnerApp.Models.Learner;
using Xamarin.Forms;

namespace LearnerApp.Controls.ListItemCard.HorizontalItem
{
    public class HorizontalItemTemplateSelector : SkeletonDataTemplateSelector
    {
        public DataTemplate HorizontalCourseItemTemplate { get; set; }

        public DataTemplate HorizontalDigitalContentItemTemplate { get; set; }

        public DataTemplate HorizontalLearningPathItemTemplate { get; set; }

        public DataTemplate HorizontalCommunityItemTemplate { get; set; }

        protected override DataTemplate GetContentTemplate(object item, BindableObject container)
        {
            var value = item as ItemCard;

            switch (value.CardType)
            {
                case BookmarkType.Course:
                case BookmarkType.Microlearning:
                    return HorizontalCourseItemTemplate;
                case BookmarkType.DigitalContent:
                    return HorizontalDigitalContentItemTemplate;
                case BookmarkType.LearningPath:
                    return HorizontalLearningPathItemTemplate;
                case BookmarkType.Community:
                    return HorizontalCommunityItemTemplate;
                default:
                    break;
            }

            return null;
        }
    }
}
