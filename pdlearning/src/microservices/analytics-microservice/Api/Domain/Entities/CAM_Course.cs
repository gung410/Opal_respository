using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_Course
    {
        public CAM_Course()
        {
            CamAssignment = new HashSet<CAM_Assignment>();
            CamClassRun = new HashSet<CAM_ClassRun>();
            CamCourseApplicableBranch = new HashSet<CAM_CourseApplicableBranch>();
            CamCourseApplicableCluster = new HashSet<CAM_CourseApplicableCluster>();
            CamCourseApplicableDivision = new HashSet<CAM_CourseApplicableDivision>();
            CamCourseApplicableSchool = new HashSet<CAM_CourseApplicableSchool>();
            CamCourseApplicableZone = new HashSet<CAM_CourseApplicableZone>();
            CamCourseCategory = new HashSet<CAM_CourseCategory>();
            CamCourseCoCurricularActivity = new HashSet<CAM_CourseCoCurricularActivity>();
            CamCourseCoFacilitator = new HashSet<CAM_CourseCoFacilitator>();
            CamCourseCollaborativeContentCreator = new HashSet<CAM_CourseCollaborativeContentCreator>();
            CamCourseCourseOfStudy = new HashSet<CAM_CourseCourseOfStudy>();
            CamCourseDevelopmentRole = new HashSet<CAM_CourseDevelopmentRole>();
            CamCourseEasSubstantiveGradeBanding = new HashSet<CAM_CourseEasSubstantiveGradeBanding>();
            CamCourseFacilitator = new HashSet<CAM_CourseFacilitator>();
            CamCourseJobFamily = new HashSet<CAM_CourseJobFamily>();
            CamCourseLearningArea = new HashSet<CAM_CourseLearningArea>();
            CamCourseLearningDimension = new HashSet<CAM_CourseLearningDimension>();
            CamCourseLearningFramework = new HashSet<CAM_CourseLearningFramework>();
            CamCourseLearningSubArea = new HashSet<CAM_CourseLearningSubArea>();
            CamCourseOwnerBranch = new HashSet<CAM_CourseOwnerBranch>();
            CamCourseOwnerDivision = new HashSet<CAM_CourseOwnerDivision>();
            CamCoursePdperiod = new HashSet<CAM_CoursePdperiod>();
            CamCoursePreRequisiteCourse = new HashSet<CAM_CoursePreRequisite>();
            CamCoursePreRequisitePreRequisiteCourse = new HashSet<CAM_CoursePreRequisite>();
            CamCourseServiceScheme = new HashSet<CAM_CourseServiceScheme>();
            CamCourseTeachingLevel = new HashSet<CAM_CourseTeachingLevel>();
            CamCourseTeachingSubject = new HashSet<CAM_CourseTeachingSubject>();
            CamLecture = new HashSet<CAM_Lecture>();
            CamRegistration = new HashSet<CAM_Registration>();
            CamSection = new HashSet<CAM_Section>();
            LearnerUserCourses = new HashSet<Learner_UserCourse>();
            LearnerUserReviews = new HashSet<Learner_UserReview>();
        }

        public Guid CourseId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string CourseCode { get; set; }

        public string CourseName { get; set; }

        public string CourseType { get; set; }

        public string CourseContent { get; set; }

        public bool IsDeleted { get; set; }

        public Guid? JobFamilyId { get; set; }

        public Guid? CourseLevelId { get; set; }

        public Guid? PostCourseEvaluationFormId { get; set; }

        public Guid? PreCourseEvaluationFormId { get; set; }

        public Guid? NatureOfCourseId { get; set; }

        public string Description { get; set; }

        public string CourseObjective { get; set; }

        public int? DurationMinutes { get; set; }

        public string Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? PublishDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid ChangedByUserId { get; set; }

        public Guid? CreatedByUserHistoryId { get; set; }

        public Guid? ChangedByUserHistoryId { get; set; }

        public string CreatedByDepartmentId { get; set; }

        public string ChangedByDepartmentId { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public string Source { get; set; }

        public string AcknowledgementAndCredit { get; set; }

        public string Remarks { get; set; }

        public string CopyrightOwner { get; set; }

        public bool AllowNonCommerInMoereuseWithModification { get; set; }

        public bool AllowNonCommerInMoeReuseWithoutModification { get; set; }

        public bool AllowNonCommerReuseWithModification { get; set; }

        public bool? AllowNonCommerReuseWithoutModification { get; set; }

        public bool AllowPersonalDownload { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public decimal? CourseFee { get; set; }

        public string CourseOutlineStructure { get; set; }

        public int? DurationHours { get; set; }

        public Guid? FirstAdministratorId { get; set; }

        public Guid? LearningModeId { get; set; }

        public int? MaxParticipantPerClass { get; set; }

        public int? MaximumPlacesPerSchool { get; set; }

        public Guid? MoeofficerId { get; set; }

        public decimal? NotionalCost { get; set; }

        public int? NumOfBeginningTeacher { get; set; }

        public int? NumOfHoursPerClass { get; set; }

        public int? NumOfHoursPerSession { get; set; }

        public int? NumOfMiddleManagement { get; set; }

        public int? NumOfPlannedClass { get; set; }

        public int? NumOfSchoolLeader { get; set; }

        public int? NumOfSeniorOrLeadTeacher { get; set; }

        public int? NumOfSessionPerClass { get; set; }

        public string OtherTrainingAgencyReason { get; set; }

        public string EcertificatePrerequisite { get; set; }

        public Guid? EcertificateTemplateId { get; set; }

        public Guid? PdactivityTypeId { get; set; }

        public string PdareaThemeCode { get; set; }

        public Guid? PdareaThemeId { get; set; }

        public Guid? CoursePlanningCycleId { get; set; }

        public string PlaceOfWork { get; set; }

        public Guid? PrimaryApprovingOfficerId { get; set; }

        public Guid? SecondAdministratorId { get; set; }

        public string RegistrationMethod { get; set; }

        public int? TotalHoursAttendWithinYear { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public string ExternalCode { get; set; }

        public DateTime? PlanningArchiveDate { get; set; }

        public DateTime? PlanningPublishDate { get; set; }

        public string ContentStatus { get; set; }

        public DateTime? PublishedContentDate { get; set; }

        public DateTime? SubmittedContentDate { get; set; }

        public DateTime? ApprovalContentDate { get; set; }

        public string DepartmentId { get; set; }

        public bool IsMigrated { get; set; }

        public DateTime? VerifiedDate { get; set; }

        public string FullTextSearch { get; set; }

        public string FullTextSearchKey { get; set; }

        public int MaxReLearningTimes { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public virtual MT_CourseLevel CourseLevel { get; set; }

        public virtual CAM_CoursePlanningCycle CoursePlanningCycle { get; set; }

        public virtual CAM_EcertificateTemplate EcertificateTemplate { get; set; }

        public virtual MT_JobFamily JobFamily { get; set; }

        public virtual MT_LearningMode LearningMode { get; set; }

        public virtual MT_NatureOfCourse NatureOfCourse { get; set; }

        public virtual MT_PdactivityType PdactivityType { get; set; }

        public virtual CCPM_Form PostCourseEvaluationForm { get; set; }

        public virtual CCPM_Form PreCourseEvaluationForm { get; set; }

        public virtual ICollection<CAM_Assignment> CamAssignment { get; set; }

        public virtual ICollection<CAM_ClassRun> CamClassRun { get; set; }

        public virtual ICollection<CAM_CourseApplicableBranch> CamCourseApplicableBranch { get; set; }

        public virtual ICollection<CAM_CourseApplicableCluster> CamCourseApplicableCluster { get; set; }

        public virtual ICollection<CAM_CourseApplicableDivision> CamCourseApplicableDivision { get; set; }

        public virtual ICollection<CAM_CourseApplicableSchool> CamCourseApplicableSchool { get; set; }

        public virtual ICollection<CAM_CourseApplicableZone> CamCourseApplicableZone { get; set; }

        public virtual ICollection<CAM_CourseCategory> CamCourseCategory { get; set; }

        public virtual ICollection<CAM_CourseCoCurricularActivity> CamCourseCoCurricularActivity { get; set; }

        public virtual ICollection<CAM_CourseCoFacilitator> CamCourseCoFacilitator { get; set; }

        public virtual ICollection<CAM_CourseCollaborativeContentCreator> CamCourseCollaborativeContentCreator { get; set; }

        public virtual ICollection<CAM_CourseCourseOfStudy> CamCourseCourseOfStudy { get; set; }

        public virtual ICollection<CAM_CourseDevelopmentRole> CamCourseDevelopmentRole { get; set; }

        public virtual ICollection<CAM_CourseEasSubstantiveGradeBanding> CamCourseEasSubstantiveGradeBanding { get; set; }

        public virtual ICollection<CAM_CourseFacilitator> CamCourseFacilitator { get; set; }

        public virtual ICollection<CAM_CourseJobFamily> CamCourseJobFamily { get; set; }

        public virtual ICollection<CAM_CourseLearningArea> CamCourseLearningArea { get; set; }

        public virtual ICollection<CAM_CourseLearningDimension> CamCourseLearningDimension { get; set; }

        public virtual ICollection<CAM_CourseLearningFramework> CamCourseLearningFramework { get; set; }

        public virtual ICollection<CAM_CourseLearningSubArea> CamCourseLearningSubArea { get; set; }

        public virtual ICollection<CAM_CourseOwnerBranch> CamCourseOwnerBranch { get; set; }

        public virtual ICollection<CAM_CourseOwnerDivision> CamCourseOwnerDivision { get; set; }

        public virtual ICollection<CAM_CoursePdperiod> CamCoursePdperiod { get; set; }

        public virtual ICollection<CAM_CoursePreRequisite> CamCoursePreRequisiteCourse { get; set; }

        public virtual ICollection<CAM_CoursePreRequisite> CamCoursePreRequisitePreRequisiteCourse { get; set; }

        public virtual ICollection<CAM_CourseServiceScheme> CamCourseServiceScheme { get; set; }

        public virtual ICollection<CAM_CourseTeachingLevel> CamCourseTeachingLevel { get; set; }

        public virtual ICollection<CAM_CourseTeachingSubject> CamCourseTeachingSubject { get; set; }

        public virtual ICollection<CAM_Lecture> CamLecture { get; set; }

        public virtual ICollection<CAM_Registration> CamRegistration { get; set; }

        public virtual ICollection<CAM_Section> CamSection { get; set; }

        public virtual ICollection<Learner_UserCourse> LearnerUserCourses { get; set; }

        public virtual ICollection<Learner_UserReview> LearnerUserReviews { get; set; }
    }
}
