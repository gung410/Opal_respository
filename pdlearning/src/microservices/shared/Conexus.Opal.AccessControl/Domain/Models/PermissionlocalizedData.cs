using System.Collections.Generic;

namespace Conexus.Opal.AccessControl.Domain.Models
{
    public class PermissionlocalizedData
    {
        public int Id { get; set; }

        public string LanguageCode { get; set; }

        public IEnumerable<PermissionFields> Fields { get; set; }
    }
}
