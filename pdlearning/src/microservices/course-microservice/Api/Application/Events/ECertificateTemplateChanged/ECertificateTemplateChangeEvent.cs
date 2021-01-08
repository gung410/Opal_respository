using Microservice.Course.Domain.Entities;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Events
{
    public class ECertificateTemplateChangeEvent : BaseThunderEvent
    {
        public ECertificateTemplateChangeEvent(ECertificateTemplate eCertificateTemplate, ECertificateTemplateChangeType changeType)
        {
            ECertificateTemplate = eCertificateTemplate;
            ChangeType = changeType;
        }

        public ECertificateTemplate ECertificateTemplate { get; }

        public ECertificateTemplateChangeType ChangeType { get; }

        public override string GetRoutingKey()
        {
            return $"microservice.events.course.ecertificatetemplate.{ChangeType.ToString().ToLower()}";
        }
    }
}
