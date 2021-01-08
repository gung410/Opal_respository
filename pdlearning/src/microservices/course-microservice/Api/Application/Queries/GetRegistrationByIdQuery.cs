using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetRegistrationByIdQuery : BaseThunderQuery<RegistrationModel>
    {
        public Guid Id { get; set; }
    }
}
