using System;
using Microservice.StandaloneSurvey.Versioning.Entities;

namespace Microservice.StandaloneSurvey.Versioning.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class SchemaVersionAttribute : Attribute
    {
        public SchemaVersionAttribute(string version, VersionSchemaType objectType)
        {
            if (string.IsNullOrEmpty(version))
            {
                version = "1.0";
            }

            Version = version;
            ObjectType = objectType;
        }

        public string Version { get; private set; }

        public VersionSchemaType ObjectType { get; private set; }
    }
}
