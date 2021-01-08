using cxOrganization.Client.UserGroups;

namespace cxOrganization.Domain.Dtos.UserGroups
{
    public class UserPoolDto : UserGroupDtoBase
    {
        /// <summary>
        /// The identifier of the department which the user pool belongs to.
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// The identifier of the user who might create the user pool.
        /// </summary>
        public int? UserId { get; set; }

        public override int? GetParentDepartmentId()
        {
            return DepartmentId;
        }

        public override int? GetParentUserId()
        {
            return UserId;
        }

        /// <summary>
        /// Count the number of members in the user pool.
        /// </summary>
        public int? MemberCount { get; set; }

        public override void SetParentDepartmentId(int? parentDepartmentId)
        {
            DepartmentId = parentDepartmentId;
        }
    }
}
