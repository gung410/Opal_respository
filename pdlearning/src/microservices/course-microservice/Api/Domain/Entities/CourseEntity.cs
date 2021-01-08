using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.Interfaces;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Core.Validation;
using Thunder.Service.Authentication;

#pragma warning disable SA1124 // Do not use regions
namespace Microservice.Course.Domain.Entities
{
    public class CourseEntity : FullAuditedEntity, ISoftDelete, IHasDepartment, IEqualityComparer<CourseEntity>, IFullTextSearchable
    {
        private IEnumerable<Guid> _collaborativeContentCreatorIds = new List<Guid>();
        private IEnumerable<Guid> _courseFacilitatorIds = new List<Guid>();
        private IEnumerable<Guid> _courseCoFacilitatorIds = new List<Guid>();
        private IEnumerable<int> _ownerDivisionIds = new List<int>();
        private IEnumerable<int> _ownerBranchIds = new List<int>();
        private IEnumerable<int> _partnerOrganisationIds = new List<int>();
        private IEnumerable<string> _trainingAgency = new List<string>();
        private IEnumerable<string> _otherTrainingAgencyReason = new List<string>();
        private IEnumerable<string> _nieAcademicGroups = new List<string>();
        private IEnumerable<Guid> _prerequisiteCourseIds = new List<Guid>();
        private IEnumerable<int> _applicableDivisionIds = new List<int>();
        private IEnumerable<int> _applicableBranchIds = new List<int>();
        private IEnumerable<int> _applicableZoneIds = new List<int>();
        private IEnumerable<int> _applicableClusterIds = new List<int>();
        private IEnumerable<int> _applicableSchoolIds = new List<int>();
        private IEnumerable<string> _developmentalRoleIds = new List<string>();
        private IEnumerable<string> _easSubstantiveGradeBandingIds = new List<string>();
        private IEnumerable<string> _serviceSchemeIds = new List<string>();
        private IEnumerable<string> _subjectAreaIds = new List<string>();
        private IEnumerable<string> _learningFrameworkIds = new List<string>();
        private IEnumerable<string> _learningDimensionIds = new List<string>();
        private IEnumerable<string> _learningAreaIds = new List<string>();
        private IEnumerable<string> _learningSubAreaIds = new List<string>();
        private IEnumerable<string> _teacherOutcomeIds = new List<string>();
        private IEnumerable<string> _trackIds = new List<string>();
        private IEnumerable<string> _categoryIds = new List<string>();
        private IEnumerable<string> _jobFamily = new List<string>();
        private IEnumerable<string> _teachingCourseStudyIds = new List<string>();
        private IEnumerable<string> _teachingLevels = new List<string>();
        private IEnumerable<string> _teachingSubjectIds = new List<string>();
        private IEnumerable<string> _cocurricularActivityIds = new List<string>();
        private IEnumerable<string> _pdActivityPeriods = new List<string>();
        private IEnumerable<string> _metadataKeys = new List<string>();
        private int _maxReLearningTimes = 0;

        // Overview Information
        public string ThumbnailUrl { get; set; }

        public string CourseName { get; set; }

        public int? DurationMinutes { get; set; }

        public int? DurationHours { get; set; }

        public string LearningMode { get; set; }

        public string CourseCode { get; set; }

        public string CourseOutlineStructure { get; set; }

        public string CourseObjective { get; set; }

        public string Description { get; set; }

        // Provider Information
        public IEnumerable<int> OwnerDivisionIds
        {
            get => _ownerDivisionIds ?? new List<int>();

            set
            {
                _ownerDivisionIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.OwnerDivisionIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.OwnerDivisionIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<int> OwnerBranchIds
        {
            get => _ownerBranchIds ?? new List<int>();

            set
            {
                _ownerBranchIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.OwnerBranchIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.OwnerBranchIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<int> PartnerOrganisationIds
        {
            get => _partnerOrganisationIds ?? new List<int>();

            set
            {
                _partnerOrganisationIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.PartnerOrganisationIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.PartnerOrganisationIds, p))
                        .ToList());
                }
            }
        }

        public Guid? MOEOfficerId { get; set; }

        public string MOEOfficerPhoneNumber { get; set; }

        public string MOEOfficerEmail { get; set; }

        public decimal? NotionalCost { get; set; }

        public decimal? CourseFee { get; set; }

        public IEnumerable<string> TrainingAgency
        {
            get => _trainingAgency ?? new List<string>();

            set
            {
                _trainingAgency = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.TrainingAgency);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.TrainingAgency, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> OtherTrainingAgencyReason
        {
            get => _otherTrainingAgencyReason ?? new List<string>();

            set
            {
                _otherTrainingAgencyReason = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.OtherTrainingAgencyReason);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.OtherTrainingAgencyReason, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> NieAcademicGroups
        {
            get => _nieAcademicGroups ?? new List<string>();

            set
            {
                _nieAcademicGroups = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.NieAcademicGroups);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.NieAcademicGroups, p))
                        .ToList());
                }
            }
        }

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

        public RegistrationMethod? RegistrationMethod { get; set; }

        public PlaceOfWorkType PlaceOfWork { get; set; }

        public IEnumerable<Guid> PrerequisiteCourseIds
        {
            get => _prerequisiteCourseIds ?? new List<Guid>();

            set
            {
                _prerequisiteCourseIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.PrerequisiteCourseIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.PrerequisiteCourseIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<int> ApplicableDivisionIds
        {
            get => _applicableDivisionIds ?? new List<int>();

            set
            {
                _applicableDivisionIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.ApplicableDivisionIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.ApplicableDivisionIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<int> ApplicableBranchIds
        {
            get => _applicableBranchIds ?? new List<int>();

            set
            {
                _applicableBranchIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.ApplicableBranchIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.ApplicableBranchIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<int> ApplicableZoneIds
        {
            get => _applicableZoneIds ?? new List<int>();

            set
            {
                _applicableZoneIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.ApplicableZoneIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.ApplicableZoneIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<int> ApplicableClusterIds
        {
            get => _applicableClusterIds ?? new List<int>();

            set
            {
                _applicableClusterIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.ApplicableClusterIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.ApplicableClusterIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<int> ApplicableSchoolIds
        {
            get => _applicableSchoolIds ?? new List<int>();

            set
            {
                _applicableSchoolIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.ApplicableSchoolIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.ApplicableSchoolIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> DevelopmentalRoleIds
        {
            get => _developmentalRoleIds ?? new List<string>();

            set
            {
                _developmentalRoleIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.DevelopmentalRoleIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.DevelopmentalRoleIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> EasSubstantiveGradeBandingIds
        {
            get => _easSubstantiveGradeBandingIds ?? new List<string>();

            set
            {
                _easSubstantiveGradeBandingIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.EasSubstantiveGradeBandingIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.EasSubstantiveGradeBandingIds, p))
                        .ToList());
                }
            }
        }

        // Metadata
        public string PDActivityType { get; set; }

        public string CourseLevel { get; set; }

        public string PDAreaThemeId { get; set; }

        public string PDAreaThemeCode { get; set; }

        public IEnumerable<string> MetadataKeys
        {
            get => _metadataKeys ?? new List<string>();

            set
            {
                _metadataKeys = value;
            }
        }

        public IEnumerable<string> ServiceSchemeIds
        {
            get => _serviceSchemeIds ?? new List<string>();

            set
            {
                _serviceSchemeIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.ServiceSchemeIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.ServiceSchemeIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> SubjectAreaIds
        {
            get => _subjectAreaIds ?? new List<string>();

            set
            {
                _subjectAreaIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.SubjectAreaIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.SubjectAreaIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> LearningFrameworkIds
        {
            get => _learningFrameworkIds ?? new List<string>();

            set
            {
                _learningFrameworkIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.LearningFrameworkIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.LearningFrameworkIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> LearningDimensionIds
        {
            get => _learningDimensionIds ?? new List<string>();

            set
            {
                _learningDimensionIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.LearningDimensionIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.LearningDimensionIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> LearningAreaIds
        {
            get => _learningAreaIds ?? new List<string>();

            set
            {
                _learningAreaIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.LearningAreaIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.LearningAreaIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> LearningSubAreaIds
        {
            get => _learningSubAreaIds ?? new List<string>();

            set
            {
                _learningSubAreaIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.LearningSubAreaIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.LearningSubAreaIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> TeacherOutcomeIds
        {
            get => _teacherOutcomeIds ?? new List<string>();

            set
            {
                _teacherOutcomeIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.TeacherOutcomeIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.TeacherOutcomeIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> TrackIds
        {
            get => _trackIds ?? new List<string>();

            set
            {
                _trackIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.TrackIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.TrackIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> CategoryIds
        {
            get => _categoryIds ?? new List<string>();

            set
            {
                _categoryIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.CategoryIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.CategoryIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> JobFamily
        {
            get => _jobFamily ?? new List<string>();

            set
            {
                _jobFamily = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.JobFamily);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.JobFamily, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> TeachingCourseStudyIds
        {
            get => _teachingCourseStudyIds ?? new List<string>();

            set
            {
                _teachingCourseStudyIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.TeachingCourseStudyIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.TeachingCourseStudyIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> TeachingLevels
        {
            get => _teachingLevels ?? new List<string>();

            set
            {
                _teachingLevels = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.TeachingLevels);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.TeachingLevels, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> TeachingSubjectIds
        {
            get => _teachingSubjectIds ?? new List<string>();

            set
            {
                _teachingSubjectIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.TeachingSubjectIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.TeachingSubjectIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> CocurricularActivityIds
        {
            get => _cocurricularActivityIds ?? new List<string>();

            set
            {
                _cocurricularActivityIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.CocurricularActivityIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.CocurricularActivityIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<string> PdActivityPeriods
        {
            get => _pdActivityPeriods ?? new List<string>();

            set
            {
                _pdActivityPeriods = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.PdActivityPeriods);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.PdActivityPeriods, p))
                        .ToList());
                }
            }
        }

        // Course Planning Information
        public string NatureOfCourse { get; set; }

        public int? NumOfPlannedClass { get; set; }

        public int? NumOfSessionPerClass { get; set; }

        public double? NumOfHoursPerClass
        {
            get { return (NumOfSessionPerClass ?? 0) * ((NumOfHoursPerSession ?? 0) + (((NumOfMinutesPerSession ?? 0) * 1.0) / 60)); }
            private set { }
        }

        public int? NumOfHoursPerSession { get; set; }

        public int? NumOfMinutesPerSession { get; set; }

        public int? MinParticipantPerClass { get; set; }

        public int? MaxParticipantPerClass { get; set; }

        public DateTime? PlanningPublishDate { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime? PlanningArchiveDate { get; set; }

        public bool WillArchiveCommunity { get; set; }

        public CourseType CourseType { get; set; }

        public int MaxReLearningTimes
        {
            get => _maxReLearningTimes <= 0 ? int.MaxValue : _maxReLearningTimes;
            set { _maxReLearningTimes = value; }
        }

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

        public IEnumerable<Guid> CollaborativeContentCreatorIds
        {
            get => _collaborativeContentCreatorIds;

            set
            {
                _collaborativeContentCreatorIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.CollaborativeContentCreatorIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.CollaborativeContentCreatorIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<Guid> CourseFacilitatorIds
        {
            get => _courseFacilitatorIds;

            set
            {
                _courseFacilitatorIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.CourseFacilitatorIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.CourseFacilitatorIds, p))
                        .ToList());
                }
            }
        }

        public IEnumerable<Guid> CourseCoFacilitatorIds
        {
            get => _courseCoFacilitatorIds;

            set
            {
                _courseCoFacilitatorIds = value;
                CourseInternalValues.RemoveAll(p => p.Type == CourseInternalValueType.CourseCoFacilitatorIds);
                if (value != null)
                {
                    CourseInternalValues.AddRange(value
                        .Select(p => CourseInternalValue.Create(Id, CourseInternalValueType.CourseCoFacilitatorIds, p))
                        .ToList());
                }
            }
        }

        // System fields
        public CourseStatus Status { get; set; } = CourseStatus.Draft;

        public ContentStatus ContentStatus { get; set; } = ContentStatus.Draft;

        public DateTime? PublishedContentDate { get; set; }

        public DateTime? SubmittedContentDate { get; set; }

        public string Version { get; set; }

        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid ChangedBy { get; set; }

        public string ExternalId { get; set; }

        public string ExternalCode { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public DateTime? ApprovalContentDate { get; set; }

        public DateTime? PublishDate { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public Guid? ArchivedBy { get; set; }

        // TODO: Remove in 3.0. OPX-3428 Temp solution for 2.2
        public bool IsLearnerStarted { get; set; }

        /*
         * This field is created to handle courses which are migrated from Traisy or Opal 1
         * Set it is true for migrated course
         * If true, we allow user edit course with status is unpublished without validating that there is ANY class run started
         * Need to remove it if there is NOT any course has IsMigrated = true in production database in release 2.1.
         * It means all migrated courses are edited and submit for AO review
         */

        public DateTime? StartDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public int DepartmentId { get; set; }

        public Guid? CoursePlanningCycleId { get; set; }

        public DateTime? VerifiedDate { get; set; }

        public bool IsMigrated { get; set; }

        public int VersionNo { get; set; }

        public double TotalHoursAttendWithinYear
        {
            get { return (NumOfHoursPerClass ?? 0) * (NumOfPlannedClass ?? 0); }
            private set { }
        }

        [JsonIgnore]
        public virtual ICollection<CourseInternalValue> CourseInternalValues { get; set; } = new List<CourseInternalValue>();

        // Technical Columns
        #region FullTextSearch

        /// <summary>
        /// This column to support search by text function in UI. This is a computed data column from CourseName, CourseCode and ExternalCode.
        /// </summary>
        public string FullTextSearch
        {
            // Adding double space between CourseName and CourseCode because some records SQL cannot create full-text index for it and user cannot search exact course's name.
            get => $"{CourseName ?? string.Empty}  {CourseCode ?? string.Empty} {ExternalCode ?? string.Empty}";
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to CourseFacilitatorIds.Contain([some user id]) by using full-text search.
        /// </summary>
        public string CourseFacilitatorIdsFullTextSearch
        {
            get => CourseFacilitatorIds != null && CourseFacilitatorIds.Any() ? JsonSerializer.Serialize(CourseFacilitatorIds) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to CourseCoFacilitatorIds.Contain([some user id]) by using full-text search.
        /// </summary>
        public string CourseCoFacilitatorIdsFullTextSearch
        {
            get => CourseCoFacilitatorIds != null && CourseCoFacilitatorIds.Any() ? JsonSerializer.Serialize(CourseCoFacilitatorIds) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to CollaborativeContentCreatorIds.Contain([some user id]) by using full-text search.
        /// </summary>
        public string CollaborativeContentCreatorIdsFullTextSearch
        {
            get => CollaborativeContentCreatorIds != null && CollaborativeContentCreatorIds.Any() ? JsonSerializer.Serialize(CollaborativeContentCreatorIds) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to DevelopmentalRoleIds.Contain([some user id]) by using full-text search.
        /// </summary>
        public string DevelopmentalRoleIdsFullTextSearch
        {
            get => DevelopmentalRoleIds != null && DevelopmentalRoleIds.Any() ? JsonSerializer.Serialize(DevelopmentalRoleIds) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to ServiceSchemeIds.Contain([some user id]) by using full-text search.
        /// </summary>
        public string ServiceSchemeIdsFullTextSearch
        {
            get => ServiceSchemeIds != null && ServiceSchemeIds.Any() ? JsonSerializer.Serialize(ServiceSchemeIds) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to SubjectAreaIds.Contain([some user id]) by using full-text search.
        /// </summary>
        public string SubjectAreaIdsFullTextSearch
        {
            get => SubjectAreaIds != null && SubjectAreaIds.Any() ? JsonSerializer.Serialize(SubjectAreaIds) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to LearningFrameworkIds.Contain([some user id]) by using full-text search.
        /// </summary>
        public string LearningFrameworkIdsFullTextSearch
        {
            get => LearningFrameworkIds != null && LearningFrameworkIds.Any() ? JsonSerializer.Serialize(LearningFrameworkIds) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to LearningDimensionIds.Contain([some user id]) by using full-text search.
        /// </summary>
        public string LearningDimensionIdsFullTextSearch
        {
            get => LearningDimensionIds != null && LearningDimensionIds.Any() ? JsonSerializer.Serialize(LearningDimensionIds) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to LearningAreaIds.Contain([some user id]) by using full-text search.
        /// </summary>
        public string LearningAreaIdsFullTextSearch
        {
            get => LearningAreaIds != null && LearningAreaIds.Any() ? JsonSerializer.Serialize(LearningAreaIds) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to LearningSubAreaIds.Contain([some user id]) by using full-text search.
        /// </summary>
        public string LearningSubAreaIdsFullTextSearch
        {
            get => LearningSubAreaIds != null && LearningSubAreaIds.Any() ? JsonSerializer.Serialize(LearningSubAreaIds) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to CategoryIds.Contain([some user id]) by using full-text search.
        /// </summary>
        public string CategoryIdsFullTextSearch
        {
            get => CategoryIds != null && CategoryIds.Any() ? JsonSerializer.Serialize(CategoryIds) : null;
            set { }
        }

        /// <summary>
        /// This column to support filter equivalent to TeachingLevels.Contain([some user id]) by using full-text search.
        /// </summary>
        public string TeachingLevelsFullTextSearch
        {
            get => TeachingLevels != null && TeachingLevels.Any() ? JsonSerializer.Serialize(TeachingLevels) : null;
            set { }
        }

        /// <summary>
        /// This column is a unique full-text search key column. It also support OrderBy. For now we order by CreatedDate.
        /// </summary>
        public string FullTextSearchKey
        {
            get => $"{CreatedDate:yyyy-MM-dd hh:mm:ss.fffffff}_{Id.ToString().ToUpperInvariant()}";
            set { }
        }

        #endregion

        // Permission Expressions
        public static Expression<Func<CourseEntity, bool>> HasOwnerPermissionExpr(Guid? userId, IEnumerable<string> userRoles)
        {
            return x => userId == null || UserRoles.IsSysAdministrator(userRoles) || x.CreatedBy == userId;
        }

        public static Expression<Func<CourseEntity, bool>> HasApprovalPermissionExpr(Guid? userId, IEnumerable<string> userRoles)
        {
            return x => userId == null
                || UserRoles.IsSysAdministrator(userRoles)
                || x.AlternativeApprovingOfficerId == userId
                || x.PrimaryApprovingOfficerId == userId;
        }

        public static Expression<Func<CourseEntity, bool>> HasFacilitatorPermissionExpr(Guid? userId, IEnumerable<string> userRoles)
        {
            return x => userId == null
                        || UserRoles.IsSysAdministrator(userRoles)
                        || x.CourseFacilitatorIds.Contains(userId.Value)
                        || x.CourseCoFacilitatorIds.Contains(userId.Value);
        }

        public static Expression<Func<CourseEntity, bool>> IsFacilitatorExpr(Guid? userId)
        {
            return x => x.CourseFacilitatorIds.Contains(userId.Value)
                        || x.CourseCoFacilitatorIds.Contains(userId.Value);
        }

        public static Expression<Func<CourseEntity, bool>> HasAdministrationPermissionExpr(Guid? userId, IEnumerable<string> userRoles)
        {
            return x => userId == null
                || UserRoles.IsSysAdministrator(userRoles)
                || (x.FirstAdministratorId == userId || x.SecondAdministratorId == userId);
        }

        public static Expression<Func<CourseEntity, bool>> HasContentCreatorPermissionExpr(
            Guid? userId,
            IEnumerable<string> userRoles)
        {
            return x => userId == null
                        || UserRoles.IsSysAdministrator(userRoles)
                        || userId == x.CreatedBy
                        || x.CollaborativeContentCreatorIds.Contains(userId.Value);
        }

        public static Expression<Func<CourseEntity, bool>> HasPublishUnpublishPermissionExpr(Guid? userId, IEnumerable<string> userRoles, Func<string, bool> checkHasPermissionFn)
        {
            return x => userId == null
                        || UserRoles.IsSysAdministrator(userRoles)
                        || (checkHasPermissionFn(CourseAdminManagementPermissionKeys.PublishUnpublishCourse)
                            && (x.CollaborativeContentCreatorIds.Contains(userId.Value) || x.CreatedBy == userId));
        }

        public static Expression<Func<CourseEntity, bool>> HasUpdatePermissionExpr(Guid? userId, List<string> userRoles, Func<string, bool> checkHasPermissionFn)
        {
            Expression<Func<CourseEntity, bool>> expr1 = x =>
                userId == null
                || UserRoles.IsAdministrator(userRoles)
                || userRoles.Any(r =>
                    r == UserRoles.MOEHQContentApprovingOfficer ||
                    r == UserRoles.SchoolContentApprovingOfficer);
            return expr1.Or(HasContentCreatorPermissionExpr(userId, userRoles).AndAlso(x => checkHasPermissionFn(CourseAdminManagementPermissionKeys.CreateEditCourse)));
        }

        public static Expression<Func<CourseEntity, bool>> HasAddingParticipantsPermissionExpr(Guid? userId, IEnumerable<string> userRoles)
        {
            return x => userId != null &&
                        (UserRoles.IsAdministrator(userRoles)
                        || x.FirstAdministratorId == userId
                        || x.SecondAdministratorId == userId);
        }

        public static Expression<Func<CourseEntity, bool>> HasMarkScorePermissionExpr(Guid? userId, IEnumerable<string> userRoles, Func<string, bool> checkHasPermissionFn)
        {
            return x => userId == null
                        || UserRoles.IsAdministrator(userRoles)
                        || ((x.CourseFacilitatorIds.Contains(userId.Value)
                            || x.CourseCoFacilitatorIds.Contains(userId.Value)) && checkHasPermissionFn(LearningManagementPermissionKeys.ScoreGivingAssignment));
        }

        public static Expression<Func<CourseEntity, bool>> HasArchivalPermissionExpr(Guid? userId, IEnumerable<string> userRoles)
        {
            return x => userId == null
                        || UserRoles.IsAdministrator(userRoles)
                        || x.CreatedBy == userId.Value;
        }

        // Others
        public static Expression<Func<CourseEntity, bool>> IsNotArchivedExpr()
        {
            return IsArchivedExpr().Not();
        }

        public static Expression<Func<CourseEntity, bool>> IsArchivedExpr()
        {
            return x => x.Status == CourseStatus.Archived;
        }

        public static Expression<Func<CourseEntity, bool>> IsNotELearningExpr()
        {
            return IsELearningExpr().Not();
        }

        public static Expression<Func<CourseEntity, bool>> IsELearningExpr()
        {
            return x => x.LearningMode == MetadataTagConstants.ELearningTagId;
        }

        // State on Course expression
        public static Expression<Func<CourseEntity, bool>> IsEditableExpr(CourseType? courseType)
        {
            Expression<Func<CourseEntity, bool>> notPublishedAndCompleted =
                course => course.Status != CourseStatus.Published && course.Status != CourseStatus.Completed;
            Expression<Func<CourseEntity, bool>> migratedOrNotStarted =
                course => course.IsMigrated;
            Expression<Func<CourseEntity, bool>> recurringAndCompleted =
                course => courseType == CourseType.Recurring && course.Status == CourseStatus.Completed;

            return notPublishedAndCompleted.Or(recurringAndCompleted);
        }

        public static ExpressionValidator<CourseEntity> CanEditContentValidator()
        {
            Expression<Func<CourseEntity, bool>> notPublishedContent = course => course.ContentStatus != ContentStatus.Published;
            Expression<Func<CourseEntity, bool>> notCompleted = course => course.Status != CourseStatus.Completed;
            return new ExpressionValidator<CourseEntity>(
                notPublishedContent.AndAlso(notCompleted).AndAlso(IsNotArchivedExpr()),
                "Course content must be not published and Course must be not Completed/Archived");
        }

        public static Expression<Func<CourseEntity, bool>> IsPlanningVerificationRequiredExpr()
        {
            return course => course.CoursePlanningCycleId != null && course.VerifiedDate == null;
        }

        public static ExpressionValidator<CourseEntity> CanPublishValidator()
        {
            Expression<Func<CourseEntity, bool>> validStatusExpr = course => course.Status == CourseStatus.Unpublished || course.Status == CourseStatus.PlanningCycleCompleted;
            Expression<Func<CourseEntity, bool>> isSubmittedApprovedExpr = course => course.Status == CourseStatus.Approved && course.SubmittedDate != null;
            Expression<Func<CourseEntity, bool>> isApprovedAndNotIsPlanningVerificationRequiredExpr = isSubmittedApprovedExpr.AndAlso(IsPlanningVerificationRequiredExpr().Not());

            return new ExpressionValidator<CourseEntity>(
                validStatusExpr.Or(isApprovedAndNotIsPlanningVerificationRequiredExpr),
                "Course must be Unpublished/PlanningCycleCompleted or course is Approved and not planning verification required.");
        }

        public static Expression<Func<CourseEntity, bool>> AfterApprovingExpr()
        {
            return course => course.Status == CourseStatus.Approved
                             || course.Status == CourseStatus.PlanningCycleVerified
                             || course.Status == CourseStatus.PlanningCycleCompleted
                             || course.Status == CourseStatus.Published
                             || course.Status == CourseStatus.Unpublished
                             || course.Status == CourseStatus.Completed
                             || course.Status == CourseStatus.Archived;
        }

        public static Expression<Func<CourseEntity, bool>> StartedExpr()
        {
            return course => course.StartDate != null && course.StartDate.Value <= Clock.Now;
        }

        public static Expression<Func<CourseEntity, bool>> IsMicroLearningExpr()
        {
            return course => course.PDActivityType == MetadataTagConstants.MicroLearningTagId;
        }

        public static Expression<Func<CourseEntity, bool>> IsNotMicroLearningExpr()
        {
            return IsMicroLearningExpr().Not();
        }

        public static Expression<Func<CourseEntity, bool>> HasAfterVerificationEditPermissionExpr(Guid? userId, IEnumerable<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return x => x.CoursePlanningCycleId != null && (x.Status == CourseStatus.PlanningCycleVerified || x.Status == CourseStatus.PlanningCycleCompleted) && x.HasContentCreatorPermission(userId, userRoles, haveFullRight);
        }

        public static Expression<Func<CourseEntity, bool>> HasPlanningCycleVerificationPermissionExpr(Guid? userId, List<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return x => x.CoursePlanningCycleId != null && x.Status == CourseStatus.Approved && (userId == null || UserRoles.IsSysAdministrator(userRoles) || userRoles.Any(p => p == UserRoles.CoursePlanningCoordinator) || haveFullRight(x));
        }

        public static ExpressionValidator<CourseEntity> CanApproveRejectContentValidator()
        {
            Expression<Func<CourseEntity, bool>> pendingApprovalExpr = course => course.ContentStatus == ContentStatus.PendingApproval;
            return new ExpressionValidator<CourseEntity>(
                pendingApprovalExpr.AndAlso(IsNotArchivedExpr()),
                "ContentStatus must be PendingApproval and course is not archived");
        }

        public static Expression<Func<CourseEntity, bool>> PendingApprovalSubmittedDaysAgoFromNow(int days)
        {
            return course => course.Status == CourseStatus.PendingApproval && course.SubmittedDate >= Clock.Now.AddDays(-days - 1) && course.SubmittedDate < Clock.Now.AddDays(-days);
        }

        public static Expression<Func<CourseEntity, bool>> ContentPendingApprovalSubmittedDaysAgoFromNow(int days)
        {
            return course => course.ContentStatus == ContentStatus.PendingApproval && course.SubmittedDate >= Clock.Now.AddDays(-days - 1) && course.SubmittedDate < Clock.Now.AddDays(-days);
        }

        public static ExpressionValidator<CourseEntity> CanPublishContentValidator()
        {
            Expression<Func<CourseEntity, bool>> approvedOrUnpublishedExpr = course =>
                course.ContentStatus == ContentStatus.Approved ||
                course.ContentStatus == ContentStatus.Unpublished;
            return new ExpressionValidator<CourseEntity>(
                approvedOrUnpublishedExpr.AndAlso(IsNotArchivedExpr()),
                "ContentStatus must be Approved/Unpublished and course is not Archived");
        }

        public static ExpressionValidator<CourseEntity> CanUnpublishContentValidator()
        {
            Expression<Func<CourseEntity, bool>> published = course => course.ContentStatus == ContentStatus.Published;
            return new ExpressionValidator<CourseEntity>(
                published.AndAlso(IsNotArchivedExpr()),
                "ContentStatus must be Published and course is not Archived");
        }

        public static ExpressionValidator<CourseEntity> ImportParticipantValidator()
        {
            Expression<Func<CourseEntity, bool>> publishedExpr = course => course.Status == CourseStatus.Published;
            var finalExpr = publishedExpr.AndAlsoNot(IsMicroLearningExpr()).AndAlso(IsNotArchivedExpr());
            return new ExpressionValidator<CourseEntity>(
                finalExpr,
                "Can only import participant for Published,not Microlearning and not archived course.");
        }

        public static Expression<Func<CourseEntity, bool>> OnlyForNominatedRegistrationExpr()
        {
            return x => x.RegistrationMethod == Enums.RegistrationMethod.Private;
        }

        public static Expression<Func<CourseEntity, bool>> CanByPassCAConfirmedExpr()
        {
            return CanByPassApprovalExpr().AndAlso(p => p.LearningMode == MetadataTagConstants.ELearningTagId);
        }

        public static Expression<Func<CourseEntity, bool>> CanByPassApprovalExpr()
        {
            return x => x.RegistrationMethod == Enums.RegistrationMethod.Public;
        }

        public static ExpressionValidator<CourseEntity> CanCloneValidator()
        {
            var expr = IsNotArchivedExpr().OrNot(x => x.ArchiveDate.HasValue).Or(x => x.ArchiveDate.Value >= Clock.Now.AddYears(-3));
            return new ExpressionValidator<CourseEntity>(expr, "Course must be not archived or archived date less than 3 years from now.");
        }

        public static Expression<Func<CourseEntity, bool>> CanBeArchivedExpr(bool isAnyInProgressLearner)
        {
            Expression<Func<CourseEntity, bool>> expr1 =
                x => !isAnyInProgressLearner &&
                     (x.Status == CourseStatus.Approved ||
                     x.Status == CourseStatus.Rejected ||
                     x.Status == CourseStatus.Draft ||
                     x.Status == CourseStatus.Unpublished);

            return expr1.AndAlsoNot(IsPendingCoursePlanningCycleVerifiedExpr());
        }

        public static Expression<Func<CourseEntity, bool>> IsPendingCoursePlanningCycleVerifiedExpr()
        {
            return x => x.CoursePlanningCycleId != null && x.Status == CourseStatus.Approved;
        }

        public static ExpressionValidator<CourseEntity> CanArchiveCourseValidator(bool isAnyInProgressLearner)
        {
            return new ExpressionValidator<CourseEntity>(
                CanBeArchivedExpr(isAnyInProgressLearner),
                "Unable to archive this course since there are still in-progress learning learner or status is invalid");
        }

        public static ExpressionValidator<CourseEntity> CanBeApprovalValidator()
        {
            return new ExpressionValidator<CourseEntity>(course => course.Status == CourseStatus.PendingApproval, "Can only approve/reject a pending approval course.");
        }

        public static Expression<Func<CourseEntity, bool>> CanSubmitForApprovalExpr()
        {
            return course => (course.Status != CourseStatus.Completed || course.CourseType == CourseType.Recurring) &&
                             course.Status != CourseStatus.Published &&
                             course.Status != CourseStatus.PlanningCycleVerified &&
                             course.Status != CourseStatus.PlanningCycleCompleted;
        }

        public static Expression<Func<CourseEntity, bool>> CanCompletePlanningExpr()
        {
            return course => course.Status == CourseStatus.PlanningCycleVerified;
        }

        public static Expression<Func<CourseEntity, bool>> CanCompleteCourseExpr()
        {
            return course => course.Status == CourseStatus.Published;
        }

        public static ExpressionValidator<CourseEntity> CanUnpublishCourseValidator()
        {
            return new ExpressionValidator<CourseEntity>(
                course => course.Status == CourseStatus.Published,
                "Course must be published");
        }

        public static Expression<Func<CourseEntity, bool>> CanVerifyPlanningCycleOrRejectVerificationExpr()
        {
            return course =>
                course.CoursePlanningCycleId != null &&
                (course.Status == CourseStatus.Approved ||
                 course.Status == CourseStatus.PlanningCycleVerified ||
                 course.Status == CourseStatus.PlanningCycleCompleted);
        }

        public static Expression<Func<CourseEntity, bool>> HasViewContentPermissionExpr(Guid? userId, IEnumerable<string> userRoles)
        {
            return x =>
                userId == null ||
                UserRoles.IsSysAdministrator(userRoles) ||
                (x.CreatedBy == userId
                 || x.FirstAdministratorId == userId
                 || x.SecondAdministratorId == userId
                 || x.PrimaryApprovingOfficerId == userId
                 || x.AlternativeApprovingOfficerId == userId
                 || x.CollaborativeContentCreatorIds.Contains(userId.Value)
                 || x.CourseCoFacilitatorIds.Contains(userId.Value)
                 || x.CourseCoFacilitatorIds.Contains(userId.Value));
        }

        public bool IsNotArchived()
        {
            return IsNotArchivedExpr().Compile()(this);
        }

        public bool IsArchived()
        {
            return IsArchivedExpr().Compile()(this);
        }

        public bool IsMicroLearning()
        {
            return IsMicroLearningExpr().Compile()(this);
        }

        public bool IsNotMicroLearning()
        {
            return IsNotMicroLearningExpr().Compile()(this);
        }

        public bool IsNotELearning()
        {
            return IsNotELearningExpr().Compile()(this);
        }

        public bool IsELearning()
        {
            return IsELearningExpr().Compile()(this);
        }

        public bool HasOwnerPermission(Guid? userId, IEnumerable<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return HasOwnerPermissionExpr(userId, userRoles).Compile()(this) || haveFullRight(this);
        }

        public bool IsPlanningVerificationRequired()
        {
            return IsPlanningVerificationRequiredExpr().Compile()(this);
        }

        public bool HasApprovalPermission(Guid? userId, IEnumerable<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return HasApprovalPermissionExpr(userId, userRoles).Compile()(this) || haveFullRight(this);
        }

        public bool HasApproveRejectCourseContentPermission(Guid? userId, IEnumerable<string> userRoles, Func<CourseEntity, bool> haveFullRight, Func<string, bool> checkHasPermissionFn)
        {
            return HasApprovalPermission(userId, userRoles, haveFullRight) &&
                   checkHasPermissionFn(LearningManagementPermissionKeys.ApproveRejectCourseContent);
        }

        public bool HasSubmitCourseForApprovalPermission(Guid? userId, List<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return HasContentCreatorPermission(userId, userRoles, haveFullRight) ||
                   HasApprovalPermission(userId, userRoles, haveFullRight);
        }

        public bool HasAdministrationPermission(Guid? userId, IEnumerable<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return HasAdministrationPermissionExpr(userId, userRoles).Compile()(this) || haveFullRight(this);
        }

        public bool HasFacilitatorPermission(Guid? userId, IEnumerable<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return HasFacilitatorPermissionExpr(userId, userRoles).Compile()(this) || haveFullRight(this);
        }

        public bool IsFacilitator(Guid? userId)
        {
            return IsFacilitatorExpr(userId).Compile()(this);
        }

        public bool HasContentCreatorPermission(Guid? userId, IEnumerable<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return HasContentCreatorPermissionExpr(userId, userRoles).Compile()(this) || haveFullRight(this);
        }

        public bool HasPublishContentPermission(
            Guid? userId,
            IEnumerable<string> userRoles,
            Func<CourseEntity, bool> haveFullRight,
            Func<string, bool> checkHasPermissionFn)
        {
            return HasContentCreatorPermission(userId, userRoles, haveFullRight) &&
                   checkHasPermissionFn(LearningManagementPermissionKeys.PublishCourseContent);
        }

        public bool HasPublishUnpublishPermission(
            Guid? userId,
            IEnumerable<string> userRoles,
            Func<CourseEntity, bool> haveFullRight,
            Func<string, bool> checkHasPermissionFn)
        {
            return HasPublishUnpublishPermissionExpr(userId, userRoles, checkHasPermissionFn).Compile()(this) || haveFullRight(this);
        }

        public bool HasUpdatePermission(Guid? userId, List<string> userRoles, Func<CourseEntity, bool> haveFullRight, Func<string, bool> checkHasPermissionFn)
        {
            return HasUpdatePermissionExpr(userId, userRoles, checkHasPermissionFn).Compile()(this) || haveFullRight(this);
        }

        public bool HasCreatePermission(Guid? userId, List<string> userRoles, Func<string, bool> checkHasPermissionFn)
        {
            return userId == null ||
                   UserRoles.IsAdministrator(userRoles) ||
                   userRoles.Any(r => r == UserRoles.CourseContentCreator) ||
                   checkHasPermissionFn(CourseAdminManagementPermissionKeys.CreateEditCourse);
        }

        public bool HasModifyCourseContentPermission(
            Guid? userId,
            List<string> userRoles,
            Func<CourseEntity, bool> haveFullRight,
            Func<string, bool> checkHasPermissionFn)
        {
            return userId == null ||
                   UserRoles.IsAdministrator(userRoles) ||
                   (checkHasPermissionFn(LearningManagementPermissionKeys.EditDeleteMineCourseContent) && CreatedBy == userId) ||
                   ((checkHasPermissionFn(LearningManagementPermissionKeys.CreateCourseContent) ||
                    checkHasPermissionFn(LearningManagementPermissionKeys.CreateAssignment) ||
                    checkHasPermissionFn(LearningManagementPermissionKeys.EditDeleteOthersCourseContent)) &&
                   (HasContentCreatorPermission(userId, userRoles, haveFullRight) || HasFacilitatorPermission(userId, userRoles, haveFullRight)));
        }

        public bool HasAddingParticipantsPermission(Guid? userId, IEnumerable<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return HasAddingParticipantsPermissionExpr(userId, userRoles).Compile()(this) || haveFullRight(this);
        }

        public bool HasSaveCourseCriteriaPermission(Guid? userId, List<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return HasContentCreatorPermission(userId, userRoles, haveFullRight) || HasAdministrationPermission(userId, userRoles, haveFullRight);
        }

        public bool HasSaveCourseAutomatePermission(Guid? userId, List<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return HasContentCreatorPermission(userId, userRoles, haveFullRight) || HasAdministrationPermission(userId, userRoles, haveFullRight);
        }

        public bool IsEditable(CourseType? courseType = null)
        {
            return IsEditableExpr(courseType).Compile()(this);
        }

        public Validation ValidateCanClone()
        {
            return CanCloneValidator().Validate(this);
        }

        public Validation ValidateCanEditContent()
        {
            return CanEditContentValidator().Validate(this);
        }

        public Validation ValidateCanPublish()
        {
            return CanPublishValidator().Validate(this);
        }

        public Validation ValidateCanApproveRejectContent()
        {
            return CanApproveRejectContentValidator().Validate(this);
        }

        public Validation ValidateCanPublishContent()
        {
            return CanPublishContentValidator().Validate(this);
        }

        public Validation ValidateCanUnpublishContent()
        {
            return CanUnpublishContentValidator().Validate(this);
        }

        public bool Started()
        {
            return StartedExpr().Compile()(this);
        }

        public bool HasVerifyPlanningCycleOrRejectVerificationPermission(Guid? userId, List<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return HasPlanningCycleVerificationPermissionExpr(userId, userRoles, haveFullRight)
                .Or(HasAfterVerificationEditPermissionExpr(userId, userRoles, haveFullRight))
                .Compile()(this);
        }

        public bool HasAfterVerificationEditPermission(Guid? userId, IEnumerable<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return HasAfterVerificationEditPermissionExpr(userId, userRoles, haveFullRight).Compile()(this);
        }

        public bool HasPlanningCycleVerificationPermission(Guid? userId, List<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return HasPlanningCycleVerificationPermissionExpr(userId, userRoles, haveFullRight).Compile()(this);
        }

        public bool HasImportParticipantPermission(Guid? userId, List<string> userRoles)
        {
            return HasAdministrationPermissionExpr(userId, userRoles).Compile()(this);
        }

        public bool OnlyForNominatedRegistration()
        {
            return OnlyForNominatedRegistrationExpr().Compile()(this);
        }

        public Validation ValidateCanImportParticipant()
        {
            return ImportParticipantValidator().Validate(this);
        }

        public bool CanByPassCAConfirmed()
        {
            return CanByPassCAConfirmedExpr().Compile()(this);
        }

        public bool CanByPassApproval()
        {
            return CanByPassApprovalExpr().Compile()(this);
        }

        public bool IsPendingPlanningCycleVerified()
        {
            return IsPendingCoursePlanningCycleVerifiedExpr().Compile()(this);
        }

        public CourseEntity CloneCourse(Guid courseId, Guid userId, int departmentId, bool fromCoursePlanning)
        {
            return new CourseEntity()
            {
                Id = courseId,
                Status = CourseStatus.Draft,
                CreatedDate = Clock.Now,
                CreatedBy = userId,
                CourseName = $"Copy of {CourseName}",
                CourseType = CourseType,
                ThumbnailUrl = ThumbnailUrl,
                DurationMinutes = DurationMinutes,
                DurationHours = DurationHours,
                PDActivityType = PDActivityType,
                LearningMode = PDActivityType == MetadataTagConstants.MicroLearningTagId ? MetadataTagConstants.ELearningTagId : LearningMode,
                CourseOutlineStructure = CourseOutlineStructure,
                CourseObjective = CourseObjective,
                Description = Description,
                CategoryIds = CategoryIds,
                OwnerDivisionIds = OwnerDivisionIds,
                OwnerBranchIds = OwnerBranchIds,
                PartnerOrganisationIds = PartnerOrganisationIds,
                MOEOfficerId = MOEOfficerId,
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
                ApplicableZoneIds = ApplicableZoneIds,
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
                PDAreaThemeId = PDAreaThemeId,
                CourseLevel = CourseLevel,
                ServiceSchemeIds = ServiceSchemeIds,
                SubjectAreaIds = SubjectAreaIds,
                LearningFrameworkIds = LearningFrameworkIds,
                LearningDimensionIds = LearningDimensionIds,
                LearningAreaIds = LearningAreaIds,
                LearningSubAreaIds = LearningSubAreaIds,
                TeacherOutcomeIds = TeacherOutcomeIds,
                NatureOfCourse = NatureOfCourse,
                NumOfPlannedClass = NumOfPlannedClass,
                NumOfSessionPerClass = NumOfSessionPerClass,
                NumOfHoursPerClass = NumOfHoursPerClass,
                NumOfHoursPerSession = NumOfHoursPerSession,
                NumOfMinutesPerSession = NumOfMinutesPerSession,
                MinParticipantPerClass = MinParticipantPerClass,
                MaxParticipantPerClass = MaxParticipantPerClass,
                PdActivityPeriods = PdActivityPeriods,
                MaxReLearningTimes = MaxReLearningTimes,
                DepartmentId = departmentId,
                PreCourseEvaluationFormId = PreCourseEvaluationFormId,
                PostCourseEvaluationFormId = PostCourseEvaluationFormId,
                ECertificateTemplateId = ECertificateTemplateId,
                ECertificatePrerequisite = ECertificatePrerequisite,
                FirstAdministratorId = FirstAdministratorId,
                SecondAdministratorId = SecondAdministratorId,
                PrimaryApprovingOfficerId = PrimaryApprovingOfficerId,
                AlternativeApprovingOfficerId = AlternativeApprovingOfficerId,
                CollaborativeContentCreatorIds = CollaborativeContentCreatorIds,
                CourseFacilitatorIds = CourseFacilitatorIds,
                CourseCoFacilitatorIds = CourseCoFacilitatorIds,
                Remarks = Remarks,
                CoursePlanningCycleId = fromCoursePlanning ? CoursePlanningCycleId : null,
                RegistrationMethod = RegistrationMethod,
                NieAcademicGroups = NieAcademicGroups,
                WillArchiveCommunity = WillArchiveCommunity
            };
        }

        public bool HasMarkScorePermission(Guid? userId, IEnumerable<string> userRoles, Func<CourseEntity, bool> haveFullRight, Func<string, bool> checkHasPermissionFn)
        {
            return HasMarkScorePermissionExpr(userId, userRoles, checkHasPermissionFn).Compile()(this) || haveFullRight(this);
        }

        public bool HasArchivalPermission(Guid? userId, IEnumerable<string> userRoles, Func<CourseEntity, bool> haveFullRight)
        {
            return HasArchivalPermissionExpr(userId, userRoles).Compile()(this) || haveFullRight(this);
        }

        public Validation<CourseEntity> ValidateCanArchiveCourse(bool isAnyInProgressLearner)
        {
            return CanArchiveCourseValidator(isAnyInProgressLearner).Validate(this);
        }

        public Validation<CourseEntity> ValidateCanBeApproval()
        {
            return CanBeApprovalValidator().Validate(this);
        }

        public List<Guid> GetAdministratorIds()
        {
            return new List<Guid>()
                .Concat(FirstAdministratorId.HasValue ? new List<Guid>() { FirstAdministratorId.Value } : new List<Guid>())
                .Concat(SecondAdministratorId.HasValue ? new List<Guid>() { SecondAdministratorId.Value } : new List<Guid>())
                .ToList();
        }

        public IEnumerable<Guid> GetAllInvoledUserIds()
        {
            return new List<Guid>()
                .Concat(GetAdministratorIds())
                .Concat(GetFacilitatorIds())
                .Concat(GetApprovingOfficerIds())
                .Concat(CollaborativeContentCreatorIds ?? new List<Guid>());
        }

        public IEnumerable<Guid> GetAssignmentManagementUserIds()
        {
            return new List<Guid>()
                .Concat(GetFacilitatorIds())
                .Concat(CollaborativeContentCreatorIds ?? new List<Guid>())
                .Concat(F.List(CreatedBy));
        }

        public IEnumerable<Guid> GetManagementInvoledUserIds()
        {
            return new List<Guid>()
                .Concat(GetAdministratorIds())
                .Concat(GetFacilitatorIds())
                .Concat(CollaborativeContentCreatorIds ?? new List<Guid>())
                .Concat(new List<Guid> { CreatedBy });
        }

        public List<Guid> GetAllCreatorIds()
        {
            return new List<Guid>()
                .Concat(CollaborativeContentCreatorIds ?? new List<Guid>())
                .Concat(new List<Guid> { CreatedBy })
                .ToList();
        }

        public IEnumerable<Guid> GetFacilitatorIds()
        {
            return new List<Guid>()
                .Concat(CourseFacilitatorIds ?? new List<Guid>())
                .Concat(CourseCoFacilitatorIds ?? new List<Guid>());
        }

        public List<Guid> GetApprovingOfficerIds()
        {
            return new List<Guid>()
                .Concat(PrimaryApprovingOfficerId.HasValue ? new List<Guid>() { PrimaryApprovingOfficerId.Value } : new List<Guid>())
                .Concat(AlternativeApprovingOfficerId.HasValue ? new List<Guid>() { AlternativeApprovingOfficerId.Value } : new List<Guid>())
                .ToList();
        }

        public Validation ValidateNotArchived()
        {
            return Validation.ValidIf(!IsArchived(), "Course must not be archived.");
        }

        public bool Equals([AllowNull] CourseEntity x, [AllowNull] CourseEntity y)
        {
            return (y == null && x == null) || (x != null && y != null && x.Id == y.Id);
        }

        public int GetHashCode([DisallowNull] CourseEntity obj)
        {
            return obj.Id.GetHashCode();
        }

        public bool NeedUpdateVersionNo(CourseStatus newStatus)
        {
            return Status != CourseStatus.Unpublished && newStatus == CourseStatus.Published;
        }

        public bool CanSubmitForApproval()
        {
            return CanSubmitForApprovalExpr().Compile()(this);
        }

        public List<int> GetOwnerDivisionBranchIds()
        {
            return OwnerDivisionIds.Concat(OwnerBranchIds).ToList();
        }

        public bool CanCompletePlanning()
        {
            return CanCompletePlanningExpr().Compile()(this);
        }

        public bool CanCompleteCourse()
        {
            return CanCompleteCourseExpr().Compile()(this);
        }

        public Validation ValidateCanUnpublishCourse()
        {
            return CanUnpublishCourseValidator().Validate(this);
        }

        public bool CanVerifyPlanningCycleOrRejectVerification()
        {
            return CanVerifyPlanningCycleOrRejectVerificationExpr().Compile()(this);
        }

        public bool HasViewContentPermission(Guid? userId, IEnumerable<string> userRoles)
        {
            return HasViewContentPermissionExpr(userId, userRoles).Compile()(this);
        }
    }
}
#pragma warning restore SA1124 // Do not use regions
