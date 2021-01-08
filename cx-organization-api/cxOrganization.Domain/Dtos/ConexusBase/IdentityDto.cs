namespace cxPlatform.Client.ConexusBase
{
    public class IdentityDto : IdentityBaseDto
    {
        /// The identifier of the DTO in external system
        public string ExtId { get; set; }
        /// <summary>
        /// The identifier of the owner who that the DTO belong to
        /// </summary>
        public int OwnerId { get; set; }
        /// <summary>
        /// The identifier of the customer that the DTO belong to
        /// </summary>
        public int CustomerId { get; set; }
    }
}
