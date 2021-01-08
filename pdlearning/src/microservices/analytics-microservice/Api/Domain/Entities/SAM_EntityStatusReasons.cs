using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_EntityStatusReasons
    {
        public SAM_EntityStatusReasons()
        {
            SamDepartments = new HashSet<SAM_Department>();
        }

        public int EntityStatusReasonId { get; set; }

        public int EntityStatusId { get; set; }

        public string CodeName { get; set; }

        public short TableTypeId { get; set; }

        public int ArchetypeId { get; set; }

        public string MasterId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual SAM_EntityStatuses EntityStatus { get; set; }

        public virtual ICollection<SAM_Department> SamDepartments { get; set; }
    }
}
