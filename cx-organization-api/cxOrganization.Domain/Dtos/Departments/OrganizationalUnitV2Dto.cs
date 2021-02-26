using cxPlatform.Client.ConexusBase;
using System;
using System.Collections.Generic;

namespace cxOrganization.Client.Departments
{
    /// <summary>
    /// This is the V2 version of OrganizationUnitDto to support for get organizationUnitDtos
    /// by GetOrganizationalUnitsV2 using POST method.
    /// </summary>
    [Serializable]
    public class OrganizationalUnitDtoV2
    {
        public int ParentDepartmentId { get; set; } = 0;

        public string SearchText { get; set; } = string.Empty;

        public List<int> ParentDepartmentIds { get; set; }

        public string ParentDepartmentExtId { get; set; }

        public List<int> UserIds { get; set; }

        public List<int> OrganizationalUnitIds { get; set; }

        public List<string> OrganizationalUnitExtIds { get; set; }

        public List<EntityStatusEnum> StatusEnums { get; set; }

        public List<string> DepartmentTypeExtIds { get; set; }

        public bool IsByPassFilter { get; set; } = false;

        public DateTime? LastUpdatedBefore { get; set; }

        public DateTime? LastUpdatedAfter { get; set; }

        public bool SelectIdentity { get; set; } = false;

        public bool? ExternallyMastered { get; set; }

        public int PageIndex { get; set; } = 0;

        public int PageSize { get; set; } = 0;

        public string OrderBy { get; set; } = string.Empty;
    }
}
