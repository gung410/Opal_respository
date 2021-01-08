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
    public class MySuggestionController : ApplicationApiController
    {
        private readonly ITaggingDataProvider _taggingDataProvider;

        public MySuggestionController(IUserContext userContext, ITaggingDataProvider taggingDataProvider) : base(userContext)
        {
            _taggingDataProvider = taggingDataProvider;
        }

        [HttpPost("me/suggestion/courses")]
        public async Task<IEnumerable<Guid>> GetSuggestedCoursesForMe([FromBody] IEnumerable<Guid> userTagIds)
        {
            return await _taggingDataProvider.GetSuggestedResourcesByTags(userTagIds, ResourceType.Course);
        }

        [HttpPost("me/suggestion/contents")]
        public async Task<IEnumerable<Guid>> GetSuggestedContentsForMe([FromBody] IEnumerable<Guid> userTagIds)
        {
            return await _taggingDataProvider.GetSuggestedResourcesByTags(userTagIds, ResourceType.Course);
        }
    }
}
