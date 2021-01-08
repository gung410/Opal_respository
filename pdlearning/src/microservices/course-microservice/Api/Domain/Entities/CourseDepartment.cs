using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.AccessControl.Entities;

namespace Microservice.Course.Domain.Entities
{
    public class CourseDepartment : Department
    {
        public string Name { get; set; }
    }
}
