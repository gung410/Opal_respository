using cxEvent.Client;
using cxPlatform.Core;

namespace cxOrganization.Domain.ApiClient
{
    public interface IEventLogDomainApiClient
    {
        void WriteDomainEvent(DomainEventDto domainEventDto, IRequestContext requestContext);
        void WriteBusinessEvent(BusinessEventDto businessEventDto, IRequestContext requestContext);
    }
}
