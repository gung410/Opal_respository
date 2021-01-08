using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class SAM_MemberRole
    {
        public int MemberRoleId { get; set; }

        public string Name { get; set; }

        public string ExtId { get; set; }

        public int? No { get; set; }

        public DateTime Created { get; set; }

        public int? EntityStatusId { get; set; }

        public int? EntityStatusReasonId { get; set; }

        public Guid MasterId { get; set; }
    }
}
