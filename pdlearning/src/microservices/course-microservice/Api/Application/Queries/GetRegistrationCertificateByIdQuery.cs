using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetRegistrationCertificateByIdQuery : BaseThunderQuery<RegistrationECertificateModel>
    {
        public Guid RegistrationId { get; set; }
    }
}
