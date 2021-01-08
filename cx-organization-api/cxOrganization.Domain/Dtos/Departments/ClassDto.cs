using System;
using System.Collections.Generic;

namespace cxOrganization.Client.Departments
{
    [Serializable]
    public class ClassDto : DepartmentDtoBase
    {
        public List<MemberDto> EmployeeMembers { get; set; }
    }
}
