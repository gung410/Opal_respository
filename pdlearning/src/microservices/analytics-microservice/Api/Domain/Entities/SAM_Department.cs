using System;
using System.Collections.Generic;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_Department : Entity<string>
    {
        public SAM_Department()
        {
            CamCourseApplicableBranch = new HashSet<CAM_CourseApplicableBranch>();
            CamCourseApplicableCluster = new HashSet<CAM_CourseApplicableCluster>();
            CamCourseApplicableDivision = new HashSet<CAM_CourseApplicableDivision>();
            CamCourseApplicableSchool = new HashSet<CAM_CourseApplicableSchool>();
            CamCourseApplicableZone = new HashSet<CAM_CourseApplicableZone>();
            CamCourseOwnerBranch = new HashSet<CAM_CourseOwnerBranch>();
            CamCourseOwnerDivision = new HashSet<CAM_CourseOwnerDivision>();
            InverseParent = new HashSet<SAM_Department>();
            SamDepartmentDepartmentType = new HashSet<SAM_DepartmentDepartmentType>();
            LearnerUserBookmarksCourse = new HashSet<Learner_UserBookmarksCourse>();
            LearnerUserBookmarksDigitalContent = new HashSet<Learner_UserBookmarksDigitalContent>();
            LearnerUserClassRun = new HashSet<Learner_UserClassRun>();
            LearnerUserCourses = new HashSet<Learner_UserCourse>();
            LearnerUserDigitalContent = new HashSet<Learner_UserDigitalContent>();
            LearnerUserReviews = new HashSet<Learner_UserReview>();
            SamUserOtpClaims = new HashSet<SAM_UserOtpClaim>();
        }

        public string ParentId { get; set; }

        public string Name { get; set; }

        public string Adress { get; set; }

        public string OrgNo { get; set; }

        public DateTime Created { get; set; }

        public int ExtId { get; set; }

        public short Locked { get; set; }

        public int CountryCode { get; set; }

        public DateTime? LastUpdated { get; set; }

        public string ArchetypeId { get; set; }

        public bool IsDeleted { get; set; }

        public string TypeOfOrganizationUnits { get; set; }

        public int? EntityStatusId { get; set; }

        public int? EntityStatusReasonId { get; set; }

        public string Levels { get; set; }

        public string Path { get; set; }

        public int Hdid { get; set; }

        public int? ParentIdD { get; set; }

        public string PathNameHD { get; set; }

        public virtual SAM_Archetypes Archetype { get; set; }

        public virtual SAM_EntityStatuses EntityStatus { get; set; }

        public virtual SAM_EntityStatusReasons EntityStatusReason { get; set; }

        public virtual SAM_Department Parent { get; set; }

        public virtual ICollection<CAM_CourseApplicableBranch> CamCourseApplicableBranch { get; set; }

        public virtual ICollection<CAM_CourseApplicableCluster> CamCourseApplicableCluster { get; set; }

        public virtual ICollection<CAM_CourseApplicableDivision> CamCourseApplicableDivision { get; set; }

        public virtual ICollection<CAM_CourseApplicableSchool> CamCourseApplicableSchool { get; set; }

        public virtual ICollection<CAM_CourseApplicableZone> CamCourseApplicableZone { get; set; }

        public virtual ICollection<CAM_CourseOwnerBranch> CamCourseOwnerBranch { get; set; }

        public virtual ICollection<CAM_CourseOwnerDivision> CamCourseOwnerDivision { get; set; }

        public virtual ICollection<SAM_Department> InverseParent { get; set; }

        public virtual ICollection<SAM_DepartmentDepartmentType> SamDepartmentDepartmentType { get; set; }

        public virtual ICollection<Learner_UserBookmarksCourse> LearnerUserBookmarksCourse { get; set; }

        public virtual ICollection<Learner_UserBookmarksDigitalContent> LearnerUserBookmarksDigitalContent { get; set; }

        public virtual ICollection<Learner_UserClassRun> LearnerUserClassRun { get; set; }

        public virtual ICollection<Learner_UserCourse> LearnerUserCourses { get; set; }

        public virtual ICollection<Learner_UserDigitalContent> LearnerUserDigitalContent { get; set; }

        public virtual ICollection<Learner_UserReview> LearnerUserReviews { get; set; }

        public virtual ICollection<SAM_UserOtpClaim> SamUserOtpClaims { get; set; }
    }
}
