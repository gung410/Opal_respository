using System.Threading.Tasks;
using Microservice.NewsFeed.Application.Commands;
using Microservice.NewsFeed.Application.Dtos;
using Microservice.NewsFeed.Application.Models;
using Microservice.NewsFeed.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Cqrs;

namespace Microservice.NewsFeed.Controllers
{
    [Route("api/newsFeed")]
    public class NewsFeedController : ApplicationApiController
    {
        private readonly IThunderCqrs _thunderCqrs;

        public NewsFeedController(IUserContext userContext, IThunderCqrs thunderCqrs) : base(userContext)
        {
            _thunderCqrs = thunderCqrs;
        }

        [HttpPost]
        public async Task<FeedResultModel> Get([FromBody] GetNewsFeedRequest request)
        {
            var query = new GetNewsFeedQuery
            {
                UserId = CurrentUserId,
                MaxResultCount = request.MaxResultCount,
                SkipCount = request.SkipCount
            };

            return await _thunderCqrs.SendQuery(query);
        }

        /// <summary>
        /// For development, uat-next.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <returns>No results are returned.</returns>
        [HttpPost("migrateUserTempToUser")]
        public async Task ConvertUserTempCollectionToUserCollection([FromBody] MigrationUserRequest request)
        {
            await _thunderCqrs.SendCommand(new MigrateUserCommand { BatchSize = request.BatchSize });
        }
    }
}
