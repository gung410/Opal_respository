using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.AggregatedEntityModels;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Constants;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Exceptions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeClassRunStatusCommandHandler : BaseCommandHandler<ChangeClassRunStatusCommand>
    {
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly ClassRunCudLogic _classRunCudLogic;
        private readonly GetAggregatedClassRunSharedQuery _getAggregatedClassRunSharedQuery;

        public ChangeClassRunStatusCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<ClassRun> readClassRunRepository,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            IThunderCqrs thunderCqrs,
            IAccessControlContext<CourseUser> accessControlContext,
            ClassRunCudLogic classRunCudLogic,
            WebAppLinkBuilder webAppLinkBuilder,
            GetAggregatedClassRunSharedQuery getAggregatedClassRunSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readClassRunRepository = readClassRunRepository;
            _readCourseRepository = readCourseRepository;
            _thunderCqrs = thunderCqrs;
            _webAppLinkBuilder = webAppLinkBuilder;
            _getAggregatedClassRunSharedQuery = getAggregatedClassRunSharedQuery;
            _classRunCudLogic = classRunCudLogic;
        }

        protected override async Task HandleAsync(ChangeClassRunStatusCommand command, CancellationToken cancellationToken)
        {
            var aggregatedClassRuns = await _getAggregatedClassRunSharedQuery.ByClassRunIds(command.Ids, true, cancellationToken);
            var classRuns = aggregatedClassRuns.Select(p => p.ClassRun).ToList();

            await EnsureValidPermission(HasValidPermission(classRuns, cancellationToken));

            if (command.Status == ClassRunStatus.Published)
            {
                EnsureBusinessLogicValid(aggregatedClassRuns, p => p.ClassRun.ValidateCanPublish(p.Course));
            }

            aggregatedClassRuns.ForEach(aggregatedClassRun =>
            {
                var classRun = aggregatedClassRun.ClassRun;

                classRun.Status = command.Status;
                classRun.ChangedBy = CurrentUserId;
                classRun.ChangedDate = Clock.Now;
            });

            Dictionary<Guid, CourseEntity> courseDict = null;

            if (command.Status == ClassRunStatus.Published)
            {
                var courseIds = classRuns.Select(x => x.CourseId).ToList();
                var classRunNotPublished = classRuns.Where(x => string.IsNullOrEmpty(x.ClassRunCode)).ToList();
                courseDict = await BuildCourseDictionary(courseIds, cancellationToken);
                await GenerateClassRunCode(classRunNotPublished, courseIds, courseDict, cancellationToken);
            }

            await _classRunCudLogic.UpdateMany(aggregatedClassRuns, cancellationToken);
            await SendNotification(command, classRuns, courseDict, cancellationToken);
        }

        private async Task SendNotification(ChangeClassRunStatusCommand command, IEnumerable<ClassRun> classRuns, Dictionary<Guid, CourseEntity> courseDict, CancellationToken cancellationToken)
        {
            switch (command.Status)
            {
                case ClassRunStatus.Published:
                    await SendNotificationWhenPublishedClassRun(classRuns, courseDict, cancellationToken);
                    break;
                default:
                    break;
            }
        }

        private async Task SendNotificationWhenPublishedClassRun(IEnumerable<ClassRun> classRuns, Dictionary<Guid, CourseEntity> courseDict, CancellationToken cancellationToken)
        {
            if (courseDict == null || courseDict.Count == 0)
            {
                return;
            }

            await _thunderCqrs.SendEvents(
                classRuns
                    .Where(classRun => courseDict.ContainsKey(classRun.CourseId) && courseDict[classRun.CourseId].CollaborativeContentCreatorIds.Any())
                    .Select(p =>
                        {
                            var course = courseDict[p.CourseId];
                            return new PublishedClassRunNotifyCollaboratorEvent(
                                 CurrentUserIdOrDefault,
                                 new PublishedClassRunNotifyCollaboratorPayload
                                 {
                                     CourseTitle = course.CourseName,
                                     CoursePDArea = course.PDAreaThemeId,
                                     ActionUrl = _webAppLinkBuilder.GetClassRunDetailLinkForLMMModule(
                                         LMMTabConfigurationConstant.CoursesTab,
                                         LMMTabConfigurationConstant.ClassRunsTab,
                                         LMMTabConfigurationConstant.AllClassRunsTab,
                                         CourseDetailModeConstant.View,
                                         LMMTabConfigurationConstant.ClassRunInfoTab,
                                         ClassRunDetailModeConstant.View,
                                         course.Id,
                                         p.Id)
                                 },
                                 course.CollaborativeContentCreatorIds.ToList());
                        }),
                cancellationToken);
        }

        private async Task<bool> HasValidPermission(List<ClassRun> classRuns, CancellationToken cancellationToken)
        {
            var courseIds = classRuns.Select(x => x.CourseId).ToList();
            var hasPermissionCourseIds = await _readCourseRepository
                .GetAllWithAccessControl(
                    AccessControlContext,
                    ClassRun.HasPublishUnpublishPermissionExpr(CurrentUserId, CurrentUserRoles, p => HasPermissionPrefix(p)))
                .Select(x => x.Id).ToListAsync(cancellationToken);

            return courseIds.All(id => hasPermissionCourseIds.Contains(id));
        }

        private async Task GenerateClassRunCode(
            List<ClassRun> classRuns,
            List<Guid> courseIds,
            Dictionary<Guid, CourseEntity> courseDict,
            CancellationToken cancellationToken = default)
        {
            if (classRuns == null || !classRuns.Any() || courseDict == null)
            {
                return;
            }

            var existedClassRuns = await _readClassRunRepository.GetAll()
                .Where(x => !string.IsNullOrEmpty(x.ClassRunCode) && courseIds.Contains(x.CourseId)).ToListAsync(cancellationToken);

            var existedClassRunCodes = existedClassRuns.Select(x => x.ClassRunCode).Distinct().ToHashSet();

            var existedClassRunTotalDict = existedClassRuns
                .GroupBy(x => x.CourseId)
                .Select(x => new { x.Key, Total = x.Count() })
                .ToDictionary(x => x.Key, x => x.Total);

            foreach (var classRun in classRuns)
            {
                if (!existedClassRunTotalDict.ContainsKey(classRun.CourseId))
                {
                    existedClassRunTotalDict.Add(classRun.CourseId, 0);
                }

                do
                {
                    var nextClassRunNumber = existedClassRunTotalDict[classRun.CourseId] + 1;
                    var courseCode = courseDict[classRun.CourseId].CourseCode;
                    classRun.ClassRunCode = $"{courseCode}-{nextClassRunNumber.ToString(EntitiesConstants.UniqueClassRunNumberLength)}";
                    existedClassRunTotalDict[classRun.CourseId] = nextClassRunNumber;
                }
                while (existedClassRunCodes.Contains(classRun.ClassRunCode));
            }
        }

        private async Task<Dictionary<Guid, CourseEntity>> BuildCourseDictionary(List<Guid> courseIds, CancellationToken cancellationToken = default)
        {
            var courseDict = await _readCourseRepository
                .GetAll()
                .Where(x => courseIds.Contains(x.Id))
                .Select(x => new { x.Id, Course = x })
                .ToDictionaryAsync(x => x.Id, x => x.Course, cancellationToken);

            return courseDict;
        }
    }
}
