using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Service.Authentication;

namespace Microservice.Course.Domain.Entities
{
    public class CoursePlanningCycle : FullAuditedEntity, ISoftDelete
    {
        public string ExternalId { get; set; }

        public int YearCycle { get; set; }

        public string Title { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Description { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public bool IsConfirmedBlockoutDate { get; set; }

        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public virtual ICollection<BlockoutDate> BlockoutDates { get; set; } = new List<BlockoutDate>();

        public static bool HasVerificationPermission(Guid? userId, List<string> userRoles)
        {
            return userId == null || UserRoles.IsSysAdministrator(userRoles) || userRoles.Any(r => r == UserRoles.CoursePlanningCoordinator);
        }
    }
}
