using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Application.RequestDtos;
using Microservice.StandaloneSurvey.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;
using Thunder.Platform.Core.Exceptions;
using Thunder.Service.Authentication;

namespace Microservice.StandaloneSurvey.Controllers
{
    [Route("api/standalone-survey/extractor")]
    public class ExtractorController : ApplicationApiController
    {
        private readonly ISurveyUrlExtractor _surveyUrlExtractor;

        public ExtractorController(ISurveyUrlExtractor surveyUrlExtractor, IUserContext userContext) : base(userContext)
        {
            _surveyUrlExtractor = surveyUrlExtractor;
        }

        [HttpPost("extract-all")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Pending")]
        public async Task ExtractAll(SubmoduleRequest request)
        {
            if (!UserContext.IsSysAdministrator())
            {
                throw new BusinessLogicException("You do not have permission to access this endpoint");
            }

            await _surveyUrlExtractor.ExtractAll();
        }
    }
}
