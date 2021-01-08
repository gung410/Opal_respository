using System.Threading.Tasks;
using Microservice.Form.Versioning.Application.Commands;
using Microservice.Form.Versioning.Entities;

namespace Microservice.Form.Versioning.Core
{
    public interface ICheckoutVersionResolver
    {
        public bool CanResolveSchemaType(VersionSchemaType versionSchematType);

        public string GetSchemaVersion();

        public VersionSchemaType GetObjectType();

        public bool IsValidSchema(VersionTracking versionTrackingData);

        public bool IsValidObjectType(VersionTracking versionTrackingData);

        public Task CheckoutVersion(RevertVersionCommand revertCommand, VersionTracking versionTrackingData);
    }
}
