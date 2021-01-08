using System;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CAM_CourseHistory : AuditedEntity
    {
        public Guid CourseId { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string CourseCode { get; set; }

        public string CourseName { get; set; }

        public string CourseType { get; set; }

        public string CourseContent { get; set; }

        public bool IsDeleted { get; set; }

        public Guid? CourseLevelId { get; set; }

        public Guid? PostCourseEvaluationFormId { get; set; }

        public Guid? PreCourseEvaluationFormId { get; set; }

        public Guid NatureOfCourseId { get; set; }

        public string Description { get; set; }

        public string CourseObjective { get; set; }

        public int? DurationMinutes { get; set; }

        public string Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? PublishDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid ChangedByUserId { get; set; }

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

        public float? CourseFee { get; set; }

        public string CourseOutlineStructure { get; set; }

        public int? DurationHours { get; set; }

        public Guid? FirstAdministratorId { get; set; }

        public Guid? LearningModeId { get; set; }

        public int? MaxParticipantPerClass { get; set; }

        public int? MaximumPlacesPerSchool { get; set; }

        public Guid? MoeofficerId { get; set; }

        public decimal? NotionalCost { get; set; }

        public int? NumOfBeginningTeacher { get; set; }

        public double? NumOfHoursPerClass { get; set; }

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

        public double? TotalHoursAttendWithinYear { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public string ExternalCode { get; set; }

        public DateTime? PlanningArchiveDate { get; set; }

        public DateTime? PlanningPublishDate { get; set; }

        public string ContentStatus { get; set; }

        public DateTime? PublishedContentDate { get; set; }

        public DateTime? SubmittedContentDate { get; set; }

        public DateTime? ApprovalContentDate { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public string DepartmentId { get; set; }

        public bool IsMigrated { get; set; }

        public DateTime? VerifiedDate { get; set; }

        public string FullTextSearch { get; set; }

        public string FullTextSearchKey { get; set; }

        public int MaxReLearningTimes { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int No { get; set; }

        public Guid? SpaceId { get; set; }

        public Guid? ClonedFromCourseId { get; set; }

        public int? NumOfMinutesPerSession { get; set; }

        public int? NumOfExperiencedTeacher { get; set; }

        public string CourseNameInECertificate { get; set; }

        public bool? IsLearnerStarted { get; set; }
    }
}
