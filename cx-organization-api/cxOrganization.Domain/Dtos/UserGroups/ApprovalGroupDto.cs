using cxOrganization.Client.UserGroups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace cxOrganization.Domain.Dtos.UserGroups
{
    public class ApprovalGroupDto : UserGroupDtoBase
    {
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        [Required]
        public int? ApproverId { get; set; }
        public string Approver { get; set; }
        public string EmailAddress { get; set; }
        public string AvatarUrl { get; set; }
        public override int? GetParentDepartmentId()
        {
            return DepartmentId;
        }

        public override void SetParentDepartmentId(int? parentDepartmentId)
        {
            DepartmentId = parentDepartmentId;
        }

        public override int? GetParentUserId()
        {
            return ApproverId;
        }
    }
}
