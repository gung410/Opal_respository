using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class DeleteECertificateTemplateCommand : BaseThunderCommand
    {
        public Guid ECertificateTemplateId { get; set; }
    }
}
