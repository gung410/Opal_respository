using System;

namespace Microservice.Analytics.Domain.Entities
{
    public partial class CSL_User
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public string Username { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public int? ContentcontainerId { get; set; }
    }
}
