using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyAssignmentsByAssignmentIdsQueryHandler : BaseQueryHandler<GetMyAssignmentsByAssignmentIdsQuery, List<MyAssignmentModel>>
    {
        private readonly IRepository<MyAssignment> _myAssignmentRepository;

        public GetMyAssignmentsByAssignmentIdsQueryHandler(
            IRepository<MyAssignment> myAssignmentRepository,
            IUserContext userContext) : base(userContext)
        {
            _myAssignmentRepository = myAssignmentRepository;
        }

        protected override Task<List<MyAssignmentModel>> HandleAsync(GetMyAssignmentsByAssignmentIdsQuery query, CancellationToken cancellationToken)
        {
            if (!query.AssignmentIds.Any())
            {
                return Task.FromResult(new List<MyAssignmentModel>());
            }

            return _myAssignmentRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => query.AssignmentIds.Contains(p.AssignmentId))
                .Select(p => new MyAssignmentModel(p))
                .ToListAsync(cancellationToken);
        }
    }
}
