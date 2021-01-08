using System;
using Microsoft.Extensions.Configuration;
using Thunder.Platform.Core.Application;

namespace Microservice.Form.Application.Services
{
    public class WebAppLinkBuilder : ApplicationService
    {
        private readonly IConfiguration _configuration;

        public WebAppLinkBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetFormDetailLink(Guid formId)
        {
            var opalClientUrl = _configuration.GetValue<string>("OpalClientUrl");
            return $"{opalClientUrl}ccpm/form/{formId}";
        }

        public string GetStandaloneFormPlayerLink(Guid formId)
        {
            var opalClientUrl = _configuration.GetValue<string>("OpalClientUrl");
            return $"{opalClientUrl}form-standalone/form/{formId}";
        }

        public string GetStandaloneFormPlayerLink()
        {
            var opalClientUrl = _configuration.GetValue<string>("OpalClientUrl");
            return $"{opalClientUrl}form-standalone-player";
        }
    }
}
