using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Dtos.Users
{
    public class UserBasicInfo: ConexusBaseDto
    {
        public UserBasicInfo()
        {
            Identity = null;
            EntityStatus = null;
        }

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string AvatarUrl { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserCxId { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}