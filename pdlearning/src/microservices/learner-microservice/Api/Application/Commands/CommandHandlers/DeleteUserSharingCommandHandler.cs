using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class DeleteUserSharingCommandHandler : BaseCommandHandler<DeleteUserSharingCommand>
    {
        private readonly IRepository<UserSharing> _userSharingRepository;
        private readonly IRepository<UserSharingDetail> _userSharingDetailRepository;
        private readonly IRepository<LearnerLearningPath> _learningPathRepository;

        public DeleteUserSharingCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserSharing> userSharingRepository,
            IRepository<UserSharingDetail> userSharingDetailRepository,
            IRepository<LearnerLearningPath> learningPathRepository,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _userSharingRepository = userSharingRepository;
            _userSharingDetailRepository = userSharingDetailRepository;
            _learningPathRepository = learningPathRepository;
        }

        protected override async Task HandleAsync(DeleteUserSharingCommand command, CancellationToken cancellationToken)
        {
            var userSharing = await _userSharingRepository.GetAsync(command.Id);

            var userSharingDetails = await _userSharingDetailRepository
                .GetAll()
                .Where(p => p.UserSharingId == command.Id)
                .ToListAsync(cancellationToken);

            await _userSharingRepository.DeleteAsync(userSharing);
            await _userSharingDetailRepository.DeleteManyAsync(userSharingDetails);
        }
    }
}
