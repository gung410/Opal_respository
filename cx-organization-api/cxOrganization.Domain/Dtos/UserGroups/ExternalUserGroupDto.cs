using System.ComponentModel.DataAnnotations;

namespace cxOrganization.Client.UserGroups
{
    public class ExternalUserGroupDto : UserGroupDtoBase
    {
        [Required]
        public int? ParentDepartmentId { get; set; }
        public string Tag { get; set; }
        public int? UserId { get; set; }
        public override int? GetParentDepartmentId()
        {
            return ParentDepartmentId;
        }

        public override int? GetParentUserId()
        {
            return UserId;
        }

        public override void SetParentDepartmentId(int? parentDepartmentId)
        {
            ParentDepartmentId = parentDepartmentId;
        }
    }
}
