using System;
using System.Collections.Generic;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_UserTypes
    {
        public SAM_UserTypes()
        {
            SamUserUserTypes = new HashSet<SAM_UserUserTypes>();
        }

        public string UserTypeId { get; set; }

        public string ExtId { get; set; }

        public string Name { get; set; }

        public short No { get; set; }

        public DateTime Created { get; set; }

        public string ArchetypeId { get; set; }

        public string ParentId { get; set; }

        public string MasterId { get; set; }

        public virtual ICollection<SAM_UserUserTypes> SamUserUserTypes { get; set; }
    }
}
