using System.Collections.Generic;
using LearnerApp.Common;
using LearnerApp.Models.Course;

namespace LearnerApp.Models.Learner
{
    public class LearningOpportunityClassRunCardTransfer
    {
        public string CourseId { get; set; }

        public string UserId { get; set; }

        public string ApprovingOfficer { get; set; }

        public MyClassRun MyClassRun { get; set; }

        public List<MyClassRun> MyClassRuns { get; set; }

        public MyCourseStatus MyCourseStatus { get; set; }

        public string AlternativeApprovingOfficer { get; set; }

        public RegistrationMethod RegistrationMethod { get; set; }

        public bool IsReachMaximumComplete { get; set; }

        public List<MyClassRun> RejectedMyClassRuns { get; set; }

        public List<MyClassRun> WithdrawnMyClassRuns { get; set; }
    }
}
