using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Conexus.Opal.Microservice.Tagging.Dtos
{
    public class SaveSearchTagRequest
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }
    }
}
