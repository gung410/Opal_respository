using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Microservice.Badge.Domain.Entities
{
    public class Community
    {
        public Community(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        [BsonId]
        public Guid Id { get; init; }

        public string Name { get; set; }

        public void UpdateName(string name)
        {
            Name = name;
        }
    }
}
