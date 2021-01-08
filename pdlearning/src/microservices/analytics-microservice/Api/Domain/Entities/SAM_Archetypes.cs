using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_Archetypes
    {
        public SAM_Archetypes()
        {
            SamDepartments = new HashSet<SAM_Department>();
        }

        public string ArcheTypeId { get; set; }

        public string CodeName { get; set; }

        public string TableTypeId { get; set; }

        public string MasterId { get; set; }

        public int? ExtId { get; set; }

        public virtual ICollection<SAM_Department> SamDepartments { get; set; }
    }
}
