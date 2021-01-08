using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Conexus.Opal.Shared.MongoDb
{
    public interface IHasMongoId
    {
        [BsonId]
        public Guid Id { get; set; }
    }
}
