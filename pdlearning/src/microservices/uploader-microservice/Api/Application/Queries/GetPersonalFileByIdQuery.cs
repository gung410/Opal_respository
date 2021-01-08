using System;
using Microservice.Uploader.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Uploader.Application.Queries
{
    public class GetPersonalFileByIdQuery : BaseThunderQuery<PersonalFileModel>
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
    }
}
