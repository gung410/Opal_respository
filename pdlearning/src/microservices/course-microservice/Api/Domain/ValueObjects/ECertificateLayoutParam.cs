using Microservice.Course.Application.Enums;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Domain.ValueObjects
{
    public class ECertificateLayoutParam : BaseValueObject
    {
        public ECertificateSupportedField Key { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public ECertificateParamType Type { get; set; }

        public bool IsAutoPopulated { get; set; }
    }
}
