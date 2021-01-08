using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datahub.Queue.Manager.Domains;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Datahub.Queue.Manager.Data.MongoDb
{
    public interface ITransactionalMongoRepository<T> where T : IEntity<ObjectId>
    {
        IClientSessionHandle StartTransaction();
        IClientSessionHandle StartTransaction(ReadConcern readConcern, WriteConcern writeConcern);
        void Commit(IClientSessionHandle session, bool isWithRetry = false);
        Task InsertAsync(IClientSessionHandle session, T instances, bool isWithRetry = true);
        Task InsertManyAsync(IClientSessionHandle session, IEnumerable<T> instances, bool isWithRetry = true);
        Task UpdateAsync(IClientSessionHandle session, T instance, bool isWithRetry = true);
        Task DeleteAsync(IClientSessionHandle session, ObjectId id, bool isWithRetry = true);
    }
}