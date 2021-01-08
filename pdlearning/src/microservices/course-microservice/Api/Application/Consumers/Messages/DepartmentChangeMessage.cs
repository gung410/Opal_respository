#pragma warning disable SA1402 // File may only contain a single type
using System;

namespace Microservice.Course.Application.Consumers
{
    public class DepartmentChangeMessage
    {
        public int DepartmentId { get; set; }

        public DepartmentInfo DepartmentData { get; set; }

        public HierarchyInfo HierarchyInfo { get; set; }
    }

    public class DepartmentInfo
    {
        public JsonDynamic JsonDynamicAttributes { get; set; }

        public string Name { get; set; }
    }

    public class JsonDynamic
    {
        public Guid TypeOfOrganizationUnits { get; set; }
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
