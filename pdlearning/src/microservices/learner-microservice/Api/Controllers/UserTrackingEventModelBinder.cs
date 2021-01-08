using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microservice.Learner.Application.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Thunder.Platform.Core.Context;

namespace Microservice.Learner.Controllers
{
    public class UserTrackingEventModelBinder : IModelBinder
    {
        private readonly IUserContext _userContext;

        public UserTrackingEventModelBinder(IUserContext userContext)
        {
            _userContext = userContext;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // use the http request object to make decisions about how to bind to the model
            var request = bindingContext?.ActionContext?.HttpContext?.Request;

            using StreamReader reader
                = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);

            var bodyStr = await reader.ReadToEndAsync();

            var jDoc = JsonDocument.Parse(bodyStr);

            var eventName = jDoc.RootElement.GetProperty("eventName").GetString();
            var time = jDoc.RootElement.GetProperty("time").GetDateTime();
            var sessionId = Guid.Parse(jDoc.RootElement.GetProperty("sessionId").GetString());
            var sourceIp = GetFirstIp(_userContext.GetValue<string>(CommonUserContextKeys.OriginIp));

            // mark the binding context result to insert into the actual value
            bindingContext.Result = ModelBindingResult.Success(new UserTrackingEventRequest
            {
                EventName = eventName,
                Time = time,
                SessionId = sessionId,
                SourceIp = sourceIp,
                UserTrackingEventAsJson = bodyStr
            });
        }

        private string GetFirstIp(string ips)
        {
            // Ips="27.65.252.184, 27.65.252.184,.."
            return ips == null ? string.Empty : ips.Split(',')[0];
        }
    }
}
