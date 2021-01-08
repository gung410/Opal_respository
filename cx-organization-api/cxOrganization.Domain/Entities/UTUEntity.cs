using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Entities
{
    public class UTUEntity
    {
        public UserEntity User { get; set; }
        public UserTypeEntity UserType { get; set; }
        public int UserId { get; set; }
        public int UserTypeId { get; set; }
    }
}
