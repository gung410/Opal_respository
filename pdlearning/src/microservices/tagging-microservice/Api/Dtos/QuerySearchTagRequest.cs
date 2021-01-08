using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thunder.Platform.Core.Application.Dtos;

namespace Conexus.Opal.Microservice.Tagging.Dtos
{
    public class QuerySearchTagRequest
    {
        public string SearchText { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
