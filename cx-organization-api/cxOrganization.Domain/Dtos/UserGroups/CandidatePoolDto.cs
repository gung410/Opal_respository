using System;
using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Client.UserGroups
{
    public class CandidatePoolDto : UserGroupDtoBase
    {
        /// <summary>
        /// The id of department which candidate pool belong to
        /// </summary>
        public int? ParentDepartmentId { get; set; }

        /// <summary>
        /// The id of user which candidate pool belong to
        /// </summary>
        public int? ParentUserId { get; set; }
        
        public override int? GetParentDepartmentId()
        {
            return ParentDepartmentId;
        }

        public override int? GetParentUserId()
        {
            return ParentUserId;
        }
        public override void SetParentDepartmentId(int? parentDepartmentId)
        {
            ParentDepartmentId = parentDepartmentId;
        }
    }
}
