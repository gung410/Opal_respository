using System;

namespace LearnerApp.Models.PDPM
{
    public class SurveyInfo
    {
        public Identity Identity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Info { get; set; }

        public string FinishText { get; set; }

        public string DisplayName { get; set; }

        public PeriodInfo PeriodInfo { get; set; }
    }
}
