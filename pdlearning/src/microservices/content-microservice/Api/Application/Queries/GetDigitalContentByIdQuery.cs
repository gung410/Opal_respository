using System;
using Microservice.Content.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Content.Application.Queries
{
    public class GetDigitalContentByIdQuery : BaseThunderQuery<DigitalContentModel>
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
    }
}
