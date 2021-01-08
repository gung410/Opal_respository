using System.ComponentModel.DataAnnotations;

namespace cxOrganization.Client.UserGroups
{
    public class TeamDto : UserGroupDtoBase
    {
        [Required]
        public int? ParentDepartmentId { get; set; }
        public override int? GetParentDepartmentId()
        {
            return ParentDepartmentId;
        }

        public override int? GetParentUserId()
        {
            return null;
        }
        public override void SetParentDepartmentId(int? parentDepartmentId)
        {
            ParentDepartmentId = parentDepartmentId;
        }

    }
}
