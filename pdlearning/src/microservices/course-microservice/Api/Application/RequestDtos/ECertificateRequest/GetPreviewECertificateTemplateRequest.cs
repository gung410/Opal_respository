using System;
using System.Collections.Generic;
using Microservice.Course.Domain.ValueObjects;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetPreviewECertificateTemplateRequest
    {
        public Guid ECertificateLayoutId { get; set; }

        public IEnumerable<ECertificateTemplateParam> Params { get; set; }
    }
}
