using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microservice.StandaloneSurvey.Versioning.Application.Commands;
using Microservice.StandaloneSurvey.Versioning.Entities;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.StandaloneSurvey.Versioning.Core
{
    public class SchemaVersionResolver<TModel, TEntity> : ICheckoutVersionResolver
        where TModel : class
        where TEntity : IEntity
    {
        public SchemaVersionResolver()
        {
            SchemaVersionAttribute[] schemaVersionAttributes = (SchemaVersionAttribute[])Attribute.GetCustomAttributes(typeof(TEntity), typeof(SchemaVersionAttribute));

            // TODO: handle exception if attr is null
            if (schemaVersionAttributes.Length == 0)
            {
                throw new ArgumentException($"{typeof(TEntity)} was not marked with SchemaVersion attribute");
            }

            VersionAttr = schemaVersionAttributes.First();
        }

        private SchemaVersionAttribute VersionAttr { get; set; }

        public VersionSchemaType GetObjectType()
        {
            return VersionAttr.ObjectType;
        }

        public string GetSchemaVersion()
        {
            return VersionAttr.Version;
        }

        public async Task CheckoutVersion(
            RevertVersionCommand revertCommand,
            VersionTracking versionTrackingData)
        {
            if (this.CanRevertToSchema(versionTrackingData) && this.CanApplyRevertForObjectType(versionTrackingData))
            {
                TModel data = DeserializeData(versionTrackingData);
                await DoLogicCheckoutVersion(revertCommand, versionTrackingData, data);
            }
        }

        public virtual Task DoLogicCheckoutVersion(
            RevertVersionCommand revertVersionCommand,
            VersionTracking versionTrackingData,
            TModel dataVersion)
        {
            return Task.CompletedTask;
        }

        public TModel DeserializeData(VersionTracking versionData)
        {
            return JsonSerializer.Deserialize<TModel>(versionData.Data);
        }

        public bool CanRevertToSchema(VersionTracking versionTrackingData)
        {
            return versionTrackingData.SchemaVersion == this.GetSchemaVersion();
        }

        public bool CanApplyRevertForObjectType(VersionTracking versionTrackingData)
        {
            return versionTrackingData.ObjectType == this.GetObjectType();
        }

        public bool CanResolveSchemaType(VersionSchemaType versionSchematType)
        {
            return this.GetObjectType() == versionSchematType;
        }
    }
}
