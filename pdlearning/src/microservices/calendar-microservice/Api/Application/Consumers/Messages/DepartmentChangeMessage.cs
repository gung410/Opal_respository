#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Calendar.Application.Consumers
{
    public class DepartmentChangeMessage
    {
        public int DepartmentId { get; set; }

        public HierarchyInfo HierarchyInfo { get; set; }
    }

    public class HierarchyInfo
    {
        public int HierarchyId { get; set; }

        public int HdId { get; set; }

        public int ParentHdId { get; set; }

        public string Path { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
