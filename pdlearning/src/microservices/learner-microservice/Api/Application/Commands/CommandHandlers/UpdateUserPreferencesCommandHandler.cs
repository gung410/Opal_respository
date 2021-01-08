using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Application.Commands.UpdateUserPreferences;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using UserPreference = Microservice.Learner.Domain.Entities.UserPreference;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class UpdateUserPreferencesCommandHandler : BaseCommandHandler<UpdateUserPreferencesCommand>
    {
        private readonly IRepository<UserPreference> _userPreferenceRepository;

        public UpdateUserPreferencesCommandHandler(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserPreference> userPreferenceRepository,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _userPreferenceRepository = userPreferenceRepository;
        }

        protected override async Task HandleAsync(UpdateUserPreferencesCommand command, CancellationToken cancellationToken)
        {
            var userPreferenceKey = command.UserPreferences.Select(u => u.Key).ToList();

            var userPreferences = await _userPreferenceRepository
                .GetAll()
                .Where(u => u.UserId == CurrentUserId)
                .Where(u => userPreferenceKey.Contains(u.Key))
                .ToDictionaryAsync(u => u.Key, u => u, cancellationToken);

            var listUserPreferenceNeedUpdated = new List<UserPreference>();
            foreach (var up in command.UserPreferences)
            {
                if (userPreferences.ContainsKey(up.Key))
                {
                    var userPreference = userPreferences[up.Key];
                    userPreference.ValueString = up.ValueString;

                    // todo: check valid value before update new
                    listUserPreferenceNeedUpdated.Add(userPreference);
                }
            }

            await _userPreferenceRepository.UpdateManyAsync(listUserPreferenceNeedUpdated);
        }
    }
}
