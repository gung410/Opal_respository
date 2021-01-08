using LearnerApp.Models;
using LearnerApp.Models.Learner;
using Xamarin.Forms;

namespace LearnerApp.Controls.ListItemCard.VerticalItem
{
    public class VerticalItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate VerticalCourseItemTemplate { get; set; }

        public DataTemplate VerticalDigitalContentItemTemplate { get; set; }

        public DataTemplate VerticalCommunityItemTemplate { get; set; }

        public DataTemplate VerticalLearningPathItemTemplate { get; set; }

        public DataTemplate VerticalStandAloneFormItemTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var value = item as ItemCard;

            switch (value.CardType)
            {
                case BookmarkType.Course:
                case BookmarkType.Microlearning:
                    return VerticalCourseItemTemplate;
                case BookmarkType.DigitalContent:
                    return VerticalDigitalContentItemTemplate;
                case BookmarkType.Community:
                    return VerticalCommunityItemTemplate;
                case BookmarkType.LearningPath:
                    return VerticalLearningPathItemTemplate;
                case BookmarkType.StandAloneForm:
                    return VerticalStandAloneFormItemTemplate;
                default:
                    return null;
            }
        }
    }
}
