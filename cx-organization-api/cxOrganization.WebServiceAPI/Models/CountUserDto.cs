using System;
using System.Collections.Generic;
using cxOrganization.Domain.DomainEnums;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.WebServiceAPI.Models
{
    public class CountUserDto
    {
        public List<int> UserIds { get; set; }
        public List<int> UserGroupIds { get; set; }
        public List<EntityStatusEnum> UserEntityStatuses { get; set; }
        public List<ArchetypeEnum> UserArchetypes { get; set; }
        public List<int> UserTypeIds { get; set; }
        public List<string> UserTypeExtIds { get; set; }
        public List<int> ParentDepartmentIds { get; set; }
        public List<string> ExtIds { get; set; }
        public List<string> JsonDynamicData { get; set; }
        public List<int> ExceptUserIds { get; set; }
        public List<List<int>> MultiUserTypeFilters { get; set; }
        public List<List<string>> MultiUserTypeExtIdFilters { get; set; }
        public List<List<int>> MultiUserGroupFilters { get; set; }
        public DateTime? LastUpdatedBefore { get; set; }
        public DateTime? LastUpdatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public UserGroupByField GroupByField { get; set; } = UserGroupByField.None;
    }
}