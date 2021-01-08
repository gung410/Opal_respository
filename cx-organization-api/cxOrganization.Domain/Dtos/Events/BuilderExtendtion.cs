using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxEvent.Client
{
    /// <summary>
    /// Builder class helper
    /// </summary>
    public static class BuilderHelper
    {
        /// <summary>
        ///  get information from IrequestContext and map to Dto
        /// </summary>
        /// <param name="eventDto"></param>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        public static EventDtoBase MapInformationToDto( EventDtoBase eventDto, IRequestContext requestContext)
        {
            eventDto.CorrelationId = requestContext.CorrelationId;
            eventDto.CreatedBy = requestContext.CurrentUserId;
            eventDto.Identity.OwnerId = requestContext.CurrentOwnerId;
            eventDto.Identity.CustomerId = requestContext.CurrentCustomerId;
            eventDto.ApplicationName = requestContext.ApplicationName;
            return eventDto;
        }
    }
}
