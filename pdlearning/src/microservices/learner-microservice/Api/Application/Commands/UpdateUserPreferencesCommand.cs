using System.Collections.Generic;
using Thunder.Platform.Cqrs;

// ReSharper disable once CheckNamespace
namespace Microservice.Learner.Application.Commands.UpdateUserPreferences
{
    public class UpdateUserPreferencesCommand : BaseThunderCommand
    {
        public List<UserPreference> UserPreferences { get; set; }
    }

    public class UserPreference
    {
        public string Key { get; set; }

        public string ValueString { get; set; }
    }
}
