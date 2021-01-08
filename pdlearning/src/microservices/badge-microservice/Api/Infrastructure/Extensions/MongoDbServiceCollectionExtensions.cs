using System.Collections.Generic;
using Conexus.Opal.Shared.MongoDb;
using Microservice.Badge.Domain.Entities;
using Microservice.Badge.Domain.Enums;
using Microservice.Badge.Domain.ValueObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;

namespace Microservice.Badge.Infrastructure.Extensions
{
    public static class MongoDbServiceCollectionExtensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterCustomBsonSerializer();

            services
                .AddOptions<MongoOptions>()
                .Bind(configuration.GetSection(nameof(MongoOptions)));

            services.AddSingleton<BadgeDbClient>();
            services.AddScoped<BadgeDbContext>();
            services.AddScoped<BadgeSeeder>();

            services.AddTransient<IStartupFilter, MongoMigrationStartupFilter>();

            return services;
        }

        public static void RegisterCustomBsonSerializer()
        {
            ConventionRegistry.Register(
                "EnumStringConvention",
                new ConventionPack
                {
                    new EnumRepresentationConvention(BsonType.String)
                },
                t => true);

            BsonClassMap.RegisterClassMap<RewardBadgeLimitation>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.LimitValues).SetSerializer(new DictionaryInterfaceImplementerSerializer<Dictionary<RewardBadgeLimitType, int>>(DictionaryRepresentation.ArrayOfDocuments));
            });

            BsonClassMap.RegisterClassMap<BadgeEntity>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(c => c.LevelImages).SetSerializer(new DictionaryInterfaceImplementerSerializer<Dictionary<BadgeLevelEnum, string>>(DictionaryRepresentation.ArrayOfDocuments));
            });
#pragma warning disable CS0618 // Type or member is obsolete
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
