using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_DepartmentTypes
    {
        public SAM_DepartmentTypes()
        {
            SamDepartmentDepartmentType = new HashSet<SAM_DepartmentDepartmentType>();
        }

        public string DepartmentTypeId { get; set; }

        public string Name { get; set; }

        public string ExtId { get; set; }

        public string Code { get; set; }

        public short No { get; set; }

        public DateTime Created { get; set; }

        public string ArchetypeId { get; set; }

        public string ParentId { get; set; }

        public string MasterId { get; set; }

        public int? ExtDepartmentTypeId { get; set; }

        public virtual ICollection<SAM_DepartmentDepartmentType> SamDepartmentDepartmentType { get; set; }
    }
}
