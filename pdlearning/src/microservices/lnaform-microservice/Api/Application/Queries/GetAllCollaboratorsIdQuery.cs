using System;
using System.Collections.Generic;
using Microservice.LnaForm.Application.RequestDtos;
using Microservice.LnaForm.Application.Services;
using Thunder.Platform.Cqrs;

namespace Microservice.LnaForm.Application.Queries
{
    public class GetAllCollaboratorsIdQuery : BaseThunderQuery<List<Guid>>
    {
        public GetAllAccessRightIdsRequest Request { get; set; }
    }
}
