using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Conexus.Opal.Shared.MongoDb
{
    public class MongoEntity : IHasMongoId
    {
        [BsonId]
        public Guid Id { get; set; }
    }
}
