using System.Collections.Generic;
using System.Threading.Tasks;
using Microservice.Calendar.Application.Models;
using Microservice.Calendar.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;

namespace Microservice.Calendar.Controllers
{
    [Route("api/communities")]
    public class CommunityController : ApplicationApiController
    {
        private readonly ICommunityApplicationService _service;

        public CommunityController(
            ICommunityApplicationService service,
            IUserContext userContext)
            : base(userContext)
        {
            _service = service;
        }

        [HttpGet]
        [Route("hierarchy")]
        public Task<List<CommunityModel>> GetCommunityHierarchy()
        {
            return _service.GetCommunityHierarchyTree(CurrentUserId);
        }

        [HttpGet]
        [Route("own-communities")]
        public Task<List<CommunityModel>> GetOwnCommunities()
        {
            return _service.GetOwnCommunities(CurrentUserId);
        }
    }
}
