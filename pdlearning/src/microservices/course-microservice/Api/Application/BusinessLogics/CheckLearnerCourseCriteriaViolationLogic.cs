using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;
using Microservice.Course.Application.BusinessLogics.Abstractions;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.Enums.CourseCriteria;
using Microservice.Course.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.BusinessLogics
{
    public class CheckLearnerCourseCriteriaViolationLogic : BaseBusinessLogic
    {
        private readonly IReadOnlyRepository<CourseUser> _readUserRepository;
        private readonly IReadOnlyRepository<DepartmentUnitTypeDepartment> _readDepartmentUnitTypeDepartmentRepository;
        private readonly IReadOnlyRepository<DepartmentTypeDepartment> _readDepartmentTypeDepartmentRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;
        private readonly GetAggregatedDepartmentSharedQuery _getAggregatedDepartmentSharedQuery;

        public CheckLearnerCourseCriteriaViolationLogic(
            IReadOnlyRepository<DepartmentUnitTypeDepartment> readDepartmentUnitTypeDepartmentRepository,
            IReadOnlyRepository<DepartmentTypeDepartment> readDepartmentTypeDepartmentRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<CourseUser> readUserRepository,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery,
            GetAggregatedDepartmentSharedQuery getAggregatedDepartmentSharedQuery,
            IUserContext userContext) : base(userContext)
        {
            _readUserRepository = readUserRepository;
            _readDepartmentUnitTypeDepartmentRepository = readDepartmentUnitTypeDepartmentRepository;
            _readDepartmentTypeDepartmentRepository = readDepartmentTypeDepartmentRepository;
            _readRegistrationRepository = readRegistrationRepository;
            _readCourseRepository = readCourseRepository;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
            _getAggregatedDepartmentSharedQuery = getAggregatedDepartmentSharedQuery;
        }

        public async Task<CourseCriteriaLearnerViolation> Execute(
            Guid courseId,
            Guid classRunId,
            Guid learnerId,
            CourseCriteria courseCriteria,
            List<Registration> willBeAddedRegistrations,
            CancellationToken cancellationToken = default)
        {
            var courseCriteriaLearnerViolation = new CourseCriteriaLearnerViolation
            {
                CourseId = courseId,
                UpdatedDate = Clock.Now
            };

            var learnerInfo = await _readUserRepository.FirstOrDefaultAsync(learnerId);
            await SetLearnerCriteria(courseCriteriaLearnerViolation, learnerInfo);
            if (courseCriteria != null)
            {
                await SetAccountType(courseCriteriaLearnerViolation, courseCriteria, learnerInfo);
                await SetServiceSchemes(courseCriteriaLearnerViolation, courseCriteria, classRunId, learnerInfo, willBeAddedRegistrations, cancellationToken);
                await SetTracks(courseCriteriaLearnerViolation, courseCriteria, learnerInfo);
                await SetDevRoles(courseCriteriaLearnerViolation, courseCriteria, learnerInfo);
                await SetTeachingLevels(courseCriteriaLearnerViolation, courseCriteria, learnerInfo);
                await SetTeachingCourseOfStudy(courseCriteriaLearnerViolation, courseCriteria, learnerInfo);
                await SetJobFamily(courseCriteriaLearnerViolation, courseCriteria, learnerInfo);
                await SetCoCurricularActivity(courseCriteriaLearnerViolation, courseCriteria, learnerInfo);
                await SetSubGradeBanding(courseCriteriaLearnerViolation, courseCriteria, learnerInfo);
                await SetPlaceOfWork(courseCriteriaLearnerViolation, courseCriteria, classRunId, learnerInfo, willBeAddedRegistrations, cancellationToken);
                await SetPreRequisiteCourses(courseCriteriaLearnerViolation, courseCriteria, learnerInfo, cancellationToken);
            }

            courseCriteriaLearnerViolation.IsViolated = courseCriteriaLearnerViolation.CheckIsViolated();

            return courseCriteriaLearnerViolation;
        }

        private async Task SetPlaceOfWork(
            CourseCriteriaLearnerViolation courseCriteriaLearnerViolation,
            CourseCriteria courseCriteria,
            Guid classRunId,
            CourseUser learnerInfo,
            List<Registration> willBeAddedRegistrations,
            CancellationToken cancellationToken = default)
        {
            var inProgressRegistrationWithUser = await _getAggregatedRegistrationSharedQuery.WithUserByClassRun(
                courseCriteria.Id, classRunId, Registration.InProgressExpr(), willBeAddedRegistrations, cancellationToken);
            var inProgressRegistrationDepartments =
                await _getAggregatedDepartmentSharedQuery.ByDepartmentIds(
                    inProgressRegistrationWithUser.Select(p => p.User.DepartmentId).Distinct().ToList(), cancellationToken);

            if (courseCriteria.PlaceOfWork.Type == CourseCriteriaPlaceOfWorkType.DepartmentUnitTypes)
            {
                var userDepartmentUnitTypeId = _readDepartmentUnitTypeDepartmentRepository.GetAll().FirstOrDefault(p => p.DepartmentId == learnerInfo.DepartmentId)?.DepartmentUnitTypeId;
                courseCriteriaLearnerViolation.PlaceOfWork.DepartmentUnitTypes = courseCriteria.PlaceOfWork.DepartmentUnitTypes.Select(p => new CourseCriteriaLearnerViolationDepartmentUnitType
                {
                    DepartmentUnitTypeId = p.DepartmentUnitTypeId,
                    MaxParticipant = p.MaxParticipant,
                    ViolationType = p.DepartmentUnitTypeId != userDepartmentUnitTypeId
                        ? CourseCriteriaLearnerViolationType.Missing
                        : (p.MaxParticipant.HasValue && inProgressRegistrationDepartments.Count(x => x.DepartmentUnitType == p.DepartmentUnitTypeId) >= p.MaxParticipant
                            ? CourseCriteriaLearnerViolationType.OverMaxParticipant
                            : CourseCriteriaLearnerViolationType.NotViolate)
                });
                courseCriteriaLearnerViolation.PlaceOfWork.Type = CourseCriteriaPlaceOfWorkType.DepartmentUnitTypes;
            }

            if (courseCriteria.PlaceOfWork.Type == CourseCriteriaPlaceOfWorkType.DepartmentLevelTypes)
            {
                var userDepartmentLevelTypeId = _readDepartmentTypeDepartmentRepository.GetAll().FirstOrDefault(p => p.DepartmentId == learnerInfo.DepartmentId)?.DepartmentTypeId;
                courseCriteriaLearnerViolation.PlaceOfWork.DepartmentLevelTypes = courseCriteria.PlaceOfWork.DepartmentLevelTypes.Select(p => new CourseCriteriaLearnerViolationDepartmentLevelType
                {
                    DepartmentLevelTypeId = p.DepartmentLevelTypeId,
                    MaxParticipant = p.MaxParticipant,
                    ViolationType = p.DepartmentLevelTypeId != userDepartmentLevelTypeId
                        ? CourseCriteriaLearnerViolationType.Missing
                        : (p.MaxParticipant.HasValue && inProgressRegistrationDepartments.Count(x => x.DepartmentLevelType == p.DepartmentLevelTypeId) >= p.MaxParticipant
                            ? CourseCriteriaLearnerViolationType.OverMaxParticipant
                            : CourseCriteriaLearnerViolationType.NotViolate)
                });
                courseCriteriaLearnerViolation.PlaceOfWork.Type = CourseCriteriaPlaceOfWorkType.DepartmentLevelTypes;
            }

            if (courseCriteria.PlaceOfWork.Type == CourseCriteriaPlaceOfWorkType.SpecificDepartments)
            {
                var userDepartmentId = learnerInfo.DepartmentId;
                courseCriteriaLearnerViolation.PlaceOfWork.SpecificDepartments = courseCriteria.PlaceOfWork.SpecificDepartments.Select(p => new CourseCriteriaLearnerViolationSpecificDepartment
                {
                    DepartmentId = p.DepartmentId,
                    MaxParticipant = p.MaxParticipant,
                    ViolationType = p.DepartmentId != userDepartmentId
                        ? CourseCriteriaLearnerViolationType.Missing
                        : (p.MaxParticipant.HasValue && inProgressRegistrationDepartments.Count(x => x.Department.DepartmentId == p.DepartmentId) >= p.MaxParticipant
                            ? CourseCriteriaLearnerViolationType.OverMaxParticipant
                            : CourseCriteriaLearnerViolationType.NotViolate)
                });
                courseCriteriaLearnerViolation.PlaceOfWork.Type = CourseCriteriaPlaceOfWorkType.SpecificDepartments;
            }
        }

        private async Task SetPreRequisiteCourses(
            CourseCriteriaLearnerViolation courseCriteriaLearnerViolation,
            CourseCriteria courseCriteria,
            CourseUser learnerInfo,
            CancellationToken cancellationToken = default)
        {
            if (!courseCriteria.PreRequisiteCourses.Any())
            {
                courseCriteriaLearnerViolation.PreRequisiteCourses = new List<CourseCriteriaLearnerViolationPreRequisiteCourse>();
            }
            else
            {
                // TODO: We only use microlearning to fix bug temporarily because dont know how to check user have learning completed for microlearning
                var notMicrolearningPreRequisiteCourses = await _readCourseRepository.GetAll()
                    .Where(p => courseCriteria.PreRequisiteCourses.Contains(p.Id))
                    .Where(CourseEntity.IsMicroLearningExpr().Not())
                    .ToListAsync(cancellationToken);
                var userCompletedPreRequisiteCourses = await _readRegistrationRepository.GetAll()
                    .Where(p => courseCriteria.PreRequisiteCourses.Contains(p.CourseId) && p.UserId == learnerInfo.Id)
                    .Where(Registration.IsCompletedExpr())
                    .Select(p => p.CourseId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                var userCompletedPreRequisiteCoursesHashSet = userCompletedPreRequisiteCourses.ToHashSet();

                courseCriteriaLearnerViolation.PreRequisiteCourses = notMicrolearningPreRequisiteCourses.Select(p => new CourseCriteriaLearnerViolationPreRequisiteCourse
                {
                    CourseId = p.Id,
                    ViolationType = userCompletedPreRequisiteCoursesHashSet.Contains(p.Id)
                    ? CourseCriteriaLearnerViolationType.NotViolate
                    : CourseCriteriaLearnerViolationType.Missing
                });
            }
        }

        private Task SetTracks(CourseCriteriaLearnerViolation courseCriteriaLearnerViolation, CourseCriteria courseCriteria, CourseUser learnerInfo)
        {
            courseCriteriaLearnerViolation.Tracks = courseCriteria.Tracks.Select(id => new CourseCriteriaLearnerViolationTaggingMetadata
            {
                TagId = id,
                ViolationType = learnerInfo.HasMetadata(UserMetadataValueType.Track, id)
                ? CourseCriteriaLearnerViolationType.NotViolate
                : CourseCriteriaLearnerViolationType.Missing
            });
            return Task.CompletedTask;
        }

        private Task SetDevRoles(CourseCriteriaLearnerViolation courseCriteriaLearnerViolation, CourseCriteria courseCriteria, CourseUser learnerInfo)
        {
            courseCriteriaLearnerViolation.DevRoles = courseCriteria.DevRoles.Select(id => new CourseCriteriaLearnerViolationTaggingMetadata
            {
                TagId = id,
                ViolationType = learnerInfo.HasMetadata(UserMetadataValueType.DevelopmentalRole, id)
                ? CourseCriteriaLearnerViolationType.NotViolate
                : CourseCriteriaLearnerViolationType.Missing
            });
            return Task.CompletedTask;
        }

        private Task SetTeachingLevels(CourseCriteriaLearnerViolation courseCriteriaLearnerViolation, CourseCriteria courseCriteria, CourseUser learnerInfo)
        {
            courseCriteriaLearnerViolation.TeachingLevels = courseCriteria.TeachingLevels.Select(id => new CourseCriteriaLearnerViolationTaggingMetadata
            {
                TagId = id,
                ViolationType = learnerInfo.HasMetadata(UserMetadataValueType.TeachingLevel, id)
                ? CourseCriteriaLearnerViolationType.NotViolate
                : CourseCriteriaLearnerViolationType.Missing
            });
            return Task.CompletedTask;
        }

        private Task SetTeachingCourseOfStudy(CourseCriteriaLearnerViolation courseCriteriaLearnerViolation, CourseCriteria courseCriteria, CourseUser learnerInfo)
        {
            courseCriteriaLearnerViolation.TeachingCourseOfStudy = courseCriteria.TeachingCourseOfStudy.Select(id => new CourseCriteriaLearnerViolationTaggingMetadata
            {
                TagId = id,
                ViolationType = learnerInfo.HasMetadata(UserMetadataValueType.TeachingCourseOfStudy, id)
                ? CourseCriteriaLearnerViolationType.NotViolate
                : CourseCriteriaLearnerViolationType.Missing
            });
            return Task.CompletedTask;
        }

        private Task SetJobFamily(CourseCriteriaLearnerViolation courseCriteriaLearnerViolation, CourseCriteria courseCriteria, CourseUser learnerInfo)
        {
            courseCriteriaLearnerViolation.JobFamily = courseCriteria.JobFamily.Select(id => new CourseCriteriaLearnerViolationTaggingMetadata
            {
                TagId = id,
                ViolationType = learnerInfo.HasMetadata(UserMetadataValueType.JobFamily, id)
                ? CourseCriteriaLearnerViolationType.NotViolate
                : CourseCriteriaLearnerViolationType.Missing
            });
            return Task.CompletedTask;
        }

        private Task SetCoCurricularActivity(CourseCriteriaLearnerViolation courseCriteriaLearnerViolation, CourseCriteria courseCriteria, CourseUser learnerInfo)
        {
            courseCriteriaLearnerViolation.CoCurricularActivity = courseCriteria.CoCurricularActivity.Select(id => new CourseCriteriaLearnerViolationTaggingMetadata
            {
                TagId = id,
                ViolationType = learnerInfo.HasMetadata(UserMetadataValueType.CocurricularActivity, id)
                ? CourseCriteriaLearnerViolationType.NotViolate
                : CourseCriteriaLearnerViolationType.Missing
            });
            return Task.CompletedTask;
        }

        private Task SetSubGradeBanding(CourseCriteriaLearnerViolation courseCriteriaLearnerViolation, CourseCriteria courseCriteria, CourseUser learnerInfo)
        {
            courseCriteriaLearnerViolation.SubGradeBanding = courseCriteria.SubGradeBanding.Select(id => new CourseCriteriaLearnerViolationTaggingMetadata
            {
                TagId = id,
                ViolationType = learnerInfo.HasMetadata(UserMetadataValueType.EasSubstantiveGradeBanding, id)
                ? CourseCriteriaLearnerViolationType.NotViolate
                : CourseCriteriaLearnerViolationType.Missing
            });
            return Task.CompletedTask;
        }

        private async Task SetServiceSchemes(
            CourseCriteriaLearnerViolation courseCriteriaLearnerViolation,
            CourseCriteria courseCriteria,
            Guid classRunId,
            CourseUser learnerInfo,
            List<Registration> willBeAddedRegistrations,
            CancellationToken cancellationToken = default)
        {
            var inProgressRegistrationWithUser = await _getAggregatedRegistrationSharedQuery
                .WithUserByClassRun(courseCriteria.Id, classRunId, Registration.InProgressExpr(), willBeAddedRegistrations, cancellationToken);
            var inProgressRegistrationUsers = inProgressRegistrationWithUser.Select(p => p.User).ToList();
            var serviceSchemeToInProgressRegistrationCount = courseCriteria.CourseCriteriaServiceSchemes
                .ToDictionary(p => p.ServiceSchemeId, p => inProgressRegistrationUsers.Count(x => x.HasMetadata(UserMetadataValueType.ServiceScheme, p.ServiceSchemeId)));
            courseCriteriaLearnerViolation.ServiceSchemes = courseCriteria.CourseCriteriaServiceSchemes.Select(p => new CourseCriteriaLearnerViolationTaggingMetadata
            {
                TagId = p.ServiceSchemeId,
                MaxParticipant = p.MaxParticipant,
                ViolationType = learnerInfo.HasMetadata(UserMetadataValueType.ServiceScheme, p.ServiceSchemeId)
                ? (p.MaxParticipant.HasValue && serviceSchemeToInProgressRegistrationCount[p.ServiceSchemeId] >= p.MaxParticipant.Value
                    ? CourseCriteriaLearnerViolationType.OverMaxParticipant
                    : CourseCriteriaLearnerViolationType.NotViolate)
                : CourseCriteriaLearnerViolationType.Missing
            });
        }

        private Task SetAccountType(CourseCriteriaLearnerViolation courseCriteriaLearnerViolation, CourseCriteria courseCriteria, CourseUser learnerInfo)
        {
            courseCriteriaLearnerViolation.AccountType = new CourseCriteriaLearnerViolationAccountType
            {
                AccountType = courseCriteria.AccountType,
                ViolationType = CheckAccountTypeViolationType(courseCriteria, learnerInfo)
            };
            return Task.CompletedTask;
        }

        private CourseCriteriaLearnerViolationType CheckAccountTypeViolationType(CourseCriteria courseCriteria, CourseUser learnerInfo)
        {
            switch (courseCriteria.AccountType)
            {
                case CourseCriteriaAccountType.AllLearners:
                    return CourseCriteriaLearnerViolationType.NotViolate;
                case CourseCriteriaAccountType.MOELearners:
                    return learnerInfo.AccountType == CourseUserAccountType.Internal
                        ? CourseCriteriaLearnerViolationType.NotViolate
                        : CourseCriteriaLearnerViolationType.Missing;
                case CourseCriteriaAccountType.ExternalLearners:
                    return learnerInfo.AccountType == CourseUserAccountType.External
                        ? CourseCriteriaLearnerViolationType.NotViolate
                        : CourseCriteriaLearnerViolationType.Missing;
            }

            return CourseCriteriaLearnerViolationType.NotViolate;
        }

        private async Task SetLearnerCriteria(CourseCriteriaLearnerViolation courseCriteriaLearnerViolation, CourseUser learnerInfo)
        {
            var departmentUnitTypeDepartment = await _readDepartmentUnitTypeDepartmentRepository.FirstOrDefaultAsync(p => p.DepartmentId == learnerInfo.DepartmentId);
            var departmentTypeDepartment = await _readDepartmentTypeDepartmentRepository.FirstOrDefaultAsync(p => p.DepartmentId == learnerInfo.DepartmentId);
            courseCriteriaLearnerViolation.LearnerCriteria = new LearnerCourseCriteria
            {
                UserId = learnerInfo.Id,
                AccountType = learnerInfo.AccountType,
                ServiceSchemes = learnerInfo.ServiceScheme,
                Tracks = learnerInfo.Track,
                DevRoles = learnerInfo.DevelopmentalRole,
                TeachingLevels = learnerInfo.TeachingLevel,
                TeachingCourseOfStudy = learnerInfo.TeachingCourseOfStudy,
                JobFamily = learnerInfo.JobFamily,
                CoCurricularActivity = learnerInfo.CocurricularActivity,
                SubGradeBanding = learnerInfo.EasSubstantiveGradeBanding,
                Department = new LearnerCourseCriteriaDepartment
                {
                    DepartmentId = learnerInfo.DepartmentId,
                    DepartmentUnitTypeId = departmentUnitTypeDepartment?.DepartmentUnitTypeId ?? Guid.Empty,
                    DepartmentLevelTypeId = departmentTypeDepartment?.DepartmentTypeId ?? -1
                }
            };
        }
    }
}
