using Datahub.Queue.Manager.Data.MongoDb;
using Datahub.Queue.Manager.Domains;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Datahub.Queue.Manager.Data.MongoDb
{
    public class TransactionalMongoRepository<T> : BaseMongoRepository<T>, ITransactionalMongoRepository<T> where T : IEntity<ObjectId>
    {
        private readonly IMongoClient _client;
        public TransactionalMongoRepository(IOptions<MongoDbSettings> settings) : base(settings)
        {
            _client = _context.GetClient();
        }

        public void Commit(IClientSessionHandle session, bool isWithRetry = false)
        {
            while (true)
            {
                try
                {
                    session.CommitTransaction();
                    break;
                }
                catch (MongoException exception)
                {
                    if (exception.HasErrorLabel("UnknownTransactionCommitResult") && isWithRetry)
                    {
                        continue;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public IClientSessionHandle StartTransaction(ReadConcern readConcern, WriteConcern writeConcern)
        {
            var session = _client.StartSession();
            session.StartTransaction(new TransactionOptions(readConcern: readConcern, writeConcern: writeConcern));
            return session;
        }

        public IClientSessionHandle StartTransaction()
        {
            var session = _client.StartSession();
            session.StartTransaction(new TransactionOptions(readConcern: ReadConcern.Snapshot, writeConcern: WriteConcern.WMajority));
            return session;
        }

        public async Task InsertAsync(IClientSessionHandle session, T instances, bool isWithRetry = true)
        {
            while (true)
            {
                try
                {
                    await _collection.InsertOneAsync(session, instances);
                    break;
                }
                catch (MongoException exception)
                {
                    if (exception.HasErrorLabel("TransientTransactionError") && isWithRetry)
                    {
                        continue;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public async Task InsertManyAsync(IClientSessionHandle session, IEnumerable<T> instances, bool isWithRetry = true)
        {
            while (true)
            {
                try
                {
                    await _collection.InsertManyAsync(session, instances);
                    break;
                }
                catch (MongoException exception)
                {
                    if (exception.HasErrorLabel("TransientTransactionError") && isWithRetry)
                    {
                        continue;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public async Task UpdateAsync(IClientSessionHandle session, T instance, bool isWithRetry = true)
        {
            while (true)
            {
                try
                {
                    var query = Builders<T>.Filter.Eq(o => o.Id, instance.Id);
                    await _collection.ReplaceOneAsync(query, instance);
                    break;
                }
                catch (MongoException exception)
                {
                    if (exception.HasErrorLabel("TransientTransactionError") && isWithRetry)
                    {
                        continue;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public async Task DeleteAsync(IClientSessionHandle session, ObjectId id, bool isWithRetry = true)
        {
            while (true)
            {
                try
                {
                    await _collection.DeleteOneAsync(session, Builders<T>.Filter.Eq(p => p.Id, id));
                    break;
                }
                catch (MongoException exception)
                {
                    if (exception.HasErrorLabel("TransientTransactionError") && isWithRetry)
                    {
                        continue;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}
