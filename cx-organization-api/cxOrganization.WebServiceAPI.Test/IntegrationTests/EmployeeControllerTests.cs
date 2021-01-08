using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Backend.CrossCutting.HttpClientHelper;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.WebServiceAPI.Test.Helpers;
using Xunit;

namespace cxOrganization.WebServiceAPI.Test.IntegrationTests
{
    public class EmployeeControllerTests : BaseControllerTests
    {
        public EmployeeControllerTests(
            CustomWebApplicationFactory<cxOrganization.WebServiceAPI.Startup> factory) : base(factory)
        {
        }
        [Fact]
        public async Task ShouldGetEmployeeSucess()
        {
            var message = HttpRequestMessageBuilder.GetHttpRequestBuilder(HttpMethod.Get, "/employees")
                                                   .WithBasicAuthen("dXNlcjpwYXNzd29yZA==")
                                                   .WithHeader("cxtoken", "3001:2052")
                                                   .Build();
            // Arrange
            var defaultPage = await _client.SendAsync(message);

            // Assert
            Assert.Equal(HttpStatusCode.OK, defaultPage.StatusCode);
        }

        [Fact]
        public async Task ShouldUpdateEmployeeSucess()
        {
            var message = HttpRequestMessageBuilder.GetHttpRequestBuilder(HttpMethod.Get, "/employees?pageSize=1")
                                                   .WithBasicAuthen("dXNlcjpwYXNzd29yZA==")
                                                   .WithHeader("cxtoken", "3001:2052")
                                                   .Build();
            // Arrange
            var responseMessage = await _client.SendAsync(message);
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);

            var resultResponse = await responseMessage.HandleReponseAsync<List<EmployeeDto>>();
            Assert.True(resultResponse.Count > 0);

            var result = resultResponse.FirstOrDefault();

            //Update employee
            result.FirstName = StringRandomer.RandomString(5);
            result.LastName = StringRandomer.RandomString(5);

            var updateMessage = HttpRequestMessageBuilder.GetHttpRequestBuilder(HttpMethod.Put, $"/employees/{result.Identity.Id}")
                                                   .WithBasicAuthen("dXNlcjpwYXNzd29yZA==")
                                                   .WithHeader("cxtoken", "3001:2052")
                                                   .WithContent(new JsonContent(result))
                                                   .Build();
            // Arrange
            var updateResponseMessage = await _client.SendAsync(updateMessage);
            Assert.Equal(HttpStatusCode.OK, updateResponseMessage.StatusCode);
            var updateResult = await updateResponseMessage.HandleReponseAsync<EmployeeDto>();
            Assert.Equal(result.FirstName, updateResult.FirstName);
            Assert.Equal(result.LastName, updateResult.LastName);
        }
    }
}
