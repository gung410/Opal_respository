using Thunder.Platform.Core.Application;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Badge.Application.Services
{
    /// <summary>
    /// This service work as forwarder for each request in controller. It different with business logic.
    /// </summary>
    public abstract class BaseApplicationService : ApplicationService
    {
        protected BaseApplicationService(IThunderCqrs thunderCqrs)
        {
            ThunderCqrs = thunderCqrs;
        }

        protected IThunderCqrs ThunderCqrs { get; }

        protected PagedResultRequestDto InitPagingRequestDto<TPagingRequest>(TPagingRequest request) where TPagingRequest : PagedResultRequestDto
        {
            return new() { SkipCount = request.SkipCount, MaxResultCount = request.MaxResultCount };
        }
    }
}
