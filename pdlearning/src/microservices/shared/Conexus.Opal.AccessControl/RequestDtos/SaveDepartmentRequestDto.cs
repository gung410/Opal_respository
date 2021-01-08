using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable SA1402 // File may only contain a single type
namespace Conexus.Opal.AccessControl.RequestDtos
{
    public class SaveDepartmentRequestDto
    {
        public int DepartmentId { get; set; }

        public Guid TypeOfOrganizationUnits { get; set; }

        public SaveDepartmentRequestDtoHierarchyInfo HierarchyInfo { get; set; }
    }

    public class SaveDepartmentRequestDtoHierarchyInfo
    {
        public int HierarchyId { get; set; }

        public int HdId { get; set; }

        public int ParentHdId { get; set; }

        public string Path { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
