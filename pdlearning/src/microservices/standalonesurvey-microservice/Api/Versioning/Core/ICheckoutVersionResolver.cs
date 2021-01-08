using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Versioning.Application.Commands;
using Microservice.StandaloneSurvey.Versioning.Entities;

namespace Microservice.StandaloneSurvey.Versioning.Core
{
    public interface ICheckoutVersionResolver
    {
        public bool CanResolveSchemaType(VersionSchemaType versionSchematType);

        public string GetSchemaVersion();

        public VersionSchemaType GetObjectType();

        public bool CanRevertToSchema(VersionTracking versionTrackingData);

        public bool CanApplyRevertForObjectType(VersionTracking versionTrackingData);

        public Task CheckoutVersion(RevertVersionCommand revertCommand, VersionTracking versionTrackingData);
    }
}
