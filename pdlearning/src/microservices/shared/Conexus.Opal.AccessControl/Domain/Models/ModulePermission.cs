using System.Collections.Generic;
using Conexus.Opal.AccessControl.Domain.Constants;

namespace Conexus.Opal.AccessControl.Domain.Models
{
    public class ModulePermission
    {
        public int Id { get; set; }

        public string Action { get; set; }

        public PermissionObjectType ObjectType { get; set; }

        public PermissionModuleType Module { get; set; }

        public PermissionGrantedType GrantedType { get; set; }

        public int ParentId { get; set; }

        public int No { get; set; }

        public IEnumerable<PermissionlocalizedData> LocalizedData { get; set; }
    }
}
