using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Exceptions;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class EnablePublicLearningPathCommandHandler : BaseCommandHandler<EnablePublicLearningPathCommand>
    {
        private readonly IRepository<LearnerLearningPath> _learnerLearningPath;

        public EnablePublicLearningPathCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<LearnerLearningPath> learnerLearningPath,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _learnerLearningPath = learnerLearningPath;
        }

        protected override async Task HandleAsync(EnablePublicLearningPathCommand command, CancellationToken cancellationToken)
        {
            var existingLearningPath = await _learnerLearningPath
                .GetAll()
                .Where(p => p.Id == command.Id)
                .Where(p => p.CreatedBy == CurrentUserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingLearningPath == null)
            {
                throw new NotOwnerLearningPathException();
            }

            existingLearningPath.Publish();

            await _learnerLearningPath.UpdateAsync(existingLearningPath);
        }
    }
}
