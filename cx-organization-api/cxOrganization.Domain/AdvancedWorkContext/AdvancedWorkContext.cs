using cxOrganization.Domain.DomainEnums;
using cxPlatform.Core;
using System.Collections.Generic;

namespace cxOrganization.Domain.AdvancedWorkContext
{
    public interface IAdvancedWorkContext : IWorkContext
    {
        public IEnumerable<string> CurrentUserPermissionKeys { get; set; }
        public string OriginalTokenString { get; }
        public bool HasPermission(ICollection<string> permissionKeys, LogicalOperator logicalOperator = LogicalOperator.OR);
        public bool HasPermission(string permissionKey);
        public bool isInternalRequest { get; }
    }
}