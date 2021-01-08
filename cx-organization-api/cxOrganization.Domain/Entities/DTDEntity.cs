using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Entities
{
    public class DTDEntity
    {
        public DepartmentTypeEntity DepartmentType { get; set; }
        public int DepartmentTypeId { get; set; }
        public int DepartmentId { get; set; }
        public DepartmentEntity Department { get; set; }
    }
}
