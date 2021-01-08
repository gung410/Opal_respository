namespace LearnerApp.Models.PdCatelogue
{
    public class PdCatelogueSearchFilter
    {
        public string UserId { get; set; }

        public int Page { get; set; }

        public int Limit { get; set; } = GlobalSettings.MaxResultPerPage;

        public string SearchText { get; set; }

        public string[] ResourceTypesFilter { get; set; }

        public string[] StatisticResourceTypes { get; set; }

        public string[] SearchFields { get; set; } = { "title", "description", "code", "externalcode", "tag" };

        public PdSearchCriteria SearchCriteria { get; set; }

        public PdSearchCriteriaOr SearchCriteriaOr { get; set; }

        public string Sort { get; set; } = "desc";

        public bool UseFuzzy { get; set; } = true;

        public bool UseSynonym { get; set; } = true;
    }
}
