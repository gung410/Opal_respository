using System;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class SaveCourseCommandHandler : BaseCommandHandler<SaveCourseCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly CourseCudLogic _courseCudLogic;
        private readonly ProcessCoursePdAreaThemeCodeLogic _processCoursePdAreaThemeCodeLogic;

        public SaveCourseCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            CourseCudLogic courseCudLogic,
            ProcessCoursePdAreaThemeCodeLogic processCoursePdAreaThemeCodeLogic) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _courseCudLogic = courseCudLogic;
            _processCoursePdAreaThemeCodeLogic = processCoursePdAreaThemeCodeLogic;
        }

        protected override async Task HandleAsync(SaveCourseCommand command, CancellationToken cancellationToken)
        {
            if (command.IsCreate)
            {
                await CreateNew(command, cancellationToken);
            }
            else
            {
                await Update(command, cancellationToken);
            }
        }

        private async Task Update(SaveCourseCommand command, CancellationToken cancellationToken)
        {
            var course = await _readCourseRepository.GetAsync(command.Id);
            var courseHadAdminRightChecker = _readCourseRepository.GetHasAdminRightChecker(course, AccessControlContext);

            EnsureValidPermission(
                course.HasUpdatePermission(CurrentUserId, CurrentUserRoles, courseHadAdminRightChecker, p => HasPermissionPrefix(p)));

            EnsureBusinessLogicValid(course, p => p.IsEditable(command.CourseType));

            SetDataForCourseEntity(course, command);
            course.ChangedBy = CurrentUserIdOrDefault;

            await _processCoursePdAreaThemeCodeLogic.ForSingleCourse(course, cancellationToken);
            await _courseCudLogic.Update(course, cancellationToken);
        }

        private async Task CreateNew(SaveCourseCommand command, CancellationToken cancellationToken)
        {
            var course = new CourseEntity
            {
                Id = command.Id
            };

            EnsureValidPermission(
                course.HasCreatePermission(CurrentUserId, CurrentUserRoles, p => HasPermissionPrefix(p)));

            SetDataForCourseEntity(course, command);

            course.DepartmentId = AccessControlContext.GetUserDepartment();
            course.CreatedBy = CurrentUserId ?? Guid.Empty;
            await _processCoursePdAreaThemeCodeLogic.ForSingleCourse(course, cancellationToken);

            await _courseCudLogic.Insert(course, cancellationToken);
        }

        private void SetDataForCourseEntity(CourseEntity course, SaveCourseCommand command)
        {
            course.ThumbnailUrl = command.ThumbnailUrl;
            course.CourseName = command.CourseName;
            course.DurationMinutes = command.DurationMinutes;
            course.DurationHours = command.DurationHours;
            course.PDActivityType = command.PDActivityType;
            course.LearningMode = command.LearningMode;
            course.ExternalCode = command.ExternalCode;
            course.CourseOutlineStructure = command.CourseOutlineStructure;
            course.CourseObjective = command.CourseObjective;
            course.Description = command.Description;
            course.CategoryIds = command.CategoryIds;
            course.OwnerDivisionIds = command.OwnerDivisionIds;
            course.OwnerBranchIds = command.OwnerBranchIds;
            course.PartnerOrganisationIds = command.PartnerOrganisationIds;
            course.MOEOfficerId = command.MOEOfficerId;
            course.MOEOfficerPhoneNumber = command.MOEOfficerPhoneNumber;
            course.MOEOfficerEmail = command.MOEOfficerEmail;
            course.NotionalCost = command.NotionalCost;
            course.CourseFee = command.CourseFee;
            course.TrainingAgency = command.TrainingAgency;
            course.OtherTrainingAgencyReason = command.OtherTrainingAgencyReason;
            course.AllowPersonalDownload = command.AllowPersonalDownload;
            course.AllowNonCommerInMOEReuseWithModification = command.AllowNonCommerInMOEReuseWithModification;
            course.AllowNonCommerReuseWithModification = command.AllowNonCommerReuseWithModification;
            course.AllowNonCommerInMoeReuseWithoutModification = command.AllowNonCommerInMoeReuseWithoutModification;
            course.AllowNonCommerReuseWithoutModification = command.AllowNonCommerReuseWithoutModification;
            course.CopyrightOwner = command.CopyrightOwner;
            course.AcknowledgementAndCredit = command.AcknowledgementAndCredit;
            course.MaximumPlacesPerSchool = command.MaximumPlacesPerSchool;
            course.NumOfSchoolLeader = command.NumOfSchoolLeader;
            course.NumOfSeniorOrLeadTeacher = command.NumOfSeniorOrLeadTeacher;
            course.NumOfMiddleManagement = command.NumOfMiddleManagement;
            course.NumOfBeginningTeacher = command.NumOfBeginningTeacher;
            course.NumOfExperiencedTeacher = command.NumOfExperiencedTeacher;
            course.PlaceOfWork = command.PlaceOfWork;
            course.PrerequisiteCourseIds = command.PrerequisiteCourseIds;
            course.ApplicableDivisionIds = command.ApplicableDivisionIds;
            course.ApplicableBranchIds = command.ApplicableBranchIds;
            course.ApplicableZoneIds = command.ApplicableZoneIds;
            course.ApplicableClusterIds = command.ApplicableClusterIds;
            course.ApplicableSchoolIds = command.ApplicableSchoolIds;
            course.TrackIds = command.TrackIds;
            course.DevelopmentalRoleIds = command.DevelopmentalRoleIds;
            course.TeachingLevels = command.TeachingLevels;
            course.TeachingSubjectIds = command.TeachingSubjectIds;
            course.TeachingCourseStudyIds = command.TeachingCourseStudyIds;
            course.CocurricularActivityIds = command.CocurricularActivityIds;
            course.JobFamily = command.JobFamily;
            course.EasSubstantiveGradeBandingIds = command.EasSubstantiveGradeBandingIds;
            course.ApplicableZoneIds = command.ApplicableZoneIds;
            course.PDAreaThemeId = command.PDAreaThemeId;
            course.CourseLevel = command.CourseLevel;
            course.MetadataKeys = command.MetadataKeys;
            course.ServiceSchemeIds = command.ServiceSchemeIds;
            course.SubjectAreaIds = command.SubjectAreaIds;
            course.LearningFrameworkIds = command.LearningFrameworkIds;
            course.LearningDimensionIds = command.LearningDimensionIds;
            course.LearningAreaIds = command.LearningAreaIds;
            course.LearningSubAreaIds = command.LearningSubAreaIds;
            course.TeacherOutcomeIds = command.TeacherOutcomeIds;
            course.NatureOfCourse = command.NatureOfCourse;
            course.NumOfPlannedClass = command.NumOfPlannedClass;
            course.NumOfSessionPerClass = command.NumOfSessionPerClass;
            course.NumOfHoursPerSession = command.NumOfHoursPerSession;
            course.NumOfMinutesPerSession = command.NumOfMinutesPerSession;
            course.MinParticipantPerClass = command.MinParticipantPerClass;
            course.MaxParticipantPerClass = command.MaxParticipantPerClass;
            course.PlanningPublishDate = command.PlanningPublishDate;
            course.PlanningArchiveDate = command.PlanningArchiveDate;
            course.CourseType = command.CourseType;
            course.PdActivityPeriods = command.PdActivityPeriods;
            course.MaxReLearningTimes = command.MaxReLearningTimes;
            course.StartDate = command.StartDate;
            course.ExpiredDate = command.ExpiredDate;
            course.PreCourseEvaluationFormId = command.PreCourseEvaluationFormId;
            course.PostCourseEvaluationFormId = command.PostCourseEvaluationFormId;
            course.ECertificateTemplateId = command.ECertificateTemplateId;
            course.ECertificatePrerequisite = command.ECertificatePrerequisite;
            course.CourseNameInECertificate = command.CourseNameInECertificate;
            course.FirstAdministratorId = command.FirstAdministratorId;
            course.SecondAdministratorId = command.SecondAdministratorId;
            course.PrimaryApprovingOfficerId = command.PrimaryApprovingOfficerId;
            course.AlternativeApprovingOfficerId = command.AlternativeApprovingOfficerId;
            course.CollaborativeContentCreatorIds = command.CollaborativeContentCreatorIds;
            course.CourseFacilitatorIds = command.CourseFacilitatorIds;
            course.CourseCoFacilitatorIds = command.CourseCoFacilitatorIds;
            course.Remarks = command.Remarks;
            course.CoursePlanningCycleId = command.CoursePlanningCycleId;
            course.NieAcademicGroups = command.NieAcademicGroups;
            course.RegistrationMethod = command.PDActivityType == MetadataTagConstants.MicroLearningTagId ? RegistrationMethod.Public : command.RegistrationMethod;
            course.ArchiveDate = command.ArchiveDate;
            course.WillArchiveCommunity = command.WillArchiveCommunity;
        }
    }
}
