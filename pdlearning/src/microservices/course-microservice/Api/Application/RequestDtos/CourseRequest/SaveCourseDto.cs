using System;
using System.Collections.Generic;
using Microservice.Course.Application.Commands;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveCourseDto
    {
        public Guid? Id { get; set; }

        public CourseStatus Status { get; set; } = CourseStatus.Draft;

        // Basic Info
        public string ThumbnailUrl { get; set; }

        public string CourseName { get; set; }

        public int? DurationMinutes { get; set; }

        public int? DurationHours { get; set; }

        public string PDActivityType { get; set; }

        public string LearningMode { get; set; }

        public string ExternalCode { get; set; }

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

        // Course Registration Conditions
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

        public int? NumOfHoursPerSession { get; set; }

        public int? NumOfMinutesPerSession { get; set; }

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

        public IEnumerable<Guid> CourseFacilitatorIds { get; set; }

        public IEnumerable<Guid> CourseCoFacilitatorIds { get; set; }

        public IEnumerable<Guid> CollaborativeContentCreatorIds { get; set; }

        public string CourseApprovalComment { get; set; }

        public Guid? CoursePlanningCycleId { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public SaveCourseCommand ToCommand()
        {
            return new SaveCourseCommand()
            {
                Status = Status,
                Id = Id ?? Guid.NewGuid(),
                IsCreate = !Id.HasValue,
                ThumbnailUrl = ThumbnailUrl,
                CourseName = CourseName,
                DurationMinutes = DurationMinutes,
                DurationHours = DurationHours,
                PDActivityType = PDActivityType,
                LearningMode = LearningMode,
                ExternalCode = ExternalCode,
                CourseOutlineStructure = CourseOutlineStructure,
                CourseObjective = CourseObjective,
                Description = Description,
                CategoryIds = CategoryIds,
                OwnerDivisionIds = OwnerDivisionIds,
                OwnerBranchIds = OwnerBranchIds,
                PartnerOrganisationIds = PartnerOrganisationIds,
                MOEOfficerId = MOEOfficerId,
                MOEOfficerPhoneNumber = MOEOfficerPhoneNumber,
                MOEOfficerEmail = MOEOfficerEmail,
                NotionalCost = NotionalCost,
                CourseFee = CourseFee,
                TrainingAgency = TrainingAgency,
                OtherTrainingAgencyReason = OtherTrainingAgencyReason,
                AllowPersonalDownload = AllowPersonalDownload,
                AllowNonCommerInMOEReuseWithModification = AllowNonCommerInMOEReuseWithModification,
                AllowNonCommerReuseWithModification = AllowNonCommerReuseWithModification,
                AllowNonCommerInMoeReuseWithoutModification = AllowNonCommerInMoeReuseWithoutModification,
                AllowNonCommerReuseWithoutModification = AllowNonCommerReuseWithoutModification,
                CopyrightOwner = CopyrightOwner,
                AcknowledgementAndCredit = AcknowledgementAndCredit,
                MaximumPlacesPerSchool = MaximumPlacesPerSchool,
                NumOfSchoolLeader = NumOfSchoolLeader,
                NumOfSeniorOrLeadTeacher = NumOfSeniorOrLeadTeacher,
                NumOfMiddleManagement = NumOfMiddleManagement,
                NumOfBeginningTeacher = NumOfBeginningTeacher,
                NumOfExperiencedTeacher = NumOfExperiencedTeacher,
                PlaceOfWork = PlaceOfWork,
                PrerequisiteCourseIds = PrerequisiteCourseIds,
                ApplicableDivisionIds = ApplicableDivisionIds,
                ApplicableBranchIds = ApplicableBranchIds,
                ApplicableClusterIds = ApplicableClusterIds,
                ApplicableSchoolIds = ApplicableSchoolIds,
                TrackIds = TrackIds,
                DevelopmentalRoleIds = DevelopmentalRoleIds,
                TeachingLevels = TeachingLevels,
                TeachingSubjectIds = TeachingSubjectIds,
                TeachingCourseStudyIds = TeachingCourseStudyIds,
                CocurricularActivityIds = CocurricularActivityIds,
                JobFamily = JobFamily,
                EasSubstantiveGradeBandingIds = EasSubstantiveGradeBandingIds,
                ApplicableZoneIds = ApplicableZoneIds,
                CourseLevel = CourseLevel,
                PDAreaThemeId = PDAreaThemeId,
                ServiceSchemeIds = ServiceSchemeIds,
                SubjectAreaIds = SubjectAreaIds,
                LearningFrameworkIds = LearningFrameworkIds,
                LearningDimensionIds = LearningDimensionIds,
                LearningAreaIds = LearningAreaIds,
                LearningSubAreaIds = LearningSubAreaIds,
                TeacherOutcomeIds = TeacherOutcomeIds,
                MetadataKeys = MetadataKeys,
                NatureOfCourse = NatureOfCourse,
                NumOfPlannedClass = NumOfPlannedClass,
                NumOfSessionPerClass = NumOfSessionPerClass,
                NumOfHoursPerSession = NumOfHoursPerSession,
                NumOfMinutesPerSession = NumOfMinutesPerSession,
                MinParticipantPerClass = MinParticipantPerClass,
                MaxParticipantPerClass = MaxParticipantPerClass,
                PlanningPublishDate = PlanningPublishDate,
                PlanningArchiveDate = PlanningArchiveDate,
                WillArchiveCommunity = WillArchiveCommunity,
                ArchiveDate = ArchiveDate,
                MaxReLearningTimes = MaxReLearningTimes,
                StartDate = StartDate,
                ExpiredDate = ExpiredDate,
                CourseType = CourseType,
                PdActivityPeriods = PdActivityPeriods,
                PreCourseEvaluationFormId = PreCourseEvaluationFormId,
                PostCourseEvaluationFormId = PostCourseEvaluationFormId,
                ECertificateTemplateId = ECertificateTemplateId,
                ECertificatePrerequisite = ECertificatePrerequisite,
                CourseNameInECertificate = CourseNameInECertificate,
                FirstAdministratorId = FirstAdministratorId,
                SecondAdministratorId = SecondAdministratorId,
                PrimaryApprovingOfficerId = PrimaryApprovingOfficerId,
                AlternativeApprovingOfficerId = AlternativeApprovingOfficerId,
                CollaborativeContentCreatorIds = CollaborativeContentCreatorIds,
                CourseFacilitatorIds = CourseFacilitatorIds,
                CourseCoFacilitatorIds = CourseCoFacilitatorIds,
                Remarks = Remarks,
                CoursePlanningCycleId = CoursePlanningCycleId,
                RegistrationMethod = RegistrationMethod,
                NieAcademicGroups = NieAcademicGroups
            };
        }
    }
}
