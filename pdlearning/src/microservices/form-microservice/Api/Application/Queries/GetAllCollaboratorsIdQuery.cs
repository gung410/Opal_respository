using System;
using System.Collections.Generic;
using Microservice.Form.Application.RequestDtos;
using Microservice.Form.Application.Services;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class GetAllCollaboratorsIdQuery : BaseThunderQuery<List<Guid>>
    {
        public GetAllAccessRightIdsRequest Request { get; set; }
    }
}
