using System;
using Microservice.Webinar.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Webinar.Application.Queries
{
    public class GetUserByIdQuery : BaseThunderQuery<UserModel>
    {
        public Guid Id { get; set; }
    }
}
