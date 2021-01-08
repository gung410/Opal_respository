using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.Constants;
using Microservice.Course.Application.Events.Todos;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Settings;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeCourseContentStatusCommandHandler : BaseCommandHandler<ChangeCourseContentStatusCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly ClassRunCudLogic _classRunCudLogic;
        private readonly CourseCudLogic _courseCudLogic;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly EnsureCanChangeContentStatusLogic _ensureCanChangeContentStatusLogic;
        private readonly WebAppLinkBuilder _webAppLinkBuilder;
        private readonly GetAggregatedClassRunSharedQuery _getAggregatedClassRunSharedQuery;
        private readonly CheckClassRunHasContentLogic _checkClassRunHasContentLogic;

        public ChangeCourseContentStatusCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            ClassRunCudLogic classRunCudLogic,
            CourseCudLogic courseCudLogic,
            IThunderCqrs thunderCqrs,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            EnsureCanChangeContentStatusLogic ensureCanChangeContentStatusLogic,
            GetAggregatedClassRunSharedQuery getAggregatedClassRunSharedQuery,
            CheckClassRunHasContentLogic checkClassRunHasContentLogic,
            WebAppLinkBuilder webAppLinkBuilder) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _classRunCudLogic = classRunCudLogic;
            _courseCudLogic = courseCudLogic;
            _thunderCqrs = thunderCqrs;
            _ensureCanChangeContentStatusLogic = ensureCanChangeContentStatusLogic;
            _webAppLinkBuilder = webAppLinkBuilder;
            _getAggregatedClassRunSharedQuery = getAggregatedClassRunSharedQuery;
            _checkClassRunHasContentLogic = checkClassRunHasContentLogic;
        }

        protected override async Task HandleAsync(ChangeCourseContentStatusCommand command, CancellationToken cancellationToken)
        {
            var classRunAggregatedEntityModels = await _getAggregatedClassRunSharedQuery.ByCourseIds(command.Ids, true, cancellationToken);
            var courseToClassRunDict = classRunAggregatedEntityModels
                .GroupBy(x => x.Course.Id)
                .ToDictionary(x => x.Key, x => x.Where(c => c != null).ToList());
            var classRunHasContentDic = await _checkClassRunHasContentLogic
                .ByClassRunIds(courseToClassRunDict.SelectMany(p => p.Value).Select(p => p.ClassRun.Id).ToList(), cancellationToken);

            var courses = await _readCourseRepository.GetAllListAsync(p => command.Ids.Contains(p.Id));
            var courseHasAdminRightChecker = _readCourseRepository.GetHasAdminRightChecker(courses, AccessControlContext);

            foreach (var course in courses)
            {
                _ensureCanChangeContentStatusLogic.EnsureForCourse(
                    course,
                    command.ContentStatus,
                    courseHasAdminRightChecker);

                switch (command.ContentStatus)
                {
                    case ContentStatus.Published:
                        course.PublishedContentDate = Clock.Now;
                        break;
                    case ContentStatus.PendingApproval:
                        course.SubmittedContentDate = Clock.Now;
                        break;
                    case ContentStatus.Approved:
                    case ContentStatus.Rejected:
                        course.ApprovalContentDate = Clock.Now;
                        break;
                }

                course.ContentStatus = command.ContentStatus;
                course.ChangedBy = CurrentUserId ?? Guid.Empty;
                course.ChangedDate = Clock.Now;

                if (courseToClassRunDict.ContainsKey(course.Id))
                {
                    var noContentClassRuns = courseToClassRunDict[course.Id]
                        .Where(x => !classRunHasContentDic[x.ClassRun.Id]);
                    var updatedContentStatusClassruns = noContentClassRuns
                        .Select(x =>
                        {
                            x.ClassRun.ContentStatus = command.ContentStatus;
                            x.ClassRun.ChangedBy = CurrentUserId;
                            x.ClassRun.ChangedDate = Clock.Now;
                            return x;
                        })
                        .ToList();

                    await _classRunCudLogic.UpdateMany(updatedContentStatusClassruns, cancellationToken);
                }
            }

            await _courseCudLogic.UpdateMany(courses, cancellationToken);

            if (command.ContentStatus == ContentStatus.PendingApproval)
            {
                var notifyApproverEvent = courses.Where(x => x.PrimaryApprovingOfficerId.HasValue).Select(p =>
                    new SubmittedCourseContentNotifyApproverEvent(
                        CurrentUserIdOrDefault,
                        new SubmittedCourseContentNotifyApproverPayload
                        {
                            CourseName = p.CourseName,
                            ActionUrl = _webAppLinkBuilder.GetCourseDetailLinkForLMMModule(
                                            LMMTabConfigurationConstant.PendingApprovalTab,
                                            LMMTabConfigurationConstant.CourseInfoTab,
                                            LMMTabConfigurationConstant.AllClassRunsTab,
                                            CourseDetailModeConstant.ForApprover,
                                            p.Id)
                        },
                        new List<Guid> { p.PrimaryApprovingOfficerId.GetValueOrDefault() }));

                await _thunderCqrs.SendEvents(notifyApproverEvent, cancellationToken);
            }
        }
    }
}
