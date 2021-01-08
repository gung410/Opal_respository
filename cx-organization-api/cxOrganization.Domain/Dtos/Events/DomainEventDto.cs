using cxPlatform.Client.ConexusBase;

namespace cxEvent.Client
{
    public class DomainEventDto : EventDtoBase
    {
        public DomainEventDto()
        {
            UserIdentity = new IdentityBaseDto();
            DepartmentIdentity = new IdentityBaseDto();
            ObjectIdentity = new IdentityBaseDto();
        }
    }
}
