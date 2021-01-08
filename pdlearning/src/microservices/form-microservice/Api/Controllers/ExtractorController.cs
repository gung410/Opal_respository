using System.Threading.Tasks;
using Microservice.Form.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Service.Authentication;

namespace Microservice.Form.Controllers
{
    [Route("api/form/extractor")]
    public class ExtractorController : ApplicationApiController
    {
        private readonly IFormUrlExtractor _formUrlExtractor;

        public ExtractorController(IFormUrlExtractor formUrlExtractor, IUserContext userContext) : base(userContext)
        {
            _formUrlExtractor = formUrlExtractor;
        }

        [HttpPost("extract-all")]
        public async Task ExtractAll()
        {
            if (!UserContext.IsSysAdministrator())
            {
                throw new BusinessLogicException("You do not have permission to access this endpoint");
            }

            await _formUrlExtractor.ExtractAll();
        }
    }
}
