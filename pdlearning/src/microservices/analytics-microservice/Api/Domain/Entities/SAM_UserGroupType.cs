using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_UserGroupType
    {
        public int UserGroupTypeId { get; set; }

        public string Name { get; set; }

        public string ExtId { get; set; }

        public short No { get; set; }

        public DateTime Created { get; set; }

        public Guid MasterId { get; set; }
    }
}
