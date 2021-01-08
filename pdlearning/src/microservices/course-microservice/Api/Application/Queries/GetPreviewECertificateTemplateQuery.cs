using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Microservice.Course.Domain.ValueObjects;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetPreviewECertificateTemplateQuery : BaseThunderQuery<PreviewECertificateTemplateModel>
    {
        public Guid ECertificateLayoutId { get; set; }

        public IEnumerable<ECertificateTemplateParam> Params { get; set; }
    }
}
