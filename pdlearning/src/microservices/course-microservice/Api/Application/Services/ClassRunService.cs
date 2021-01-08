using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.Commands;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries;
using Microservice.Course.Application.RequestDtos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Services
{
    public class ClassRunService : BaseApplicationService
    {
        private GetAggregatedClassRunSharedQuery _aggregatedClassRunSharedQuery;
        private IReadOnlyRepository<ClassRun> _readClassRunRepository;

        public ClassRunService(
            IThunderCqrs thunderCqrs,
            IUnitOfWorkManager unitOfWork,
            GetAggregatedClassRunSharedQuery aggregatedClassRunSharedQuery,
            IReadOnlyRepository<ClassRun> readClassRunRepository) : base(thunderCqrs, unitOfWork)
        {
            _aggregatedClassRunSharedQuery = aggregatedClassRunSharedQuery;
            _readClassRunRepository = readClassRunRepository;
        }

        public async Task<ClassRunModel> SaveClassRun(SaveClassRunRequest request)
        {
            var command = new SaveClassRunCommand(request);
            await ThunderCqrs.SendCommand(command);

            if (command.IsCreate)
            {
                await ThunderCqrs.SendCommand(new CloneContentForClassRunCommand
                {
                    ClassRunId = command.Id
                });
            }

            return await ThunderCqrs.SendQuery(new GetClassRunByIdQuery { Id = command.Id });
        }

        public Task<PagedResultDto<ClassRunModel>> GetClassRunsByCourseId(GetClassRunsByCourseIdRequest request)
        {
            return ThunderCqrs.SendQuery(new GetClassRunsByCourseIdQuery
            {
                CourseId = request.CourseId,
                SearchType = request.SearchType,
                LoadHasContentInfo = request.LoadHasContentInfo,
                PageInfo = new PagedResultRequestDto()
                {
                    SkipCount = request.SkipCount,
                    MaxResultCount = request.MaxResultCount
                },
                SearchText = request.SearchText,
                NotStarted = request.NotStarted,
                NotEnded = request.NotEnded,
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

        public Task<ClassRunModel> GetClassRunById(Guid id, bool loadHasLearnerStarted = false)
        {
            return ThunderCqrs.SendQuery(new GetClassRunByIdQuery { Id = id, LoadHasLearnerStarted = loadHasLearnerStarted });
        }

        public Task<List<ClassRunModel>> GetClassRunsByClassRunCodes(GetClassRunsByClassRunCodesRequest request)
        {
            return ThunderCqrs.SendQuery(new GetClassRunsByClassRunCodesQuery { ClassRunCodes = request.ClassRunCodes });
        }

        public Task<IEnumerable<ClassRunModel>> GetClassRunsByIds(IEnumerable<Guid> classRunIds)
        {
            return ThunderCqrs.SendQuery(new GetClassRunsByIdsQuery { ClassRunIds = classRunIds });
        }

        public async Task<bool> CheckClassIsFull(Guid classRunId)
        {
            var remainSlotDict = await ThunderCqrs.SendQuery(new GetClassRunRemainingSlotQuery
            {
                ClassRunIds = new List<Guid>
                {
                    classRunId
                }
            });

            return !remainSlotDict.TryGetValue(classRunId, out var remainSlot) || remainSlot <= 0;
        }

        public async Task<PagedResultDto<Guid>> MigrateClassRunNotification(MigrateClassrunNotificationRequest request)
        {
            return await ExecuteInUoW(async () =>
            {
                var query = _readClassRunRepository
                    .GetAll()
                    .WhereIf(request.ClassRunIds != null && request.ClassRunIds.Any(), p => request.ClassRunIds.Contains(p.Id));

                var totalCount = await query.CountAsync();
                if (request.SkipCount < 0 || request.MaxResultCount <= 0)
                {
                    return new PagedResultDto<Guid>(totalCount);
                }

                query = query.Skip(request.SkipCount).Take(request.MaxResultCount);
                var items = await _aggregatedClassRunSharedQuery.ByQuery(query, true);
                await ThunderCqrs.SendEvents(items.SelectList(p => new ClassRunChangeEvent(p.ToAssociatedEntity(), ClassRunChangeType.Updated, true)));
                return new PagedResultDto<Guid>(totalCount, items.Select(p => p.ClassRun.Id).ToList());
            });
        }

        public Task ChangeStatus(ChangeClassRunStatusRequest request)
        {
            return ThunderCqrs.SendCommand(new ChangeClassRunStatusCommand
            {
                Status = request.Status,
                Ids = request.Ids
            });
        }

        public async Task ChangeCancellationStatus(ChangeClassRunCancellationStatusRequest request)
        {
            await ThunderCqrs.SendCommand(new ChangeClassRunCancellationStatusCommand
            {
                CancellationStatus = request.CancellationStatus,
                Ids = request.Ids
            });

            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                await ThunderCqrs.SendCommand(new SaveCommentCommand
                {
                    EntityCommentType = EntityCommentType.ClassRun,
                    StatusEnum = request.CancellationStatus,
                    Content = request.Comment,
                    ObjectIds = request.Ids,
                    IsCreate = true
                });
            }
        }

        public async Task ChangeRescheduleStatus(ChangeClassRunRescheduleStatusRequest request)
        {
            await ThunderCqrs.SendCommand(new ChangeClassRunRescheduleStatusCommand
            {
                RescheduleStatus = request.RescheduleStatus,
                RescheduleSessions = request.RescheduleSessions?.SelectList(p => p.ToChangeClassRunRescheduleStatusCommandSession()),
                StartDateTime = request.StartDateTime,
                EndDateTime = request.EndDateTime,
                Ids = request.Ids
            });

            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                await ThunderCqrs.SendCommand(new SaveCommentCommand
                {
                    EntityCommentType = EntityCommentType.ClassRun,
                    StatusEnum = request.RescheduleStatus,
                    Content = request.Comment,
                    ObjectIds = request.Ids,
                    IsCreate = true
                });
            }
        }

        public Task ToggleCourseCriteria(ToggleCourseCriteriaRequest request)
        {
            return ThunderCqrs.SendCommand(new ToggleCourseCriteriaForClassRunCommand
            {
                Id = request.ClassRunId,
                CourseCriteriaActivated = request.CourseCriteriaActivated
            });
        }

        public Task ToggleCourseAutomate(ToggleCourseAutomateRequest request)
        {
            return ThunderCqrs.SendCommand(new ToggleCourseAutomateForClassRunCommand
            {
                Id = request.ClassRunId,
                CourseAutomateActivated = request.CourseAutomateActivated
            });
        }

        public Task<IEnumerable<TotalParticipantInClassRunModel>> GetTotalParticipantInClassRun(GetTotalParticipantInClassRunRequest request)
        {
            return ThunderCqrs.SendQuery(new GetTotalParticipantInClassRunQuery { ClassRunIds = request.ClassRunIds });
        }
    }
}
