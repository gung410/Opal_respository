using System.Collections.Generic;
using cxOrganization.Domain.Extensions;

namespace cxOrganization.Domain.Security.HierarchyDepartment
{
    public class HierarchyDepartmentPermissionSettings
    {
        /// <summary>
        /// The root department identifier of the system.
        /// </summary>
        public int RootDepartmentId { get; set; }

        /// <summary>
        /// Set to True in order to exclude the root department from the response.
        /// </summary>
        public bool ExcludeTheRootDepartment { get; set; }

        /// <summary>
        /// The list of external identifiers of the user types of the currently logged-in user which has full access on the entire hierarchy departments.
        /// </summary>
        public List<string> FullAccessOnHierarchyDepartment { get; set; }

        /// <summary>
        /// The list of external identifiers of the user types of the currently logged-in user which has full access on the descendant departments.
        /// </summary>
        public List<string> FullAccessOnDescendentDepartmentUserTypeExtIds { get; set; }

        /// <summary>
        /// The list of external identifiers of the departments which be restricted if the current logged-in user doesn't have full access.
        /// </summary>
        public List<string> DenyDepartmentTypeExtIdsIfNotFullAccess { get; set; }

        public bool HasConfig()
        {
            return !FullAccessOnDescendentDepartmentUserTypeExtIds.IsNullOrEmpty() 
                   ||
                   !DenyDepartmentTypeExtIdsIfNotFullAccess.IsNullOrEmpty();
        }
    }
}
