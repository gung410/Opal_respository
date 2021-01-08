using System;
using System.Collections.Generic;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_UserHistory : Entity<Guid>
    {
        public SAM_UserHistory()
        {
            CamRegistration = new HashSet<CAM_Registration>();
            CcpmFormAnswer = new HashSet<CCPM_FormAnswer>();
            SamUserCoCurricularActivity = new HashSet<SAM_UserCoCurricularActivity>();
            SamUserCourseOfStudy = new HashSet<SAM_UserCourseOfStudy>();
            SamUserJobFamily = new HashSet<SAM_UserJobFamily>();
            SamUserProfessionalInterestArea = new HashSet<SAM_UserProfessionalInterestArea>();
            SamUserTeachingLevel = new HashSet<SAM_UserTeachingLevel>();
            SamUserTeachingSubject = new HashSet<SAM_UserTeachingSubject>();
            SamUserTrack = new HashSet<SAM_UserTrack>();
            SamUserUserTypes = new HashSet<SAM_UserUserTypes>();
            LearnerUserAssignments = new HashSet<Learner_UserAssignment>();
            LearnerUserClassRun = new HashSet<Learner_UserClassRun>();
            LearnerUserCourses = new HashSet<Learner_UserCourse>();
            LearnerUserDigitalContent = new HashSet<Learner_UserDigitalContent>();
            LearnerUserLearningPaths = new HashSet<Learner_UserLearningPath>();
            LearnerUserActivityAssignments = new HashSet<Learner_UserActivityAssignment>();
            LearnerUserActivityLandingPages = new HashSet<Learner_UserActivityLandingPage>();
            LearnerUserActivityLectures = new HashSet<Learner_UserActivityLecture>();
            LearnerUserActivityPdopages = new HashSet<Learner_UserActivityPdoPage>();
            LearnerUserActivityQuizzes = new HashSet<Learner_UserActivityQuiz>();
            LearnerUserActivityRevisitMlu = new HashSet<Learner_UserActivityRevisitMlu>();
            SamUserOtpClaims = new HashSet<SAM_UserOtpClaim>();
        }

        public Guid UserId { get; set; }

        public int No { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string DepartmentId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int? CountryCode { get; set; }

        public short? Gender { get; set; }

        public short Locked { get; set; }

        public DateTime? EntityExpirationDate { get; set; }

        public DateTime? EntityActiveDate { get; set; }

        public bool IsDeleted { get; set; }

        public string Roles { get; set; }

        public Guid? ServiceSchemeId { get; set; }

        public string DesignationId { get; set; }

        public string JobTitle { get; set; }

        public bool? FinishOnBoarding { get; set; }

        public bool? SentWelcomeEmail { get; set; }

        public DateTime? SentWelcomeEmailDate { get; set; }

        public int? EntityStatusId { get; set; }

        public int? EntityStatusReasonId { get; set; }

        public DateTime? StartedOnboardingDate { get; set; }

        public DateTime? FinishedOnboardingDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? FirstLoginDate { get; set; }

        public string NotificationPreference { get; set; }

        public DateTime? DateAppointedToService { get; set; }

        public DateTime? DateToCurrentScheme { get; set; }

        public DateTime? DateAppointedToTrainedGrade { get; set; }

        public bool? HoldsSupervisoryRole { get; set; }

        public string ArcheTypeId { get; set; }

        public string ExtId { get; set; }

        public Guid? DevelopmentRoleID { get; set; }

        public int? DepartmentExtId { get; set; }

        public string UserExtId { get; set; }

        public bool? MOEStaff { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? LastUpdated { get; set; }

        public virtual ICollection<CAM_Registration> CamRegistration { get; set; }

        public virtual ICollection<CCPM_FormAnswer> CcpmFormAnswer { get; set; }

        public virtual ICollection<SAM_UserCoCurricularActivity> SamUserCoCurricularActivity { get; set; }

        public virtual ICollection<SAM_UserCourseOfStudy> SamUserCourseOfStudy { get; set; }

        public virtual ICollection<SAM_UserJobFamily> SamUserJobFamily { get; set; }

        public virtual ICollection<SAM_UserProfessionalInterestArea> SamUserProfessionalInterestArea { get; set; }

        public virtual ICollection<SAM_UserTeachingLevel> SamUserTeachingLevel { get; set; }

        public virtual ICollection<SAM_UserTeachingSubject> SamUserTeachingSubject { get; set; }

        public virtual ICollection<SAM_UserTrack> SamUserTrack { get; set; }

        public virtual ICollection<SAM_UserUserTypes> SamUserUserTypes { get; set; }

        public virtual ICollection<SAM_UserLogin> SamUserLogins { get; set; }

        public virtual ICollection<Learner_UserAssignment> LearnerUserAssignments { get; set; }

        public virtual ICollection<Learner_UserClassRun> LearnerUserClassRun { get; set; }

        public virtual ICollection<Learner_UserCourse> LearnerUserCourses { get; set; }

        public virtual ICollection<Learner_UserDigitalContent> LearnerUserDigitalContent { get; set; }

        public virtual ICollection<Learner_UserLearningPath> LearnerUserLearningPaths { get; set; }

        public virtual ICollection<Learner_UserActivityAssignment> LearnerUserActivityAssignments { get; set; }

        public virtual ICollection<Learner_UserActivityLandingPage> LearnerUserActivityLandingPages { get; set; }

        public virtual ICollection<Learner_UserActivityLecture> LearnerUserActivityLectures { get; set; }

        public virtual ICollection<Learner_UserActivityPdoPage> LearnerUserActivityPdopages { get; set; }

        public virtual ICollection<Learner_UserActivityQuiz> LearnerUserActivityQuizzes { get; set; }

        public virtual ICollection<Learner_UserActivityRevisitMlu> LearnerUserActivityRevisitMlu { get; set; }

        public virtual ICollection<SAM_UserOtpClaim> SamUserOtpClaims { get; set; }

        public virtual ICollection<Learner_UserActivityResume> LearnerUserActivityResume { get; set; }
    }
}
