using System;
using Microservice.Uploader.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Uploader.Application.Queries
{
    public class GetPersonalSpaceByUserIdQuery : BaseThunderQuery<PersonalSpaceModel>
    {
        public Guid UserId { get; set; }
    }
}
