namespace Microservice.Analytics.Domain.Entities
{
    public class CSL_GroupPermission
    {
        public string PermissionId { get; set; }

        public int GroupId { get; set; }

        public string ModuleId { get; set; }

        public string Class { get; set; }

        public byte? State { get; set; }

        public virtual CSL_Group Group { get; set; }
    }
}
