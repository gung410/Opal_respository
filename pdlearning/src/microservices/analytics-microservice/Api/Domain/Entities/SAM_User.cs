using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_User
    {
        public SAM_User()
        {
            CslGroupUser = new HashSet<CSL_GroupUser>();
            CslSpaceMembership = new HashSet<CSL_SpaceMembership>();
        }

        public Guid UserId { get; set; }

        public string ExtId { get; set; }

        public string DepartmentId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime Created { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int? CountryCode { get; set; }

        public short? Gender { get; set; }

        public short Locked { get; set; }

        public DateTime? LastUpdated { get; set; }

        public DateTime? EntityExpirationDate { get; set; }

        public DateTime? EntityActiveDate { get; set; }

        public bool IsDeleted { get; set; }

        public string Roles { get; set; }

        public Guid? ServiceSchemeId { get; set; }

        public Guid? TeachingLevelId { get; set; }

        public Guid? TeachingSubjectId { get; set; }

        public Guid? CourseOfStudyId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? CoCircularactivityId { get; set; }

        public string DesignationId { get; set; }

        public string JobTitle { get; set; }

        public Guid? JobFamilyId { get; set; }

        public bool? FinishOnBoarding { get; set; }

        public bool? SentWelcomeEmail { get; set; }

        public DateTime? SentWelcomeEmailDate { get; set; }

        public int? EntityStatusId { get; set; }

        public int? EntityStatusReasonId { get; set; }

        public DateTime? StartedOnboardingDate { get; set; }

        public DateTime? FinishedOnboardingDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? FirstLoginDate { get; set; }

        public virtual ICollection<CSL_GroupUser> CslGroupUser { get; set; }

        public virtual ICollection<CSL_SpaceMembership> CslSpaceMembership { get; set; }
    }
}
