using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;
using Microservice.Course.Domain.ValueObjects;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class SaveECertificateTemplateCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public Guid ECertificateLayoutId { get; set; }

        public List<ECertificateTemplateParam> Params { get; set; }

        public ECertificateTemplateStatus Status { get; set; }

        public bool IsCreate { get; set; }
    }
}
