using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Course.Domain.Entities
{
    public class CommentViewPermission : AuditedEntity
    {
        public string CommentAction { get; set; }

        public string CommentByUserRole { get; set; }

        public string CanViewRole { get; set; }
    }
}
