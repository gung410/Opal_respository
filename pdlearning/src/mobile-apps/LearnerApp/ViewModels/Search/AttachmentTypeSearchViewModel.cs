using LearnerApp.Models.Search;

namespace LearnerApp.ViewModels.Search
{
    public class AttachmentTypeSearchViewModel
    {
        public AttachmentTypeSearchViewModel(AttachmentTypeEnum value)
        {
            Value = value;
        }

        public AttachmentTypeEnum Value { get; }

        public string DisplayText => Value.ToFriendlyString();
    }
}
