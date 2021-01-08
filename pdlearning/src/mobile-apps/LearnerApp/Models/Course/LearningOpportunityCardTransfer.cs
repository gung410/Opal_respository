using System.Collections.Generic;
using LearnerApp.Models.Course;
using LearnerApp.Models.UserOnBoarding;

namespace LearnerApp.Models.Learner
{
    public class LearningOpportunityCardTransfer
    {
        public List<MetadataTag> MetadataTags { get; set; }

        public CourseExtendedInformation CourseExtendedInformation { get; set; }

        public List<PrerequisiteCourse> PrerequisiteCourses { get; set; }

        public List<Department> Departments { get; set; }

        public List<UserInformation> UserInformations { get; set; }
    }
}
