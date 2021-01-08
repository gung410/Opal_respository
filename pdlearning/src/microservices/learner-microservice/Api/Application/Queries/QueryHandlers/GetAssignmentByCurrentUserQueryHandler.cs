using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetAssignmentByCurrentUserQueryHandler : BaseQueryHandler<GetAssignmentByCurrentUserQuery, PagedResultDto<MyAssignmentModel>>
    {
        private readonly IRepository<MyAssignment> _myAssignmentRepository;

        public GetAssignmentByCurrentUserQueryHandler(
            IRepository<MyAssignment> myAssignmentRepository,
            IUserContext userContext) : base(userContext)
        {
            _myAssignmentRepository = myAssignmentRepository;
        }

        protected override async Task<PagedResultDto<MyAssignmentModel>> HandleAsync(GetAssignmentByCurrentUserQuery query, CancellationToken cancellationToken)
        {
            var myAssignmentsQuery = _myAssignmentRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.RegistrationId == query.RegistrationId);

            var totalCount = await myAssignmentsQuery.CountAsync(cancellationToken);

            myAssignmentsQuery = ApplySorting(myAssignmentsQuery, query.PageInfo, $"{nameof(MyAssignment.CreatedDate)} DESC");
            myAssignmentsQuery = ApplyPaging(myAssignmentsQuery, query.PageInfo);

            var myAssignments = await myAssignmentsQuery.Select(p => new MyAssignmentModel(p)).ToListAsync(cancellationToken);

            return new PagedResultDto<MyAssignmentModel>(totalCount, myAssignments);
        }
    }
}
