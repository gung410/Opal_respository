using System;
using System.Collections.Generic;
using Microservice.Content.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries
{
    public class GetListDigitalContentsByListIdQuery : BaseThunderQuery<List<DigitalContentModel>>
    {
        public List<Guid> ListIds { get; set; }
    }
}
