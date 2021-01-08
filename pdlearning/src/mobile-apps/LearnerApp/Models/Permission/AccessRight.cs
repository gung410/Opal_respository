using LearnerApp.Common.Enum;

namespace LearnerApp.Models.Permission
{
    public class AccessRight
    {
        public long Id { get; set; }

        public string Action { get; set; }

        public ObjectType ObjectType { get; set; }

        public Module Module { get; set; }

        public GrantedType GrantedType { get; set; }

        public long ParentId { get; set; }

        public long No { get; set; }
    }
}
