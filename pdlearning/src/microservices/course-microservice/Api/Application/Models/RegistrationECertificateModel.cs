using System;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.Models
{
    public class RegistrationECertificateModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string PdfFileName { get; set; }

        public string Base64Pdf { get; set; }

        public string Base64Image { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public static RegistrationECertificateModel Create(RegistrationECertificate registrationECertificate)
        {
            return new RegistrationECertificateModel
            {
                Id = registrationECertificate.Id,
                UserId = registrationECertificate.UserId,
                PdfFileName = registrationECertificate.PdfFileName,
                Base64Image = registrationECertificate.Base64Image,
                Base64Pdf = registrationECertificate.Base64Pdf,
                Width = registrationECertificate.Width,
                Height = registrationECertificate.Height
            };
        }
    }
}
