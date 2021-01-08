using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conexus.Opal.Microservice.Metadata.Entities;
using Conexus.Opal.Microservice.Tagging.DataProviders;
using Conexus.Opal.Microservice.Tagging.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;

namespace Conexus.Opal.Microservice.Tagging.Controllers
{
    [Route("api")]
    public class UserSuggestionController : ApplicationApiController
    {
        private readonly ITaggingDataProvider _taggingDataProvider;

        public UserSuggestionController(IUserContext userContext, ITaggingDataProvider taggingDataProvider) : base(userContext)
        {
            _taggingDataProvider = taggingDataProvider;
        }

        /// <summary>
        /// To get suggested courses for a specific user.
        /// </summary>
        /// <param name="userId">A specific user by id.</param>
        /// <param name="userTagIds">Tag ids that tagged for a user.</param>
        /// <returns>A list of suggested course identifiers.</returns>
        [HttpPost("users/{userId:guid}/suggestion/courses")]
#pragma warning disable CA1801 // Review unused parameters
        public async Task<IEnumerable<Guid>> GetSuggestedCoursesForAUser(Guid userId, [FromBody] IEnumerable<Guid> userTagIds)
#pragma warning restore CA1801 // Review unused parameters
        {
            return await _taggingDataProvider.GetSuggestedResourcesByTags(userTagIds, ResourceType.Course);
        }

        [HttpPost("users/{userId:guid}/suggestion/contents")]
#pragma warning disable CA1801 // Review unused parameters
        public async Task<IEnumerable<Guid>> GetSuggestedContentsForAUser(Guid userId, [FromBody] IEnumerable<Guid> userTagIds)
#pragma warning restore CA1801 // Review unused parameters
        {
            return await _taggingDataProvider.GetSuggestedResourcesByTags(userTagIds, ResourceType.Content);
        }
    }
}
