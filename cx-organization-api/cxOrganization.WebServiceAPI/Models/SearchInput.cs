using cxOrganization.Domain.Enums;
using cxPlatform.Client.ConexusBase;
using System;
using System.Collections.Generic;

namespace cxOrganization.WebServiceAPI.Models
{
    public class SearchInput
    {
        public List<int> ParentDepartmentId { get; set; }
        public List<int> UserIds { get; set; }
        public List<ArchetypeEnum> UserArchetypes { get; set; }
        public List<string> ExtIds { get; set; }
        public List<string> LoginServiceClaims { get; set; }
        public List<string> SystemRolePermissions { get; set; }
        public List<string> Emails { get; set; }
        public List<cxPlatform.Client.ConexusBase.EntityStatusEnum> UserEntityStatuses { get; set; }
        public bool GetRoles { get; set; }
        public bool GetDepartment { get; set; }
        public bool GetGroups { get; set; }
        public bool GetOwnGroups { get; set; }
        public bool FilterOnParentHd { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
        public string SearchKey { get; set; }
        public List<string> JsonDynamicData { get; set; }
        public bool? FilterOnSubDepartment { get; set; }
        public List<string> DepartmentExtIds { get; set; }
        public List<int> UserGroupIds { get; set; }
        public List<List<int>> MultiUserGroupFilters { get; set; }
        public List<int> UserTypeIds {get;set;}
        public List<List<int>> MultiUserTypeFilters { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public DateTime? ExpirationDateAfter { get; set; }
        public DateTime? ExpirationDateBefore { get; set; }
        public List<int> OrgUnitTypeIds { get; set; }
        public List<AgeRange> AgeRanges { get; set; }
        public bool? ExternallyMastered { get; set; }
        public bool? GetLoginServiceClaims { get; set; }
        public List<List<string>> MultiUserTypeExtIdFilters { get; set; }
        public List<string> SsnList { get; set; }
        public List<string> UserTypeExtIds { get; set; }
        public DateTime? ActiveDateBefore { get; set; }
        public DateTime? ActiveDateAfter { get; set; }
        public List<int> ExceptUserIds { get; set; }
    }
}

