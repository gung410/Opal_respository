using System;

namespace LearnerApp.Models.PDPM
{
    public class PDORecommendationItem
    {
        public Identity ResultIdentity { get; set; }

        public ObjectiveInfo ObjectiveInfo { get; set; }

        public DateTime Timestamp { get; set; }

        public int AssessmentRoleId { get; set; }

        public DateTime StartDate { get; set; }

        public AssessmentStatusInfo AssessmentStatusInfo { get; set; }

        public string ParentResultExtId { get; set; }

        public string PdPlanType { get; set; }

        public string PdPlanActivity { get; set; }

        public ObjectiveInfo CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public ObjectiveInfo LastUpdatedBy { get; set; }

        public SurveyInfo SurveyInfo { get; set; }

        public AdditionalProperties AdditionalProperties { get; set; }
    }
}
