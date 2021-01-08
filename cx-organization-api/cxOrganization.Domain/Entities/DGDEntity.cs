using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Entities
{
    public class DGDEntity
    {
        public DepartmentGroupEntity DepartmentGroup { get; set; }
        public DepartmentEntity Department { get; set; }
        public int DepartmentGroupId { get; set; }
        public int DepartmentId { get; set; }
    }
}
