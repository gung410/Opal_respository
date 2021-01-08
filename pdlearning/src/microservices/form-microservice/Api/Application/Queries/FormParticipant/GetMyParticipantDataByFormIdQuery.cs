using System;
using Microservice.Form.Application.Models;
using Thunder.Platform.Core.Application.Dtos;
using Thunder.Platform.Cqrs;

namespace Microservice.Form.Application.Queries
{
    public class GetMyParticipantDataByFormIdQuery : BaseThunderQuery<FormParticipantModel>
    {
        public Guid FormOriginalObjectId { get; set; }

        public PagedResultRequestDto PagedInfo { get; set; }
    }
}
