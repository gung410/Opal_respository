using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Domain.UnitOfWork.Abstractions;
using Thunder.Platform.Core.Extensions;
using Thunder.Platform.Core.Timing;

namespace Microservice.Learner.Application.Commands.CommandHandlers
{
    public class InitPredefineUserPreferenceForUserCommandHandler : BaseCommandHandler<InitPredefineUserPreferenceForUserCommand>
    {
        private readonly IRepository<UserPreference> _userPreferenceRepository;

        public InitPredefineUserPreferenceForUserCommandHandler(
            IRepository<UserPreference> userPreferenceRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IUserContext userContext) : base(unitOfWorkManager, userContext)
        {
            _userPreferenceRepository = userPreferenceRepository;
        }

        protected override async Task HandleAsync(InitPredefineUserPreferenceForUserCommand command, CancellationToken cancellationToken)
        {
            var userPreferences = UserPreferenceKeyMapConfig.PredefinedConfiguration.Keys
                .Where(k => command.Keys.IsNullOrEmpty() || command.Keys.Contains(k)) // Get full if keys is null or empty otherwise get keys only
                .Select(k => new UserPreference
                {
                    Key = k,
                    CreatedDate = Clock.Now,
                    UserId = CurrentUserIdOrDefault,
                    ValueString = UserPreferenceKeyMapConfig.GetDefaultValueString(k),
                    ValueType = UserPreferenceKeyMapConfig.GetValueTypeOfKey(k)
                }).ToList();

            await _userPreferenceRepository.InsertManyAsync(userPreferences);
        }
    }
}
