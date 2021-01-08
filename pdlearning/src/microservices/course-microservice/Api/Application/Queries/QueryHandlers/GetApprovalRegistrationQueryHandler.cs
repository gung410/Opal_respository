using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Microservice.Course.Application.Queries.Abstracts;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Queries.QueryHandlers
{
    public class GetApprovalRegistrationQueryHandler : BaseQueryHandler<GetApprovalRegistrationQuery, PagedResultDto<RegistrationModel>>
    {
        private readonly IReadOnlyRepository<Registration> _readRegistrationRepository;
        private readonly IReadOnlyRepository<ClassRun> _readClassRunRepository;

        public GetApprovalRegistrationQueryHandler(
            IReadOnlyRepository<Registration> readRegistrationRepository,
            IUserContext userContext,
            IAccessControlContext<CourseUser> accessControlContext,
            IUnitOfWorkManager unitOfWorkManager,
            IReadOnlyRepository<ClassRun> readClassRunRepository) : base(userContext, accessControlContext, unitOfWorkManager)
        {
            _readRegistrationRepository = readRegistrationRepository;
            _readClassRunRepository = readClassRunRepository;
        }

        protected override async Task<PagedResultDto<RegistrationModel>> HandleAsync(
            GetApprovalRegistrationQuery query,
            CancellationToken cancellationToken)
        {
            var publishedAndNotStartedClassRuns = _readClassRunRepository.GetAll()
                .Where(x => x.Status == ClassRunStatus.Published && (x.EndDateTime == null || x.EndDateTime > Clock.Now));

            var dbQuery = _readRegistrationRepository
                .GetAll()
                .Join(publishedAndNotStartedClassRuns, x => x.ClassRunId, q => q.Id, (registration, classRun) => registration)
                .Where(x => (x.ApprovingOfficer == CurrentUserId || x.AlternativeApprovingOfficer == CurrentUserId))
                .Where(Registration.IsExistedExpr())
                .Distinct();
            switch (query.FilterType)
            {
                case ApprovalRegistrationFilterType.Registration:
                    {
                        if (query.RegistrationStatuses != null && query.RegistrationStatuses.Any())
                        {
                            dbQuery = dbQuery.Where(p => query.RegistrationStatuses.Contains(p.Status));
                        }
                        else
                        {
                            dbQuery = dbQuery.Where(x => x.Status == RegistrationStatus.PendingConfirmation);
                        }

                        dbQuery = dbQuery.WhereIf(query.RegistrationStartDate.HasValue, p => p.RegistrationDate >= query.RegistrationStartDate);
                        dbQuery = dbQuery.WhereIf(query.RegistrationEndDate.HasValue, p => p.RegistrationDate <= query.RegistrationEndDate);

                        break;
                    }

                case ApprovalRegistrationFilterType.Withdraw:
                    {
                        if (query.WithdrawalStatuses != null && query.WithdrawalStatuses.Any())
                        {
                            dbQuery = dbQuery.Where(p => query.WithdrawalStatuses.Contains(p.WithdrawalStatus.Value));
                        }
                        else
                        {
                            dbQuery = dbQuery.Where(x => x.WithdrawalStatus == WithdrawalStatus.PendingConfirmation);
                        }

                        dbQuery = dbQuery.WhereIf(query.WithdrawalStartDate.HasValue, p => p.WithdrawalRequestDate >= query.WithdrawalStartDate);
                        dbQuery = dbQuery.WhereIf(query.WithdrawalEndDate.HasValue, p => p.WithdrawalRequestDate <= query.WithdrawalEndDate);

                        break;
                    }

                case ApprovalRegistrationFilterType.ClassRunChange:
                    {
                        if (query.ClassRunChangeStatuses != null && query.ClassRunChangeStatuses.Any())
                        {
                            dbQuery = dbQuery.Where(p => query.ClassRunChangeStatuses.Contains(p.ClassRunChangeStatus.Value));
                        }
                        else
                        {
                            dbQuery = dbQuery.Where(x => x.ClassRunChangeStatus == ClassRunChangeStatus.PendingConfirmation);
                        }

                        dbQuery = dbQuery.WhereIf(query.ClassRunChangeRequestedStartDate.HasValue, p => p.ClassRunChangeRequestedDate >= query.ClassRunChangeRequestedStartDate);
                        dbQuery = dbQuery.WhereIf(query.ClassRunChangeRequestedEndDate.HasValue, p => p.ClassRunChangeRequestedDate <= query.ClassRunChangeRequestedEndDate);

                        break;
                    }
            }

            dbQuery = dbQuery.WhereIf(query.CourseId.HasValue, p => p.CourseId == query.CourseId);

            dbQuery = dbQuery.WhereIf(
                query.ClassRunIds != null && query.ClassRunIds.Any(),
                p => query.ClassRunIds.Contains(p.ClassRunId));

            dbQuery = dbQuery.WhereIf(
                query.LearnerIds != null && query.LearnerIds.Any(),
                p => query.LearnerIds.Contains(p.UserId));

            var totalCount = await dbQuery.CountAsync(cancellationToken);

            dbQuery = dbQuery.OrderByDescending(p => p.ChangedDate ?? p.CreatedDate);

            dbQuery = ApplyPaging(dbQuery, query.PageInfo);

            var data = await dbQuery.Select(p => new RegistrationModel(p)).ToListAsync(cancellationToken);

            return new PagedResultDto<RegistrationModel>(totalCount, data);
        }
    }
}
