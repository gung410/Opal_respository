using cxPlatform.Client.ConexusBase;
using System.Collections.Generic;

namespace cxOrganization.Client.DepartmentTypes
{
    public class DepartmentTypeDto : ConexusBaseDto
    {
        /// <summary>
        /// The localized data text
        /// </summary>
        public List<LocalizedDataDto> LocalizedData { get; set; }
    }
}
