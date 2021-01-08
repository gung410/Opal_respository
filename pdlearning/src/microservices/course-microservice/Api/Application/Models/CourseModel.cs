using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.Models
{
    public class CourseModel
    {
        public CourseModel()
        {
        }

        public CourseModel(CourseEntity entity, bool? hasFullRight = null, bool? hasContent = null)
        {
            Id = entity.Id;
            CourseCode = entity.CourseCode;
            CourseName = entity.CourseName;
            ThumbnailUrl = entity.ThumbnailUrl;
            Description = entity.Description;
            DurationMinutes = entity.DurationMinutes;
            DurationHours = entity.DurationHours;
            PDActivityType = entity.PDActivityType;
            LearningMode = entity.LearningMode;
            ExternalCode = entity.ExternalCode;
            CourseOutlineStructure = entity.CourseOutlineStructure;
            CourseObjective = entity.CourseObjective;
            CategoryIds = entity.CategoryIds;
            OwnerDivisionIds = entity.OwnerDivisionIds;
            OwnerBranchIds = entity.OwnerBranchIds;
            PartnerOrganisationIds = entity.PartnerOrganisationIds;
            MOEOfficerId = entity.MOEOfficerId;
            MOEOfficerPhoneNumber = entity.MOEOfficerPhoneNumber;
            NotionalCost = entity.NotionalCost;
            CourseFee = entity.CourseFee;
            TrainingAgency = entity.TrainingAgency;
            OtherTrainingAgencyReason = entity.OtherTrainingAgencyReason;
            AllowPersonalDownload = entity.AllowPersonalDownload;
            AllowNonCommerInMOEReuseWithModification = entity.AllowNonCommerInMOEReuseWithModification;
            AllowNonCommerReuseWithModification = entity.AllowNonCommerReuseWithModification;
            AllowNonCommerInMoeReuseWithoutModification = entity.AllowNonCommerInMoeReuseWithoutModification;
            AllowNonCommerReuseWithoutModification = entity.AllowNonCommerReuseWithoutModification;
            CopyrightOwner = entity.CopyrightOwner;
            AcknowledgementAndCredit = entity.AcknowledgementAndCredit;
            MaximumPlacesPerSchool = entity.MaximumPlacesPerSchool;
            NumOfSchoolLeader = entity.NumOfSchoolLeader;
            NumOfSeniorOrLeadTeacher = entity.NumOfSeniorOrLeadTeacher;
            NumOfMiddleManagement = entity.NumOfMiddleManagement;
            NumOfBeginningTeacher = entity.NumOfBeginningTeacher;
            NumOfExperiencedTeacher = entity.NumOfExperiencedTeacher;
            PlaceOfWork = entity.PlaceOfWork;
            PrerequisiteCourseIds = entity.PrerequisiteCourseIds;
            ApplicableDivisionIds = entity.ApplicableDivisionIds;
            ApplicableBranchIds = entity.ApplicableBranchIds;
            ApplicableZoneIds = entity.ApplicableZoneIds;
            ApplicableClusterIds = entity.ApplicableClusterIds;
            ApplicableSchoolIds = entity.ApplicableSchoolIds;
            TrackIds = entity.TrackIds;
            DevelopmentalRoleIds = entity.DevelopmentalRoleIds;
            TeachingLevels = entity.TeachingLevels;
            TeachingSubjectIds = entity.TeachingSubjectIds;
            TeachingCourseStudyIds = entity.TeachingCourseStudyIds;
            CocurricularActivityIds = entity.CocurricularActivityIds;
            JobFamily = entity.JobFamily;
            EasSubstantiveGradeBandingIds = entity.EasSubstantiveGradeBandingIds;
            PDAreaThemeCode = entity.PDAreaThemeCode;
            PDAreaThemeId = entity.PDAreaThemeId;
            ServiceSchemeIds = entity.ServiceSchemeIds;
            SubjectAreaIds = entity.SubjectAreaIds;
            LearningFrameworkIds = entity.LearningFrameworkIds;
            LearningDimensionIds = entity.LearningDimensionIds;
            LearningAreaIds = entity.LearningAreaIds;
            LearningSubAreaIds = entity.LearningSubAreaIds;
            TeacherOutcomeIds = entity.TeacherOutcomeIds;
            MetadataKeys = entity.MetadataKeys;
            NatureOfCourse = entity.NatureOfCourse;
            NumOfPlannedClass = entity.NumOfPlannedClass;
            NumOfSessionPerClass = entity.NumOfSessionPerClass;
            NumOfHoursPerClass = entity.NumOfHoursPerClass;
            NumOfHoursPerSession = entity.NumOfHoursPerSession;
            NumOfMinutesPerSession = entity.NumOfMinutesPerSession;
            TotalHoursAttendWithinYear = entity.TotalHoursAttendWithinYear;
            MinParticipantPerClass = entity.MinParticipantPerClass;
            MaxParticipantPerClass = entity.MaxParticipantPerClass;
            PlanningPublishDate = entity.PlanningPublishDate;
            PlanningArchiveDate = entity.PlanningArchiveDate;
            WillArchiveCommunity = entity.WillArchiveCommunity;
            MaxReLearningTimes = entity.MaxReLearningTimes;
            StartDate = entity.StartDate;
            ExpiredDate = entity.ExpiredDate;
            CourseType = entity.CourseType;
            PdActivityPeriods = entity.PdActivityPeriods;
            PreCourseEvaluationFormId = entity.PreCourseEvaluationFormId;
            PostCourseEvaluationFormId = entity.PostCourseEvaluationFormId;
            ECertificateTemplateId = entity.ECertificateTemplateId;
            ECertificatePrerequisite = entity.ECertificatePrerequisite;
            CourseNameInECertificate = entity.CourseNameInECertificate;
            FirstAdministratorId = entity.FirstAdministratorId;
            SecondAdministratorId = entity.SecondAdministratorId;
            PrimaryApprovingOfficerId = entity.PrimaryApprovingOfficerId;
            AlternativeApprovingOfficerId = entity.AlternativeApprovingOfficerId;
            CollaborativeContentCreatorIds = entity.CollaborativeContentCreatorIds;
            CourseFacilitatorIds = entity.CourseFacilitatorIds;
            CourseCoFacilitatorIds = entity.CourseCoFacilitatorIds;
            Version = entity.Version;
            Remarks = entity.Remarks;
            Status = entity.Status;
            ContentStatus = entity.ContentStatus;
            PublishedContentDate = entity.PublishedContentDate;
            CreatedBy = entity.CreatedBy;
            CreatedDate = entity.CreatedDate;
            MOEOfficerEmail = entity.MOEOfficerEmail;
            CourseLevel = entity.CourseLevel;
            SubmittedDate = entity.SubmittedDate;
            SubmittedContentDate = entity.SubmittedContentDate;
            ApprovalDate = entity.ApprovalDate;
            ApprovalContentDate = entity.ApprovalContentDate;
            CoursePlanningCycleId = entity.CoursePlanningCycleId;
            IsMigrated = entity.IsMigrated;
            HasContent = hasContent;
            VerifiedDate = entity.VerifiedDate;
            RegistrationMethod = entity.RegistrationMethod;
            NieAcademicGroups = entity.NieAcademicGroups;
            ArchiveDate = entity.ArchiveDate;
            ArchivedBy = entity.ArchivedBy;
            HasFullRight = hasFullRight;
        }

        public Guid Id { get; set; }

        // Overview Information
        public string ThumbnailUrl { get; set; }

        public string CourseName { get; set; }

        public int? DurationMinutes { get; set; }

        public int? DurationHours { get; set; }

        public string PDActivityType { get; set; }

        public string LearningMode { get; set; }

        public string CourseCode { get; set; }

        public string CourseOutlineStructure { get; set; }

        public string CourseObjective { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> CategoryIds { get; set; }

        // Provider Information
        public IEnumerable<int> OwnerDivisionIds { get; set; }

        public IEnumerable<int> OwnerBranchIds { get; set; }

        public IEnumerable<int> PartnerOrganisationIds { get; set; }

        public Guid? MOEOfficerId { get; set; }

        public string MOEOfficerPhoneNumber { get; set; }

        public string MOEOfficerEmail { get; set; }

        public decimal? NotionalCost { get; set; }

        public decimal? CourseFee { get; set; }

        public IEnumerable<string> TrainingAgency { get; set; }

        public IEnumerable<string> OtherTrainingAgencyReason { get; set; }

        public IEnumerable<string> NieAcademicGroups { get; set; }

        // Copy Rights
        public bool AllowPersonalDownload { get; set; }

        public bool AllowNonCommerInMOEReuseWithModification { get; set; }

        public bool AllowNonCommerReuseWithModification { get; set; }

        public bool AllowNonCommerInMoeReuseWithoutModification { get; set; }

        public bool AllowNonCommerReuseWithoutModification { get; set; }

        public string CopyrightOwner { get; set; }

        public string AcknowledgementAndCredit { get; set; }

        public string Remarks { get; set; }

        // Target Audience
        public int? MaximumPlacesPerSchool { get; set; }

        public int? NumOfSchoolLeader { get; set; }

        public int? NumOfSeniorOrLeadTeacher { get; set; }

        public int? NumOfMiddleManagement { get; set; }

        public int? NumOfBeginningTeacher { get; set; }

        public int? NumOfExperiencedTeacher { get; set; }

        public PlaceOfWorkType PlaceOfWork { get; set; }

        public RegistrationMethod? RegistrationMethod { get; set; }

        public IEnumerable<Guid> PrerequisiteCourseIds { get; set; }

        public IEnumerable<int> ApplicableDivisionIds { get; set; }

        public IEnumerable<int> ApplicableBranchIds { get; set; }

        public IEnumerable<int> ApplicableZoneIds { get; set; }

        public IEnumerable<int> ApplicableClusterIds { get; set; }

        public IEnumerable<int> ApplicableSchoolIds { get; set; }

        public IEnumerable<string> TrackIds { get; set; }

        public IEnumerable<string> DevelopmentalRoleIds { get; set; }

        public IEnumerable<string> TeachingLevels { get; set; }

        public IEnumerable<string> TeachingSubjectIds { get; set; }

        public IEnumerable<string> TeachingCourseStudyIds { get; set; }

        public IEnumerable<string> CocurricularActivityIds { get; set; }

        public IEnumerable<string> JobFamily { get; set; }

        public IEnumerable<string> EasSubstantiveGradeBandingIds { get; set; }

        // Metadata
        public string CourseLevel { get; set; }

        public string PDAreaThemeId { get; set; }

        public string PDAreaThemeCode { get; set; }

        public IEnumerable<string> ServiceSchemeIds { get; set; }

        public IEnumerable<string> SubjectAreaIds { get; set; }

        public IEnumerable<string> LearningFrameworkIds { get; set; }

        public IEnumerable<string> LearningDimensionIds { get; set; }

        public IEnumerable<string> LearningAreaIds { get; set; }

        public IEnumerable<string> LearningSubAreaIds { get; set; }

        public IEnumerable<string> TeacherOutcomeIds { get; set; }

        public IEnumerable<string> MetadataKeys { get; set; }

        // Course Planning Information
        public string NatureOfCourse { get; set; }

        public int? NumOfPlannedClass { get; set; }

        public int? NumOfSessionPerClass { get; set; }

        public double? NumOfHoursPerClass { get; set; }

        public int? NumOfHoursPerSession { get; set; }

        public int? NumOfMinutesPerSession { get; set; }

        public double TotalHoursAttendWithinYear { get; set; }

        public int? MinParticipantPerClass { get; set; }

        public int? MaxParticipantPerClass { get; set; }

        public DateTime? PlanningPublishDate { get; set; }

        public DateTime? PlanningArchiveDate { get; set; }

        public bool WillArchiveCommunity { get; set; }

        public CourseType CourseType { get; set; }

        public IEnumerable<string> PdActivityPeriods { get; set; }

        public int MaxReLearningTimes { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        // Evaluation And ECertificate
        public Guid? PreCourseEvaluationFormId { get; set; }

        public Guid? PostCourseEvaluationFormId { get; set; }

        public Guid? ECertificateTemplateId { get; set; }

        public PrerequisiteCertificateType? ECertificatePrerequisite { get; set; }

        public string CourseNameInECertificate { get; set; }

        // Administration Information
        public Guid? FirstAdministratorId { get; set; }

        public Guid? SecondAdministratorId { get; set; }

        public Guid? PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public IEnumerable<Guid> CollaborativeContentCreatorIds { get; set; }

        public IEnumerable<Guid> CourseFacilitatorIds { get; set; }

        public IEnumerable<Guid> CourseCoFacilitatorIds { get; set; }

        // System fields
        public CourseStatus Status { get; set; }

        public ContentStatus ContentStatus { get; set; }

        public DateTime? PublishedContentDate { get; set; }

        public string Version { get; set; }

        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid ChangedBy { get; set; }

        public string ExternalId { get; set; }

        public string ExternalCode { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime? SubmittedContentDate { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public DateTime? ApprovalContentDate { get; set; }

        public Guid? CoursePlanningCycleId { get; set; }

        public bool? HasContent { get; set; }

        public DateTime? VerifiedDate { get; set; }

        public bool IsMigrated { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public Guid? ArchivedBy { get; set; }

        public bool? HasFullRight { get; set; }
    }
}
