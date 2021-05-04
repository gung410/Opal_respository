
using cxOrganization.Client.UserGroups;
using cxOrganization.Client.UserTypes;
using cxOrganization.Domain.Dtos.UserGroups;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Dtos.Users
{
    public class UserGenericDto : UserDtoBase
    {
        public string DepartmentName { get; set; }
        public string DepartmentAddress { get; set; }

        /// <summary>
        /// The list of user group that user is member of
        /// </summary>
        public List<UserGroupDto> Groups { get; set; }

        /// <summary>
        /// The list of user group that user is owner of
        /// </summary>
        public List<UserGroupDto> OwnGroups { get; set; }
        public int DepartmentId { get; set; }
        public bool? IdpLocked { get; set; }
        public bool SkipGenerateUserName { get; set; }
        public override int GetParentDepartmentId()
        {
            return DepartmentId;
        }
    }
}
