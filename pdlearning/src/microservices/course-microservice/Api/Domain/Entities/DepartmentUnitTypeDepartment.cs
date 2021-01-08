using System;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Course.Domain.Entities
{
    public class DepartmentUnitTypeDepartment : FullAuditedEntity, ISoftDelete
    {
        public int DepartmentId { get; set; }

        public Guid DepartmentUnitTypeId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
