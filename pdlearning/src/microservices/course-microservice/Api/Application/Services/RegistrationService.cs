using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.AssociatedEntities;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.RequestDtos.RegistrationRequest.ImportParticipant;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Common.Extensions;
using Microservice.Course.Common.Helpers;
using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Infrastructure;
using Microservice.Course.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Services
{
    public class RegistrationService : BaseApplicationService
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly SendLearningRecordEventLogic _sendLearningRecordEventLogic;
        private readonly ProcessJustLearningCompletedParticipantLogic _processJustLearningCompletedParticipantLogic;
        private readonly GetAggregatedClassRunSharedQuery _getAggregatedClassRunSharedQuery;
        private readonly GetAggregatedRegistrationSharedQuery _getAggregatedRegistrationSharedQuery;
        private readonly GetCanApplyCourseUsersSharedQuery _getCanApplyCourseUsersSharedQuery;
        private readonly GetRemainingClassRunSlotSharedQuery _getRemainingClassRunSlotSharedQuery;
        private readonly GetClassRunsByClassRunCodesSharedQuery _getClassRunsByClassRunCodesSharedQuery;
        private readonly GetUsersSharedQuery _getUsersSharedQuery;

        public RegistrationService(
            IThunderCqrs thunderCqrs,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IUnitOfWorkManager unitOfWorkManager,
            GetAggregatedClassRunSharedQuery getAggregatedClassRunSharedQuery,
            GetCanApplyCourseUsersSharedQuery getCanApplyCourseUsersSharedQuery,
            GetRemainingClassRunSlotSharedQuery getRemainingClassRunSlotSharedQuery,
            GetClassRunsByClassRunCodesSharedQuery getClassRunsByClassRunCodesSharedQuery,
            GetUsersSharedQuery getUsersSharedQuery,
            ProcessJustLearningCompletedParticipantLogic processJustLearningCompletedParticipantLogic,
            SendLearningRecordEventLogic sendLearningRecordEventLogic,
            GetAggregatedRegistrationSharedQuery getAggregatedRegistrationSharedQuery)
            : base(thunderCqrs, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _getAggregatedClassRunSharedQuery = getAggregatedClassRunSharedQuery;
            _getCanApplyCourseUsersSharedQuery = getCanApplyCourseUsersSharedQuery;
            _getRemainingClassRunSlotSharedQuery = getRemainingClassRunSlotSharedQuery;
            _getClassRunsByClassRunCodesSharedQuery = getClassRunsByClassRunCodesSharedQuery;
            _getUsersSharedQuery = getUsersSharedQuery;
            _processJustLearningCompletedParticipantLogic = processJustLearningCompletedParticipantLogic;
            _sendLearningRecordEventLogic = sendLearningRecordEventLogic;
            _getAggregatedRegistrationSharedQuery = getAggregatedRegistrationSharedQuery;
            _readCourseRepository = readCourseRepository;
        }

        public Task<PagedResultDto<RegistrationModel>> SearchRegistration(SearchRegistrationRequest request)
        {
            return ThunderCqrs.SendQuery(new SearchRegistrationQuery
            {
                CourseId = request.CourseId,
                ClassRunIds = request.ClassRunId.HasValue ? F.List(request.ClassRunId.Value) : null,
                ExcludeAssignedAssignmentId = request.ExcludeAssignedAssignmentId,
                SearchType = request.SearchType,
                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                },
                SearchText = request.SearchText,
                ApplySearchTextForCourse = request.ApplySearchTextForCourse,
                UserFilter = request.UserFilter != null
                    ? new CommonFilter
                    {
                        ContainFilters = request.UserFilter.ContainFilters?
                            .Select(p => new ContainFilter { Field = p.Field, Values = p.Values, NotContain = p.NotContain })
                            .ToList(),
                        FromToFilters = request.UserFilter.FromToFilters?
                            .Select(p => new FromToFilter
                            {
                                Field = p.Field,
                                FromValue = p.FromValue,
                                ToValue = p.ToValue,
                                EqualFrom = p.EqualFrom,
                                EqualTo = p.EqualTo
                            })
                            .ToList()
                    }
                    : null,
                Filter = request.Filter != null
                    ? new CommonFilter
                    {
                        ContainFilters = request.Filter.ContainFilters?
                            .Select(p => new ContainFilter { Field = p.Field, Values = p.Values, NotContain = p.NotContain })
                            .ToList(),
                        FromToFilters = request.Filter.FromToFilters?
                            .Select(p => new FromToFilter
                            {
                                Field = p.Field,
                                FromValue = p.FromValue,
                                ToValue = p.ToValue,
                                EqualFrom = p.EqualFrom,
                                EqualTo = p.EqualTo
                            })
                            .ToList()
                    }
                    : null
            });
        }

        public async Task ChangeRegistrationStatus(ChangeRegistrationStatusRequest request)
        {
            await ThunderCqrs.SendCommand(new ChangeRegistrationStatusCommand
            {
                Status = request.Status,
                Ids = request.Ids,
                Comment = request.Comment
            });

            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                await ThunderCqrs.SendCommand(new SaveCommentCommand
                {
                    EntityCommentType = EntityCommentType.Registration,
                    StatusEnum = request.Status,
                    Content = request.Comment,
                    ObjectIds = request.Ids,
                    IsCreate = true
                });
            }
        }

        public async Task OverrideRegistrationCourseCriteria(OverrideRegistrationCourseCriteriaRequest request)
        {
            await ThunderCqrs.SendCommand(new ChangeRegistrationCourseCriteriaOverridedStatusCommand
            {
                Ids = request.RegistrationIds,
                CourseCriteriaOverrided = true
            });

            var remainSlotDict = await ThunderCqrs.SendQuery(new GetClassRunRemainingSlotQuery
            {
                ClassRunIds = new List<Guid>
                {
                    request.ClassrunId
                }
            });

            if (remainSlotDict[request.ClassrunId] > 0)
            {
                await ThunderCqrs.SendCommand(new ChangeRegistrationStatusCommand
                {
                    Ids = request.RegistrationIds,
                    Status = RegistrationStatus.ConfirmedByCA
                });
            }
        }

        public Task<RegistrationModel> GetRegistrationById(Guid id)
        {
            return ThunderCqrs.SendQuery(new GetRegistrationByIdQuery { Id = id });
        }

        public Task<List<RegistrationModel>> GetRegistrationByIds(List<Guid> registrationIds)
        {
            return ThunderCqrs.SendQuery(new GetRegistrationByIdsQuery { Ids = registrationIds });
        }

        public async Task ChangeRegistrationWithdrawStatus(ChangeRegistrationWithdrawStatusRequest request)
        {
            await ThunderCqrs.SendCommand(new ChangeRegistrationWithdrawStatusCommand
            {
                WithdrawalStatus = request.WithdrawalStatus,
                Ids = request.Ids,
                IsManual = request.IsManual
            });

            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                await ThunderCqrs.SendCommand(new SaveCommentCommand
                {
                    EntityCommentType = EntityCommentType.Registration,
                    StatusEnum = request.WithdrawalStatus,
                    Content = request.Comment,
                    ObjectIds = request.Ids,
                    IsCreate = true
                });
            }
        }

        public async Task ChangeRegistrationClassRunChangeStatus(ChangeRegistrationClassRunChangeStatusRequest request)
        {
            await ThunderCqrs.SendCommand(new ChangeRegistrationClassRunChangeStatusCommand
            {
                ClassRunChangeStatus = request.ClassRunChangeStatus,
                Ids = request.Ids
            });

            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                await ThunderCqrs.SendCommand(new SaveCommentCommand
                {
                    EntityCommentType = EntityCommentType.Registration,
                    StatusEnum = request.ClassRunChangeStatus,
                    Content = request.Comment,
                    ObjectIds = request.Ids,
                    IsCreate = true
                });
            }
        }

        public async Task<ExportParticipantsResultModel> ExportParticipants(ExportParticipantsRequest request)
        {
            var course = await ThunderCqrs.SendQuery(new GetCourseByIdQuery() { Id = request.CourseId });
            var exportParticipantsResult = await ThunderCqrs.SendQuery(new SearchRegistrationQuery()
            {
                CourseId = request.CourseId,
                ClassRunIds = request.ClassRunIds,
                SearchType = SearchRegistrationType.Participant,
                IncludeCourseClassRun = true,
                IncludeUserInfo = true
            });

            var colNameToCellValueFnList = F.List(
                new KeyValuePair<string, Func<RegistrationModel, string>>("Course", p => p.Course?.CourseName ?? "n/a"),
                new KeyValuePair<string, Func<RegistrationModel, string>>("Course Code", p => p.Course?.CourseCode ?? "n/a"),
                new KeyValuePair<string, Func<RegistrationModel, string>>("Course Run", p => p.ClassRun?.ClassTitle ?? "n/a"),
                new KeyValuePair<string, Func<RegistrationModel, string>>("Course Run Code", p => p.ClassRun?.ClassRunCode ?? "n/a"),
                new KeyValuePair<string, Func<RegistrationModel, string>>("Course Run Status", p => p.ClassRun?.Status.ToString() ?? "n/a"),
                new KeyValuePair<string, Func<RegistrationModel, string>>("Participant", p => p.User?.FullName ?? "n/a"),
                new KeyValuePair<string, Func<RegistrationModel, string>>("Participant's Email", p => p.User?.EmailAddress ?? "n/a"));

            if (request.FileFormat == ExportParticipantsFileFormat.Csv)
            {
                return new ExportParticipantsResultModel
                {
                    Course = course,
                    FileContent = CsvHelper.ExportData(exportParticipantsResult.Items.ToList(), colNameToCellValueFnList),
                    FileFormat = request.FileFormat
                };
            }

            return new ExportParticipantsResultModel
            {
                Course = course,
                FileContent = ExcelHelper.ExportData(exportParticipantsResult.Items.ToList(), colNameToCellValueFnList),
                FileFormat = request.FileFormat
            };
        }

        public ExportParticipantTemplateResultModel ExportParticipantTemplate(ExportParticipantTemplateRequest request)
        {
            var exportParticipantTemplateResult = new List<ImportPartcipantDto>
            {
                new ImportPartcipantDto { ClassRunCode = "AM-000139-01", LearnerEmail = "learner@yopmail.com" }
            };

            return new ExportParticipantTemplateResultModel
            {
                FileContent = request.FileFormat == ExportParticipantTemplateFileFormat.Csv
                    ? CsvHelper.ExportData(exportParticipantTemplateResult)
                    : ExcelHelper.ExportData(exportParticipantTemplateResult),
                FileFormat = request.FileFormat
            };
        }

        public Task ChangeRegistrationStatusByLearner(ChangeRegistrationByLearnerRequest request)
        {
            return ThunderCqrs.SendCommand(new ChangeRegistrationByLearnerCommand
            {
                CourseId = request.CourseId,
                ClassRunId = request.ClassRunId,
                Status = request.Status
            });
        }

        public async Task<List<RegistrationModel>> SaveRegistration(CreateRegistrationRequest request)
        {
            var command = request.ToCommand();
            await ThunderCqrs.SendCommand(command);

            var registrations = await ThunderCqrs.SendQuery(new GetRegistrationByIdsQuery { Ids = command.Registrations.Select(p => p.Id).ToList() });
            return registrations;
        }

        public Task NominateUser(UserNominationRequest request)
        {
            return ThunderCqrs.SendCommand(new NominateUserCommand
            {
                Registrations = request.NominatedRegistrations
                    .Select(p => new NominateUserCommandRegistration
                    {
                        AlternativeApprovingOfficer = p.AlternativeApprovingOfficer,
                        ApprovingOfficer = p.ApprovingOfficer,
                        ClassRunId = request.ClassRunId,
                        CourseId = request.CourseId,
                        UserId = p.UserId
                    })
            });
        }

        public Task MassNominateUsers(MassUserNominationRequest request)
        {
            return ThunderCqrs.SendCommand(new NominateUserCommand
            {
                Registrations = request.NominatedLearners
                    .Select(p => new NominateUserCommandRegistration
                    {
                        AlternativeApprovingOfficer = p.AlternativeAOId,
                        ApprovingOfficer = p.PrimaryAOId,
                        ClassRunId = p.ClassRunId,
                        CourseId = p.CourseId,
                        UserId = p.LearnerId
                    })
            });
        }

        public Task<List<ValidateNominateLearnerResultModel>> ValidateNominatedLearners(ValidateNominateLearnersRequest request)
        {
            return ThunderCqrs.SendQuery(new ValidateNominateLearnersQuery
            {
                Registrations = request.UserId
                    .Select(x => new ValidateNominatedLearnersQueryRegistration
                    {
                        CourseId = request.CourseId,
                        UserId = x
                    })
                    .ToList()
            });
        }

        public Task<List<ValidateNominateLearnerResultModel>> ValidateMassNominatedLearners(
            ValidateMassNominateLearnersRequest request)
        {
            return ThunderCqrs.SendQuery(new ValidateNominateLearnersQuery
            {
                Registrations = request.ValidateNominatedLearners
                    .Select(x => new ValidateNominatedLearnersQueryRegistration
                    {
                        CourseId = x.CourseId,
                        UserId = x.LearnerId
                    })
                    .ToList()
            });
        }

        public Task<PagedResultDto<RegistrationModel>> GetApprovalRegistration(GetApprovalRegistrationRequest request)
        {
            return ThunderCqrs.SendQuery(new GetApprovalRegistrationQuery
            {
                FilterType = request.FilterType,

                CourseId = request.CourseId,
                ClassRunIds = request.ClassRunIds,
                LearnerIds = request.LearnerIds,
                WithdrawalStatuses = request.WithdrawalStatuses,
                RegistrationStatuses = request.RegistrationStatuses,
                ClassRunChangeStatuses = request.ClassRunChangeStatuses,

                RegistrationStartDate = request.RegistrationStartDate,
                RegistrationEndDate = request.RegistrationEndDate,
                WithdrawalStartDate = request.WithdrawalStartDate,
                WithdrawalEndDate = request.WithdrawalEndDate,
                ClassRunChangeRequestedStartDate = request.ClassRunChangeRequestedStartDate,
                ClassRunChangeRequestedEndDate = request.ClassRunChangeRequestedEndDate,

                PageInfo = new PagedResultRequestDto
                {
                    MaxResultCount = request.MaxResultCount,
                    SkipCount = request.SkipCount
                }
            });
        }

        public async Task<PagedResultDto<Guid>> MigrateRegistrations(MigrateRegistrationRequest request)
        {
            return await ExecuteInUoW(async () =>
            {
                var query = _readRegistrationRepository
                    .GetAll()
                    .WhereIf(request.ClassRunIds != null && request.ClassRunIds.Any(), p => request.ClassRunIds.Contains(p.ClassRunId));

                var totalCount = await query.CountAsync();

                if (request.SkipCount < 0 || request.MaxResultCount <= 0)
                {
                    return new PagedResultDto<Guid>(totalCount);
                }

                var registrationsQuery = query.Skip(request.SkipCount).Take(request.MaxResultCount);
                var aggregatedRegistrations = await _getAggregatedRegistrationSharedQuery.FullByQuery(registrationsQuery);
                await ThunderCqrs.SendEvents(aggregatedRegistrations.SelectList(
                    p => new RegistrationChangeEvent(new RegistrationAssociatedEntity(p.Registration, p.Course, p.ClassRun), RegistrationChangeType.Updated, true)));
                return new PagedResultDto<Guid>(totalCount, aggregatedRegistrations.Select(p => p.Registration.Id).ToList());
            });
        }

        public async Task<PagedResultDto<Guid>> MigrateLearningRecords(MigrateLearningRecordRequest request)
        {
            return await ExecuteInUoW(async () =>
            {
                var query = _readRegistrationRepository
                    .GetAll()
                    .WhereIf(request.UserIds != null && request.UserIds.Any(), p => request.UserIds.Contains(p.UserId));

                var totalCount = await query.CountAsync();

                if (request.SkipCount < 0 || request.MaxResultCount <= 0)
                {
                    return new PagedResultDto<Guid>(totalCount);
                }

                var results = await query.Skip(request.SkipCount).Take(request.MaxResultCount).ToListAsync();

                await _sendLearningRecordEventLogic.ByRegistrations(results);

                return new PagedResultDto<Guid>(totalCount, results.Select(p => p.Id).ToList());
            });
        }

        /// <summary>
        /// Support migrate for old completed registrations which don't have certificate to generate certificates.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>PagedResult of registration ids.</returns>
        public async Task<PagedResultDto<Guid>> MigrateRegistrationECertificates(MigrateRegistrationECertificateRequest request)
        {
            return await ExecuteInUoW(async () =>
            {
                var missedECertificateCompletedParticipantQuery = _readRegistrationRepository
                    .GetAll()
                    .Where(Registration.IsCompletedExpr())
                    .Where(p => p.ECertificate == null)
                    .WhereIf(request.ClassRunIds != null && request.ClassRunIds.Any(), p => request.ClassRunIds.Contains(p.ClassRunId))
                    .Join(_readCourseRepository.GetAll().Where(p => p.ECertificateTemplateId != null), p => p.CourseId, p => p.Id, (registration, course) => registration);

                var totalCount = await missedECertificateCompletedParticipantQuery.CountAsync();

                if (request.SkipCount < 0 || request.MaxResultCount <= 0)
                {
                    return new PagedResultDto<Guid>(totalCount);
                }

                var missedECertificateCompletedParticipants = await missedECertificateCompletedParticipantQuery.Skip(request.SkipCount).Take(request.MaxResultCount).ToListAsync();

                await _sendLearningRecordEventLogic.ByRegistrations(missedECertificateCompletedParticipants);
                await _processJustLearningCompletedParticipantLogic.CreateECertificateForCompletedParticipant(missedECertificateCompletedParticipants);

                return new PagedResultDto<Guid>(totalCount, missedECertificateCompletedParticipants.Select(p => p.Id).ToList());
            });
        }

        public Task<double> GetCompletionRate(Guid classRunId)
        {
            return ThunderCqrs.SendQuery(new GetCompletionRateQuery
            {
                ClassRunId = classRunId
            });
        }

        public Task CompleteOrIncompleteRegistration(ChangeLearnerStatusRequest request)
        {
            return ThunderCqrs.SendCommand(new UpdateRegistrationLearningInfoCommand
            {
                RegistrationIds = request.RegistrationIds,
                ClassRunId = request.ClassRunId,
                LearningStatus = request.IsCompleted ? LearningStatus.Completed : LearningStatus.Failed
            });
        }

        public async Task ChangeClassRunRegistration(ChangeClassRunRequest request)
        {
            await ThunderCqrs.SendCommand(new ChangeClassRunCommand
            {
                RegistrationIds = new List<Guid>() { request.RegistrationId },
                ClassRunChangeId = request.ClassRunChangeId
            });

            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                await ThunderCqrs.SendCommand(new SaveCommentCommand
                {
                    EntityCommentType = EntityCommentType.Registration,
                    StatusEnum = ClassRunChangeStatus.PendingConfirmation,
                    Content = request.Comment,
                    ObjectIds = new List<Guid>() { request.RegistrationId },
                    IsCreate = true
                });
            }
        }

        public Task MassChangeClassRunRegistration(MassChangeClassRunRequest request)
        {
            return ThunderCqrs.SendCommand(new ChangeClassRunCommand
            {
                RegistrationIds = request.RegistrationIds,
                ClassRunChangeId = request.ClassRunChangeId
            });
        }

        public async Task<AddParticipantsModel> AddParticipants(AddParticipantsRequest request)
        {
            return await ExecuteInUoW(async () =>
            {
                var allCourseUsersQuery = _getUsersSharedQuery.ByIds(request.UserIds, request.DepartmentIds);
                var addParticipantsCommandRegistrationItems = await BuildAddParticipantsCommandRegistrationItems(request);

                await ThunderCqrs.SendCommand(new AddParticipantsCommand
                {
                    ClassRunId = request.ClassRunId,
                    Items = addParticipantsCommandRegistrationItems
                });

                return new AddParticipantsModel
                {
                    NumberOfAddedParticipants = addParticipantsCommandRegistrationItems.Count(p => p.Action == AddParticipantsCommandRegistrationAction.SaveAddedByCASuccessfully),
                    TotalNumberOfUsers = await allCourseUsersQuery.CountAsync()
                };
            });
        }

        public async Task<AddParticipantsModel> ImportParticipant(ImportPartcipantRequest request)
        {
            return await ExecuteInUoW(async () =>
            {
                var course = await _readCourseRepository.FirstOrDefaultAsync(request.CourseId);

                await using var fileStream = request.File.OpenReadStream();
                List<ImportPartcipantDto> importingRecords;
                switch (Path.GetExtension(request.File.FileName))
                {
                    case ".xlsx":
                    case ".xls":
                        {
                            importingRecords = ExcelHelper.ImportData<ImportPartcipantDto>(fileStream);
                            break;
                        }

                    case ".csv":
                        {
                            importingRecords = CsvHelper.ImportData<ImportPartcipantDto>(fileStream);
                            break;
                        }

                    default:
                        {
                            throw new GeneralException($"Not supported extension for file {request.File.FileName}.");
                        }
                }

                importingRecords = importingRecords.DistinctBy(x => x.LearnerEmail).ToList();
                var classRuns = await _getClassRunsByClassRunCodesSharedQuery.Execute(
                    importingRecords.Select(x => x.ClassRunCode).ToList(), request.CourseId, F.List(ClassRunStatus.Published));
                var userInfos = await _getUsersSharedQuery.ByEmails(importingRecords.Select(x => x.LearnerEmail).Distinct().ToList());
                var toImportRegistrationInfos = importingRecords
                    .Join(userInfos, x => x.LearnerEmail, q => q.Email, (r, u) => new { UserInfo = u, r.ClassRunCode })
                    .Join(classRuns, x => x.ClassRunCode, q => q.ClassRunCode, (x, q) => new { x.UserInfo, ClassRun = q, q.CourseId })
                    .ToList();

                var existedInprogressRegistrationByUserDic = (await _readRegistrationRepository
                        .GetAll()
                        .Where(x => x.CourseId == request.CourseId)
                        .Where(Registration.InProgressExpr())
                        .ToListAsync())
                    .GroupBy(x => x.UserId)
                    .ToDictionary(x => x.Key, x => x.AsEnumerable());
                var remainingSlotDict = await _getRemainingClassRunSlotSharedQuery.ByClassRunIds(classRuns.Select(x => x.Id).ToList());

                var importParticipantsCommand = new ImportParticipantsCommand(request.CourseId);
                foreach (var item in toImportRegistrationInfos)
                {
                    if (item.ClassRun.CourseId != course.Id)
                    {
                        throw new BusinessLogicException($"Classrun [{item.ClassRun.ClassRunCode}] is not for the course [{course.CourseName}]");
                    }

                    if (item.ClassRun.IsStarted)
                    {
                        if (course.LearningMode != MetadataTagConstants.ELearningTagId || item.ClassRun.IsEnded)
                        {
                            continue;
                        }
                    }

                    var existedInProgressRegistrations =
                        existedInprogressRegistrationByUserDic.GetValueOrDefault(item.UserInfo.Id)?.ToList() ?? new List<Registration>();

                    // Skip if learner is learning in this course
                    if (existedInProgressRegistrations.Any(x =>
                        (x.IsParticipant || x.RegistrationType == RegistrationType.AddedByCA) &&
                        !x.IsLearningFinished()))
                    {
                        continue;
                    }

                    var notLearningFinishedRegistrations = existedInProgressRegistrations.Where(x => !x.IsLearningFinished()).ToList();
                    var existedNotLearningFinishedSameClassRunInProgressRegistration = notLearningFinishedRegistrations.FirstOrDefault(x => x.ClassRunId == item.ClassRun.Id);

                    // If learner already has a registration in the same class waiting to be confirmed, that registration will be confirmed automatically.
                    if (existedNotLearningFinishedSameClassRunInProgressRegistration != null
                        && !existedNotLearningFinishedSameClassRunInProgressRegistration.IsParticipant
                        && (remainingSlotDict[item.ClassRun.Id] > 0 || existedNotLearningFinishedSameClassRunInProgressRegistration.IsSlotTaking()))
                    {
                        if (!existedNotLearningFinishedSameClassRunInProgressRegistration.IsSlotTaking())
                        {
                            remainingSlotDict[item.ClassRun.Id]--;
                        }

                        importParticipantsCommand.ToConfirmRegistrations.Add(existedNotLearningFinishedSameClassRunInProgressRegistration);
                        importParticipantsCommand.ToRejectRegistrations.AddRange(notLearningFinishedRegistrations.Where(x => x.Id != existedNotLearningFinishedSameClassRunInProgressRegistration.Id));
                        continue;
                    }

                    // New registration will be created to import if not exceed max relearning times for the course.
                    if (existedInProgressRegistrations.Count(x => x.IsParticipant && x.IsLearningFinished()) < course.MaxReLearningTimes)
                    {
                        importParticipantsCommand.ToCreateRegistrations.Add(new Registration
                        {
                            Status = RegistrationStatus.ConfirmedByCA,
                            RegistrationType = RegistrationType.AddedByCA,
                            CourseId = item.CourseId,
                            ClassRunId = item.ClassRun.Id,
                            UserId = item.UserInfo.Id,
                            ApprovingOfficer = item.UserInfo.PrimaryApprovingOfficerId,
                            AlternativeApprovingOfficer = item.UserInfo.AlternativeApprovingOfficerId,
                            AdministrationDate = Clock.Now,
                            RegistrationDate = Clock.Now
                        });
                        remainingSlotDict[item.ClassRun.Id]--;
                    }
                }

                await ThunderCqrs.SendCommand(importParticipantsCommand);

                return new AddParticipantsModel
                {
                    TotalNumberOfUsers = importingRecords.Count,
                    NumberOfAddedParticipants = importParticipantsCommand.NumberOfAddedParticipants()
                };
            });
        }

        public Task<GetLearnerCourseViolationQueryResult> GetLearnerCourseViolation(GetLearnerViolationRequest request)
        {
            return ThunderCqrs.SendQuery(new GetLearnerCourseViolationQuery
            {
                CourseId = request.CourseId,
                ClassRunId = request.ClassRunId
            });
        }

        public Task<NoOfFinishedRegistrationModel> GetNoOfFinishedRegistration(GetNoOfFinishedRegistrationRequest request)
        {
            return ThunderCqrs.SendQuery(new GetNoOfFinishedRegistrationQuery
            {
                CourseId = request.CourseId,
                DepartmentId = request.DepartmentId,
                ForClassRunEndAfter = request.ForClassRunEndAfter,
                ForClassRunEndBefore = request.ForClassRunEndBefore
            });
        }

        public Task CompletePostEvaluation(Guid registrationId)
        {
            return ThunderCqrs.SendCommand(new CompletePostEvaluationCommand
            {
                RegistrationId = registrationId
            });
        }

        public Task<RegistrationECertificateModel> GetECertificateByRegistrationId(Guid registrationId)
        {
            return ThunderCqrs.SendQuery(new GetRegistrationCertificateByIdQuery
            {
                RegistrationId = registrationId
            });
        }

        public Task<PagedResultDto<RegistrationModel>> GetMyCertificates(GetMyCertificatesRequest request)
        {
            return ThunderCqrs.SendQuery(request.ToQuery());
        }

        private async Task<List<AddParticipantsCommandRegistrationItem>> BuildAddParticipantsCommandRegistrationItems(
            AddParticipantsRequest request)
        {
            var aggregatedClassRun = await _getAggregatedClassRunSharedQuery.ByClassRunId(request.ClassRunId);
            var allCourseUsersQuery = _getUsersSharedQuery.ByIds(request.UserIds, request.DepartmentIds);

            var validUsersQuery = _getCanApplyCourseUsersSharedQuery
                .FromQuery(allCourseUsersQuery, aggregatedClassRun.Course, request.FollowCourseTargetParticipant);

            var validUserIds = await validUsersQuery.Select(p => p.Id).ToListAsync();
            var inprogressUserIdRegistrationDict = (await _readRegistrationRepository
                .GetAll()
                .Join(validUsersQuery, p => p.UserId, p => p.Id, (registration, user) => registration)
                .Where(x => x.CourseId == aggregatedClassRun.Course.Id)
                .Where(Registration.InProgressExpr().AndAlso(Registration.IsNotAbleToBeNominatedExpr()))
                .ToListAsync())
                .GroupBy(p => p.UserId)
                .Select(a => new { a.Key, List = a.ToList() })
                .ToDictionary(p => p.Key, p => p.List);
            var remainingSlot = await _getRemainingClassRunSlotSharedQuery.ByClassRunId(request.ClassRunId);

            var registrationItemsToAdd = validUserIds.Select(x =>
            {
                if (remainingSlot <= 0)
                {
                    return new AddParticipantsCommandRegistrationItem
                    {
                        Action = AddParticipantsCommandRegistrationAction.SaveAddedByCAClassfull,
                        UserId = x,
                        RegistrationId = inprogressUserIdRegistrationDict.ContainsKey(x)
                            ? inprogressUserIdRegistrationDict[x].FirstOrDefault(p =>
                                p.RegistrationType == RegistrationType.AddedByCA
                                && p.ClassRunId == aggregatedClassRun.ClassRun.Id
                                && (p.Status == RegistrationStatus.AddedByCAClassfull ||
                                    p.Status == RegistrationStatus.AddedByCAConflict))?.Id
                            : null
                    };
                }

                if (inprogressUserIdRegistrationDict.ContainsKey(x) &&
                    inprogressUserIdRegistrationDict[x].Any(p => p.ClassRunId != aggregatedClassRun.ClassRun.Id && p.Status != RegistrationStatus.AddedByCAClassfull && p.Status != RegistrationStatus.AddedByCAConflict))
                {
                    return new AddParticipantsCommandRegistrationItem
                    {
                        Action = AddParticipantsCommandRegistrationAction.SaveAddedByCAConflict,
                        UserId = x,
                        RegistrationId = inprogressUserIdRegistrationDict.ContainsKey(x)
                            ? inprogressUserIdRegistrationDict[x].FirstOrDefault(p =>
                                p.RegistrationType == RegistrationType.AddedByCA
                                && p.ClassRunId == aggregatedClassRun.ClassRun.Id
                                && (p.Status == RegistrationStatus.AddedByCAClassfull ||
                                    p.Status == RegistrationStatus.AddedByCAConflict))?.Id
                            : null
                    };
                }

                remainingSlot--;

                return new AddParticipantsCommandRegistrationItem
                {
                    Action = AddParticipantsCommandRegistrationAction.SaveAddedByCASuccessfully,
                    UserId = x,
                    OtherInProgressRegistrationIds = inprogressUserIdRegistrationDict.ContainsKey(x)
                        ? inprogressUserIdRegistrationDict[x].Where(p => p.IsPending()).Select(p => p.Id).ToList()
                        : null,
                    RegistrationId = inprogressUserIdRegistrationDict.ContainsKey(x)
                        ? inprogressUserIdRegistrationDict[x].FirstOrDefault(p =>
                            p.RegistrationType == RegistrationType.AddedByCA
                            && p.ClassRunId == aggregatedClassRun.ClassRun.Id
                            && (p.Status == RegistrationStatus.AddedByCAClassfull ||
                                p.Status == RegistrationStatus.AddedByCAConflict))?.Id
                        : null
                };
            }).ToList();
            return registrationItemsToAdd;
        }
    }
}
