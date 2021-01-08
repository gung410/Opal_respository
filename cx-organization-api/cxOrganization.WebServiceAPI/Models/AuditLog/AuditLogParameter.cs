using System.ComponentModel.DataAnnotations;

namespace cxOrganization.WebServiceAPI.Models.AuditLog
{
    public class AuditLogParameter
    {
        [Required]
        public string UserExtId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
