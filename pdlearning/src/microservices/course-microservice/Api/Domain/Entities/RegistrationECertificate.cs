using System;
using System.Text.Json.Serialization;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Domain.Entities
{
    /// <summary>
    /// Store e-certificate for completed participant. This have one to one relationship to Registration. It's Id equal to Registration.Id.
    /// </summary>
    public class RegistrationECertificate : FullAuditedEntity
    {
        public Guid UserId { get; set; }

        public string PdfFileName { get; set; }

        public string Base64Pdf { get; set; }

        public string Base64Image { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string ImageFileName => PdfFileName.Replace("pdf", "jpeg");

        [JsonIgnore]
        public virtual Registration Participant { get; set; }

        public static RegistrationECertificate New(
            Registration registration,
            CourseUser registrationUser,
            CourseEntity registrationCourse,
            ECertificateLayout eCertificateLayout,
            string base64PdfCertificate,
            string base64ImageCerticiate)
        {
            return new RegistrationECertificate()
            {
                Id = registration.Id,
                PdfFileName = $"Certificate__{registrationCourse.CourseCode}__{registrationUser.FullName()}.pdf",
                Base64Image = base64ImageCerticiate,
                Base64Pdf = base64PdfCertificate,
                UserId = registration.UserId,
                Width = eCertificateLayout.Width,
                Height = eCertificateLayout.Height,
                CreatedDate = Clock.Now
            };
        }
    }
}
