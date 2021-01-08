using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class UpdateUserSharingCommandHandler : BaseCommandHandler<UpdateUserSharingCommand>
    {
        private readonly IRepository<UserSharing> _userSharingRepository;
        private readonly IRepository<UserSharingDetail> _userSharingDetailRepository;

        public UpdateUserSharingCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserSharing> userSharingRepository,
            IRepository<UserSharingDetail> userSharingDetailRepository,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _userSharingRepository = userSharingRepository;
            _userSharingDetailRepository = userSharingDetailRepository;
        }

        protected override async Task HandleAsync(UpdateUserSharingCommand command, CancellationToken cancellationToken)
        {
            var currentUserSharing = await _userSharingRepository
                .GetAll()
                .Where(f => f.Id == command.Id)
                .Where(f => f.CreatedBy == CurrentUserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentUserSharing == null)
            {
                throw new EntityNotFoundException(typeof(UserSharing), command.Id);
            }

            var currentUserSharingDetails = await _userSharingDetailRepository.GetAllListAsync(p => p.UserSharingId == currentUserSharing.Id);
            var currentUserSharingDetailsDic = currentUserSharingDetails.ToDictionary(p => p.Id);

            var toUpdatedUserSharingDetail = command.UsersShared
               .Where(p => p.Id.HasValue)
               .Select(p =>
               {
                   p.UpdateExistUserSharingDetail(currentUserSharingDetailsDic[p.Id.Value]);
                   return currentUserSharingDetailsDic[p.Id.Value];
               })
               .ToList();

            var toInsertUserSharingDetail = command.UsersShared
               .Where(p => p.Id == null)
               .Select(p => p.CreateNewUserSharingDetail(currentUserSharing.Id, p.UserId))
               .ToList();

            var listUserShared = command.UsersShared.Where(_ => _.Id.HasValue).Select(_ => _.Id).ToList();
            var toDeleteUserSharingDetail = currentUserSharingDetails
                .Where(p => !listUserShared.Contains(p.Id));

            await _userSharingRepository.UpdateAsync(currentUserSharing);
            await _userSharingDetailRepository.DeleteManyAsync(toDeleteUserSharingDetail);
            await _userSharingDetailRepository.UpdateManyAsync(toUpdatedUserSharingDetail);
            await _userSharingDetailRepository.InsertManyAsync(toInsertUserSharingDetail);
        }
    }
}
