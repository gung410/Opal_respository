using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Backend.CrossCutting.HttpClientHelper;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace cxOrganization.WebServiceAPI.Test.IntegrationTests
{
    public class LearnerControllerTests
        : BaseControllerTests
    {
        public LearnerControllerTests(
            CustomWebApplicationFactory<cxOrganization.WebServiceAPI.Startup> factory) : base(factory)
        {
        }
        [Fact]
        public async Task ShouldGetLearnersSucess()
        {
            var message = HttpRequestMessageBuilder.GetHttpRequestBuilder(HttpMethod.Get, "/learners")
                                                   .WithBasicAuthen("dXNlcjpwYXNzd29yZA==")
                                                   .WithHeader("cxtoken", "15:1792")
                                                   .Build();
            // Arrange
            var defaultPage = await _client.SendAsync(message);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, defaultPage.StatusCode);
        }
    }
}