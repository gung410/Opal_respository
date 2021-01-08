using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class SaveCourseDepartmentRequestDto
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public Guid TypeOfOrganizationUnits { get; set; }

        public SaveCourseDepartmentRequestDtoHierarchyInfo HierarchyInfo { get; set; }
    }

    public class SaveCourseDepartmentRequestDtoHierarchyInfo
    {
        public int HierarchyId { get; set; }

        public int HdId { get; set; }

        public int ParentHdId { get; set; }

        public string Path { get; set; }
    }
}
