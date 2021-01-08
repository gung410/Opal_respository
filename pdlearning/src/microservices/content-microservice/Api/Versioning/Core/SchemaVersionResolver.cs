using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microservice.Content.Versioning.Application.Commands;
using Microservice.Content.Versioning.Entities;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Content.Versioning.Core
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
                throw new ArgumentException(string.Format("{0} was not marked with SchemaVersion attribute", typeof(TEntity)));
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
            RevertVersionCommand command,
            VersionTracking versionTrackingData)
        {
            if (this.IsValidSchema(versionTrackingData) && this.IsValidObjectType(versionTrackingData))
            {
                TModel data = DeserializeData(versionTrackingData);
                await DoLogicCheckoutVersion(command, versionTrackingData, data);
            }
        }

        public virtual Task DoLogicCheckoutVersion(
            RevertVersionCommand command,
            VersionTracking versionTrackingData,
            TModel contentModel)
        {
            return Task.CompletedTask;
        }

        public TModel DeserializeData(VersionTracking versionData)
        {
            return JsonSerializer.Deserialize<TModel>(versionData.Data);
        }

        public bool IsValidSchema(VersionTracking versionTrackingData)
        {
            return versionTrackingData.SchemaVersion == this.GetSchemaVersion();
        }

        public bool IsValidObjectType(VersionTracking versionTrackingData)
        {
            return versionTrackingData.ObjectType == this.GetObjectType();
        }

        public bool CanResolveSchemaType(VersionSchemaType versionSchematType)
        {
            return this.GetObjectType() == versionSchematType;
        }
    }
}
