using System.Threading.Tasks;
using Microservice.Content.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Service.Authentication;

namespace Microservice.Content.Controllers
{
    [Route("api/extractor")]
    public class ExtractorController : ApplicationApiController
    {
        private readonly IContentUrlExtractor _appService;

        public ExtractorController(IContentUrlExtractor appService, IUserContext userContext) : base(userContext)
        {
            _appService = appService;
        }

        [HttpPost("extract-all")]
        public async Task ExtractAll()
        {
            if (!UserContext.IsSysAdministrator())
            {
                throw new BusinessLogicException("You do not have permission to access this endpoint");
            }

            await _appService.ExtractAll();
        }
    }
}
