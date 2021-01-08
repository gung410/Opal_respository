using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.Queries;
using Thunder.Platform.Core.Application;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Services
{
    public class CommunityApplicationService : ApplicationService, ICommunityApplicationService
    {
        private readonly IThunderCqrs _thunderCqrs;

        public CommunityApplicationService(IThunderCqrs thunderCqrs)
        {
            _thunderCqrs = thunderCqrs;
        }

        /// <summary>
        /// Get community hierarchy tree, depth = 1.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>Community hierarchy tree.</returns>
        public Task<List<CommunityModel>> GetCommunityHierarchyTree(Guid userId)
        {
            var query = new GetCommunityHierarchyTreeQuery
            {
                UserId = userId
            };

            return _thunderCqrs.SendQuery(query);
        }

        /// <summary>
        /// Get communities that user is Owner or Admin.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>CommunityModel List.</returns>
        public Task<List<CommunityModel>> GetOwnCommunities(Guid userId)
        {
            var query = new GetOwnCommunityQuery
            {
                UserId = userId
            };

            return _thunderCqrs.SendQuery(query);
        }
    }
}
