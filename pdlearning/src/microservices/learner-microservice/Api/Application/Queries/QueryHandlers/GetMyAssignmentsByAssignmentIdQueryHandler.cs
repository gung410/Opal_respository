using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Queries.QueryHandlers
{
    public class GetMyAssignmentsByAssignmentIdQueryHandler : BaseQueryHandler<GetMyAssignmentsByAssignmentIdQuery, MyAssignmentModel>
    {
        private readonly IRepository<MyAssignment> _myAssignmentRepository;

        public GetMyAssignmentsByAssignmentIdQueryHandler(
            IRepository<MyAssignment> myAssignmentRepository,
            IUserContext userContext) : base(userContext)
        {
            _myAssignmentRepository = myAssignmentRepository;
        }

        protected override async Task<MyAssignmentModel> HandleAsync(GetMyAssignmentsByAssignmentIdQuery query, CancellationToken cancellationToken)
        {
            var myAssignment = await _myAssignmentRepository
                .GetAll()
                .Where(p => p.UserId == CurrentUserId)
                .Where(p => p.AssignmentId == query.AssignmentId)
                .Select(p => new MyAssignmentModel(p))
                .FirstOrDefaultAsync(cancellationToken);

            if (myAssignment == null)
            {
                throw new EntityNotFoundException(typeof(MyAssignment), query.AssignmentId);
            }

            return myAssignment;
        }
    }
}
