using System.Threading.Tasks;
using Microservice.Content.Versioning.Application.Commands;
using Microservice.Content.Versioning.Entities;

namespace Microservice.Content.Versioning.Core
{
    public interface ICheckoutVersionResolver
    {
        public bool CanResolveSchemaType(VersionSchemaType versionSchematType);

        public string GetSchemaVersion();

        public VersionSchemaType GetObjectType();

        public bool IsValidSchema(VersionTracking versionTrackingData);

        public bool IsValidObjectType(VersionTracking versionTrackingData);

        public Task CheckoutVersion(RevertVersionCommand command, VersionTracking versionTrackingData);
    }
}
