using LearnerApp.Models.Search;

namespace LearnerApp.ViewModels.Search
{
    public class CommunityTypeSearchViewModel
    {
        public CommunityTypeSearchViewModel(CommunitySearchTypeEnum value)
        {
            Value = value;
        }

        public CommunitySearchTypeEnum Value { get; }

        public string DisplayText => Value.ToFriendlyString();
    }
}
