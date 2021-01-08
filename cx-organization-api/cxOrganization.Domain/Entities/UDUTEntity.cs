using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Entities
{
    public class UDUTEntity
    {
        public UserDepartmentEntity UserDepartment { get; set; }
        public UserTypeEntity UserType { get; set; }
        public int U_DId { get; set; }
        public int UsertypeId { get; set; }
    }
}
