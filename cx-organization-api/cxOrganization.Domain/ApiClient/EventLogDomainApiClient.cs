using Backend.CrossCutting.HttpClientHelper;
using cxEvent.Client;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain.ApiClient
{
    public class EventLogDomainApiClient : IEventLogDomainApiClient
    {
        private readonly EventDomainLogSettings _eventDomainLogSettings;
        private readonly IHttpRequestSender _httpRequestSender;
        private readonly ILogger _logger;
        private const string DomainEventEndpoint = "domainevents";
        private const string BussinessEventEndpoint = "bussinessevents";
        public EventLogDomainApiClient(IOptions<EventDomainLogSettings> eventDomainLogSettingsOption,
            IHttpRequestSender httpRequestSender,
            ILoggerFactory loggerFactory)
        {
            _eventDomainLogSettings = eventDomainLogSettingsOption.Value;
            _httpRequestSender = httpRequestSender;
            _logger = loggerFactory.CreateLogger<EventLogDomainApiClient>();
        }

        public void WriteDomainEvent(DomainEventDto domainEventDto, IRequestContext requestContext)
        {
            if (_eventDomainLogSettings.Enable)
            {
                var requestUri = _eventDomainLogSettings.EventAPIBaseUrl + DomainEventEndpoint;
                _httpRequestSender.Post(requestContext,
                    requestUri: requestUri,
                    body: domainEventDto,
                    authToken: _eventDomainLogSettings.EventAPIAuthorization).HandleReponse(_logger);
            }         
        }
        public void WriteBusinessEvent(BusinessEventDto businessEventDto, IRequestContext requestContext)
        {
            if (_eventDomainLogSettings.Enable)
            {
                var requestUri = _eventDomainLogSettings.EventAPIBaseUrl + BussinessEventEndpoint;
                _httpRequestSender.Post(requestContext,
                    requestUri: requestUri,
                    body: businessEventDto,
                    authToken: _eventDomainLogSettings.EventAPIAuthorization).HandleReponse(_logger);
            }
        }
    }
}
