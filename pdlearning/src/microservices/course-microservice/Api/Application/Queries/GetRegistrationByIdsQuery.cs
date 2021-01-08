using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetRegistrationByIdsQuery : BaseThunderQuery<List<RegistrationModel>>
    {
        public List<Guid> Ids { get; set; }
    }
}
