using cxPlatform.Client.ConexusBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Dtos.DepartmentType
{
    public class DepartmentTypeDto: ConexusBaseDto
    {
        public List<LocalizedDataDto> LocalizedData { get; set; }
    }
}
