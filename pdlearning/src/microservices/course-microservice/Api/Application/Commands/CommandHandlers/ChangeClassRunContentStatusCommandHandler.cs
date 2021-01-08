using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Extensions;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.BusinessLogics;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Application.Commands.Abstracts;
using Microservice.Course.Application.SharedQueries;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Commands.CommandHandlers
{
    public class ChangeClassRunContentStatusCommandHandler : BaseCommandHandler<ChangeClassRunContentStatusCommand>
    {
        private readonly IReadOnlyRepository<CourseEntity> _readCourseRepository;
        private readonly GetAggregatedClassRunSharedQuery _aggregatedClassRunSharedQuery;
        private readonly EnsureCanChangeContentStatusLogic _ensureCanChangeContentStatusLogic;
        private readonly ClassRunCudLogic _classRunCudLogic;

        public ChangeClassRunContentStatusCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<CourseEntity> readCourseRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            EnsureCanChangeContentStatusLogic ensureCanChangeContentStatusLogic,
            ClassRunCudLogic classRunCudLogic,
            GetAggregatedClassRunSharedQuery aggregatedClassRunSharedQuery) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readCourseRepository = readCourseRepository;
            _ensureCanChangeContentStatusLogic = ensureCanChangeContentStatusLogic;
            _classRunCudLogic = classRunCudLogic;
            _aggregatedClassRunSharedQuery = aggregatedClassRunSharedQuery;
        }

        protected override async Task HandleAsync(ChangeClassRunContentStatusCommand command, CancellationToken cancellationToken)
        {
            var aggregatedClassRuns = await _aggregatedClassRunSharedQuery.ByClassRunIds(command.Ids, true, cancellationToken);
            if (!aggregatedClassRuns.Any())
            {
                return;
            }

            var courseIds = aggregatedClassRuns.Select(x => x.ClassRun.CourseId).ToList();
            var courseQuery = _readCourseRepository
                .GetAll()
                .Where(x => courseIds.Contains(x.Id));

            var courseDict = await courseQuery.ToDictionaryAsync(x => x.Id, x => x, cancellationToken);
            var courseHasAdminRightChecker = courseQuery.GetHasAdminRightChecker(AccessControlContext);

            aggregatedClassRuns.ForEach(aggregatedClassRun =>
            {
                _ensureCanChangeContentStatusLogic.EnsureForClassrun(
                    courseDict[aggregatedClassRun.ClassRun.CourseId],
                    aggregatedClassRun.ClassRun,
                    command.ContentStatus,
                    courseHasAdminRightChecker);

                switch (command.ContentStatus)
                {
                    case ContentStatus.Published:
                        aggregatedClassRun.ClassRun.PublishedContentDate = Clock.Now;
                        break;
                    case ContentStatus.PendingApproval:
                        aggregatedClassRun.ClassRun.SubmittedContentDate = Clock.Now;
                        break;
                    case ContentStatus.Approved:
                    case ContentStatus.Rejected:
                        aggregatedClassRun.ClassRun.ApprovalContentDate = Clock.Now;

                        break;
                }

                aggregatedClassRun.ClassRun.ContentStatus = command.ContentStatus;
                aggregatedClassRun.ClassRun.ChangedBy = CurrentUserId;
                aggregatedClassRun.ClassRun.ChangedDate = Clock.Now;
            });

            await _classRunCudLogic.UpdateMany(aggregatedClassRuns, cancellationToken);
        }
    }
}
