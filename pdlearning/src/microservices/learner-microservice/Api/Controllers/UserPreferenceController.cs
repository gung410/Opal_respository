using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Domain.Constants.PermissionKeys;
using Microservice.Learner.Application.Commands;
using Microservice.Learner.Application.Commands.UpdateUserPreferences;
using Microservice.Learner.Application.Dtos;
using Microservice.Learner.Application.Models;
using Microservice.Learner.Application.Queries;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;
using Thunder.Service.Authentication;

namespace Microservice.Learner.Controllers
{
    [Route("api/userPreferences")]
    public class UserPreferenceController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;

        public UserPreferenceController(IUserContext userContext, IThunderCqrs thunderCqrs) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
        }

        /// <summary>
        /// Get all user preferences or a part. If no preference has found create preference with default value.
        /// </summary>
        /// <param name="keys">empty if want to get all otherwise get specific user preferences.</param>
        /// <returns>user preference with key and value.</returns>
        [PermissionRequired(LearnerPermissionKeys.HomeSetting)]
        [HttpPost("get")]
        public async Task<List<UserPreferenceModel>> GetAllUserPreferences([FromBody] List<string> keys)
        {
            var userPreferenceModels = await _thunderCqrs.SendQuery(new GetAllUserPreferencesQuery(keys));

            if (!userPreferenceModels.Any())
            {
                await _thunderCqrs.SendCommand(new InitPredefineUserPreferenceForUserCommand(keys));
            }
            else if (userPreferenceModels.Count != UserPreferenceKeyMapConfig.PredefinedConfiguration.Keys.Count())
            {
                var listUserPreferences = userPreferenceModels.Select(x => x.Key);
                var missingPreference =
                    UserPreferenceKeyMapConfig.PredefinedConfiguration.Keys
                    .Where(x => !listUserPreferences.Contains(x))
                    .Select(p => p.ToString())
                    .ToList();

                await _thunderCqrs.SendCommand(new InitPredefineUserPreferenceForUserCommand(missingPreference));
            }

            return await _thunderCqrs.SendQuery(new GetAllUserPreferencesQuery(keys));
        }

        [PermissionRequired(LearnerPermissionKeys.HomeSetting)]
        [HttpPut]
        public async Task UpdateUserPreferences([FromBody] List<UpdateUserPreferenceRequest> request)
        {
            await _thunderCqrs.SendCommand(new UpdateUserPreferencesCommand
            {
                UserPreferences = request
                    .Select(_ => new UserPreference
                    {
                        Key = _.Key,
                        ValueString = _.ValueString
                    })
                    .ToList()
            });
        }
    }
}
