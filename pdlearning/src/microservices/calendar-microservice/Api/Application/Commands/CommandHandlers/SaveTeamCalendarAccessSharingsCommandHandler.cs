using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Extensions;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Commands
{
    public class SaveTeamCalendarAccessSharingsCommandHandler : BaseThunderCommandHandler<SaveTeamCalendarAccessSharingsCommand>
    {
        private readonly IRepository<TeamAccessSharing> _teamAccessSharingRepo;
        private readonly IRepository<CalendarUser> _userRepo;

        public SaveTeamCalendarAccessSharingsCommandHandler(
            IRepository<TeamAccessSharing> teamAccessSharingRepo,
            IRepository<CalendarUser> userRepo)
        {
            _teamAccessSharingRepo = teamAccessSharingRepo;
            _userRepo = userRepo;
        }

        protected override async Task HandleAsync(
            SaveTeamCalendarAccessSharingsCommand command,
            CancellationToken cancellationToken)
        {
            var requestUserIds = command.Request.UserIds;
            var currentAccessSharings = await _teamAccessSharingRepo
                .GetAccessSharingsAsync(
                    requestUserIds,
                    command.OwnerId,
                    _userRepo,
                    cancellationToken);

            switch (command.Request.Action)
            {
                case Domain.Enums.SaveTeamAccessAction.Share:
                    var shareEntites = MapNewAccessSharings(requestUserIds, currentAccessSharings, command.OwnerId);
                    await _teamAccessSharingRepo.InsertManyAsync(shareEntites);
                    break;
                case Domain.Enums.SaveTeamAccessAction.Unshare:
                    var unshareEntites = MapRevokingAccessSharings(requestUserIds, currentAccessSharings);
                    await _teamAccessSharingRepo.DeleteManyAsync(unshareEntites);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets <see cref="List{TeamAccessSharing}"/> of users that will be shared (inserted).
        /// </summary>
        /// <param name="grantingIds">The IDs of users to share with.</param>
        /// <param name="currentAccessSharings">The current <see cref="List{TeamAccessSharing}"/>.</param>
        /// <param name="ownerId">The ID of an Approval Officer who shares his Team Calendar.</param>
        /// <returns><see cref="TeamAccessSharing"/> entities will be inserted.</returns>
        private List<TeamAccessSharing> MapNewAccessSharings(
            List<Guid> grantingIds,
            List<TeamAccessSharing> currentAccessSharings,
            Guid ownerId)
        {
            return grantingIds.Where(userId => !currentAccessSharings.Any(a => a.UserId == userId))
                .Select(userId => new TeamAccessSharing
                {
                    Id = Guid.NewGuid(),
                    OwnerId = ownerId,
                    UserId = userId
                })
                .ToList();
        }

        /// <summary>
        /// Gets <see cref="List{TeamAccessSharing}"/> of users that will be unshared (deleted).
        /// </summary>
        /// <param name="revokingIds">The IDs of users to unshare with.</param>
        /// <param name="currentAccessSharings">The current <see cref="List{TeamAccessSharing}"/>.</param>
        /// <returns><see cref="TeamAccessSharing"/> entities will be deleted.</returns>
        private List<TeamAccessSharing> MapRevokingAccessSharings(
            List<Guid> revokingIds,
            List<TeamAccessSharing> currentAccessSharings)
        {
            return currentAccessSharings
                .Where(accessSharing => revokingIds.Any(id => id == accessSharing.UserId))
                .ToList();
        }
    }
}
