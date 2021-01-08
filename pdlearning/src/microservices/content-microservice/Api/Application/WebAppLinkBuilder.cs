using System;
using Microsoft.Extensions.Configuration;

namespace Microservice.Content.Application
{
    public static class WebAppLinkBuilder
    {
        public static string GetDigitalContentDetailLink(IConfiguration configuration, Guid digitalContentId)
        {
            var opalClientUrl = configuration.GetValue<string>("OpalClientUrl");
            return $"{opalClientUrl}ccpm/content/{digitalContentId}";
        }
    }
}
