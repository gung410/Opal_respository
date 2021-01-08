using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace cxOrganization.WebServiceAPI.Test
{
    public class BaseControllerTests : IClassFixture<CustomWebApplicationFactory<cxOrganization.WebServiceAPI.Startup>>
    {
        protected readonly HttpClient _client;
        protected readonly CustomWebApplicationFactory<cxOrganization.WebServiceAPI.Startup>
            _factory;

        public BaseControllerTests(
            CustomWebApplicationFactory<cxOrganization.WebServiceAPI.Startup> factory)
        {
            _factory = factory;
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");
            _client = factory.WithWebHostBuilder(
                builder =>
                {
                    builder.ConfigureAppConfiguration((context, conf) =>
                    {
                        conf.AddJsonFile(configPath);
                    });

                }).CreateClient(new WebApplicationFactoryClientOptions());
        }
    }
}
