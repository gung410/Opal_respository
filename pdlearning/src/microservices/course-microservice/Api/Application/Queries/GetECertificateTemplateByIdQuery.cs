using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetECertificateTemplateByIdQuery : BaseThunderQuery<ECertificateTemplateModel>
    {
        public Guid Id { get; set; }
    }
}
