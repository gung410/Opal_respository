using System;
using System.Collections.Generic;
using Microservice.Analytics.Domain.ValueObject;

namespace Microservice.Analytics.Application.Consumers.CAM.Messages
{
    public class CAMCourseChangeMessage
    {
        public Guid Id { get; set; }

        public string CourseName { get; set; }

        public Guid? LearningMode { get; set; }

        public string CourseCode { get; set; }

        public string Description { get; set; }

        public Guid? MOEOfficerId { get; set; }

        public Guid? PDActivityType { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public AnalyticCAMCourseType CourseType { get; set; }

        public int MaxReLearningTimes { get; set; }

        public Guid? FirstAdministratorId { get; set; }

        public Guid? SecondAdministratorId { get; set; }

        public Guid? PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public AnalyticCAMCourseStatus Status { get; set; }

        public AnalyticCAMContentStatus ContentStatus { get; set; }

        public DateTime? PublishedContentDate { get; set; }

        public DateTime? SubmittedContentDate { get; set; }

        public string Version { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid ChangedBy { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public DateTime? ApprovalContentDate { get; set; }

        public DateTime? PublishDate { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public string Source { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public int DepartmentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? MessageCreatedDate { get; set; }

        public bool? IsDeleted { get; set; }

        public Guid? CourseLevel { get; set; }

        public Guid? PostCourseEvaluationFormId { get; set; }

        public Guid? PreCourseEvaluationFormId { get; set; }

        public string CourseContent { get; set; }

        public Guid? NatureOfCourse { get; set; }

        public string CourseObjective { get; set; }

        public int? DurationMinutes { get; set; }

        public string AcknowledgementAndCredit { get; set; }

        public string Remarks { get; set; }

        public string CopyrightOwner { get; set; }

        public bool? AllowNonCommerInMoereuseWithModification { get; set; }

        public bool? AllowNonCommerInMoeReuseWithoutModification { get; set; }

        public bool? AllowNonCommerReuseWithModification { get; set; }

        public bool? AllowNonCommerReuseWithoutModification { get; set; }

        public bool? AllowPersonalDownload { get; set; }

        public float? CourseFee { get; set; }

        public string CourseOutlineStructure { get; set; }

        public int? DurationHours { get; set; }

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

        public List<string> OtherTrainingAgencyReason { get; set; }

        public string ECertificatePrerequisite { get; set; }

        public Guid? ECertificateTemplateId { get; set; }

        public string PDAreaThemeCode { get; set; }

        public Guid? PDAreaThemeId { get; set; }

        public Guid? CoursePlanningCycleId { get; set; }

        public string PlaceOfWork { get; set; }

        public string RegistrationMethod { get; set; }

        public string ExternalCode { get; set; }

        public DateTime? PlanningArchiveDate { get; set; }

        public DateTime? PlanningPublishDate { get; set; }

        public bool? IsMigrated { get; set; }

        public DateTime? VerifiedDate { get; set; }

        public string FullTextSearch { get; set; }

        public string FullTextSearchKey { get; set; }

        public int VersionNo { get; set; }

        public double? TotalHoursAttendWithinYear { get; set; }

        public int? NumOfMinutesPerSession { get; set; }

        public int? NumOfExperiencedTeacher { get; set; }

        public string CourseNameInECertificate { get; set; }

        public bool? IsLearnerStarted { get; set; }
    }
}
