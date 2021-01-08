using System;
using Microsoft.Extensions.Configuration;
using Thunder.Platform.Core.Application;

namespace Microservice.StandaloneSurvey.Application.Services
{
    public class WebAppLinkBuilder : ApplicationService
    {
        private readonly IConfiguration _configuration;

        public WebAppLinkBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetFormDetailLink(Guid formId, SubModule submodule)
        {
            var opalClientUrl = _configuration.GetValue<string>("OpalClientUrl");
            return $"{opalClientUrl}ccpm/standalonesurvey/{submodule}/{formId}";
        }

        public string GetStandaloneFormPlayerLink(Guid formId, SubModule submodule)
        {
            var opalClientUrl = _configuration.GetValue<string>("OpalClientUrl");
            return $"{opalClientUrl}form-standalone/standalonesurvey/{submodule}/{formId}";
        }
    }
}
