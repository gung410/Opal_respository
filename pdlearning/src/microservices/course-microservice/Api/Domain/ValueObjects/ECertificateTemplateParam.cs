using Microservice.Course.Application.Enums;

namespace Microservice.Course.Domain.ValueObjects
{
    public class ECertificateTemplateParam : BaseValueObject
    {
        public ECertificateTemplateParam()
        {
        }

        public ECertificateTemplateParam(ECertificateSupportedField key, string value)
        {
            Key = key;
            Value = value;
        }

        public ECertificateSupportedField Key { get; set; }

        public string Value { get; set; }
    }
}
