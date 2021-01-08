using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_EntityStatuses
    {
        public SAM_EntityStatuses()
        {
            SamDepartments = new HashSet<SAM_Department>();
            SamEntityStatusReasons = new HashSet<SAM_EntityStatusReasons>();
        }

        public int EntityStatusId { get; set; }

        public string CodeName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<SAM_Department> SamDepartments { get; set; }

        public virtual ICollection<SAM_EntityStatusReasons> SamEntityStatusReasons { get; set; }
    }
}
