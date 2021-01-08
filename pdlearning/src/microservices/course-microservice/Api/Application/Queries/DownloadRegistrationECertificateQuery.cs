using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.Enums;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class DownloadRegistrationECertificateQuery : BaseThunderQuery<DownloadECertificateRegistrationResultModel>
    {
        public Guid RegistrationId { get; set; }

        public ReportGeneralOutputFormatType FileFormat { get; set; }
    }
}
