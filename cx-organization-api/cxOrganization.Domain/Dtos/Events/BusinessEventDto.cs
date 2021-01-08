using cxPlatform.Client.ConexusBase;

namespace cxEvent.Client
{
    public class BusinessEventDto : EventDtoBase
    {
        public BusinessEventDto()
        {
            ObjectIdentity = new IdentityBaseDto();
            UserIdentity = new IdentityBaseDto();
            DepartmentIdentity = new IdentityBaseDto();
        }
        /// <summary>
        /// If true, AdditionalInformation will store to UserData as XML format
        /// else will use Json Format
        /// </summary>
        /// <value>The correlation identifier.</value>
        public bool StoreAsXmlFortmat { get; set; }
        public short? TableTypeId { get; set; }
        public int? ItemId { get; set; }
    }
}
