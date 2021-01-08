using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveCourseUserRequestDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public int DepartmentId { get; set; }

        public SaveCourseUserRequestDtoIdentity Identity { get; set; }

        public SaveCourseUserRequestDtoEntityStatus EntityStatus { get; set; }

        public Guid PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public List<string> Designations { get; set; }

        public List<string> TrackIds { get; set; }

        public List<string> TeachingLevelIds { get; set; }

        public List<string> TeachingCourseOfStudyIds { get; set; }

        public List<string> TeachingSubjectIds { get; set; }

        public List<string> CocurricularActivityIds { get; set; }

        public List<string> DevelopmentalRoleIds { get; set; }

        public List<string> ServiceSchemeIds { get; set; }

        public List<string> JobFamilyIds { get; set; }

        public List<string> EasSubstantiveGradeBandingIds { get; set; }

        public List<string> SystemRoles { get; set; }

        public List<string> LearningFrameworks { get; set; }
    }
}
