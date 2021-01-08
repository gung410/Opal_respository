using System;

namespace LearnerApp.Models.PDPM
{
    public class PeriodInfo
    {
        public Identity Identity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int No { get; set; }
    }
}
