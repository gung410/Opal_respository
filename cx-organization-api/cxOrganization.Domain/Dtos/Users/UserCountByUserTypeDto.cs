namespace cxOrganization.Domain.Dtos.Users
{
    public class UserCountByUserTypeDto
    {
        public int UserTypeId { get; set; }
        public string UserTypeExtId { get; set; }
        public string UserTypeName { get; set; }
        public int Count { get; set; }
        public int ArchetypeId { get; set; }
    }
}
