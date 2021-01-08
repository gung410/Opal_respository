namespace LearnerApp.Models.Achievement
{
    public class SearchECertificatesRequestModel
    {
        public SearchECertificatesRequestModel(int skipCount, int maxResultCount = GlobalSettings.MaxResultPerPage)
        {
            SkipCount = skipCount;
            MaxResultCount = maxResultCount;
        }

        public int MaxResultCount { get; }

        public int SkipCount { get; }

        public string SearchType => "IssuanceTracking";

        public bool ApplySearchTextForCourse => true;

        public bool MyRegistrationOnly => true;
    }
}
