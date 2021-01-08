namespace cxOrganization.Domain.Dtos.Users
{
    public class LoginServiceIdentityDto 
    {
        /// <summary>The identifier of the DTO in database</summary>
        public long? Id { get; set; }

        /// The identifier of the DTO in external system
        public string ExtId { get; set; }
        /// <summary>
        /// The siteId which login service belong to. If it is has value, login service is identify in this siteId only.
        /// </summary>
        public int? SiteId { get; set; }

    }
}
