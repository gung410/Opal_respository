using System.Threading.Tasks;
using Microservice.Course.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Service.Authentication;

namespace Microservice.Course.Controllers
{
    [Route("api/broken-links")]
    public class BrokenLinkController : ApplicationApiController
    {
        private readonly BrokenLinkService _brokenLinkService;

        public BrokenLinkController(
            BrokenLinkService brokenLinkService,
            IUserContext userContext) : base(userContext)
        {
            _brokenLinkService = brokenLinkService;
        }

        [HttpPost("extract-all")]
        public async Task ExtractAll()
        {
            EnsureValidPermission(UserContext.IsSysAdministrator());

            await _brokenLinkService.ExtractAll();
        }
    }
}
