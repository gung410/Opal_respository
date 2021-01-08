using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.Enums;

namespace Microservice.Course.Application.Models
{
    public class DownloadECertificateRegistrationResultModel
    {
        public byte[] FileContent { get; set; }

        public string FileName { get; set; }

        public ReportGeneralOutputFormatType FileFormat { get; set; }
    }
}
