using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Common
{
    public class UserEntityActiveDateDto
    {
        public string Path { get; set; }
        public string[] DepartmentType { get; set; }
        public DateTime EntityActiveDate { get; set; }
    }
}
