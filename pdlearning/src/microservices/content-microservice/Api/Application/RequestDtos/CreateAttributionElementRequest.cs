using Microservice.Content.Domain.Enums;

namespace Microservice.Content.Application.RequestDtos
{
    public class CreateAttributionElementRequest
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public string Source { get; set; }

        public LicenseType LicenseType { get; set; }
    }
}
