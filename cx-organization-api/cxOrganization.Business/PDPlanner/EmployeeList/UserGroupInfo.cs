using cxOrganization.Client.UserGroups;

namespace cxOrganization.Business.PDPlanner.EmployeeList
{
    public class UserGroupInfo : ObjectBasicInfo
    {
        public int? DepartmentId { get; set; }
        public int? UserId { get; set; }
        public GrouptypeEnum Type { get; set; }

    }
}
