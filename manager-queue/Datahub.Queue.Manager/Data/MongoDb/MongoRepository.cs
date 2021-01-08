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

namespace Datahub.Queue.Manager.Data
{
    public class MongoRepository<T> : BaseMongoRepository<T>, IMongoRepository<T> where T : IEntity<ObjectId>
    {
        public MongoRepository(IOptions<MongoDbSettings> settings):base(settings)
        {
        }

        private IQueryable<T> CreateSet()
        {
            return _collection.AsQueryable<T>();
        }

        public async Task<T> InsertAsync(T instance)
        {
            try
            {
                await _collection.InsertOneAsync(instance);

                return instance;
            }
            catch (Exception ex)
            {
                //todo: handle exception
                throw ex;
            }
        }

        public async Task<IEnumerable<T>> InsertManyAsync(IEnumerable<T> instances)
        {
            try
            {
                await _collection.InsertManyAsync(instances);

                return instances;
            }
            catch (Exception ex)
            {
                //todo: handle exception
                throw ex;
            }
        }

        public async Task UpdateAsync(T instance)
        {
            try
            {
                var query = Builders<T>.Filter.Eq(o => o.Id, instance.Id);
                await _collection.ReplaceOneAsync(query, instance);
            }
            catch (Exception ex)
            {
                //todo: handle exception
                throw ex;
            }
        }

        public async Task DeleteAsync(ObjectId id)
        {
            try
            {
                await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(p => p.Id, id));
            }
            catch (Exception ex)
            {
                //todo: handle exception
                throw ex;
            }
        }

        public T GetById(ObjectId id)
        {
            return this.Single(o => o.Id == id);
        }

        public T Single(Expression<Func<T, bool>> predicate = null)
        {
            var set = CreateSet();
            var query = (predicate == null ? set : set.Where(predicate));

            return query.SingleOrDefault();
        }

        public IReadOnlyList<T> List(Expression<Func<T, bool>> condition = null, Func<T, string> order = null)
        {
            var set = CreateSet();
            if (condition != null)
            {
                set = set.Where(condition);
            }

            if (order != null)
            {
                return set.OrderBy(order).ToList();
            }

            return set.ToList();
        }

        public int Count(Expression<Func<T, bool>> predicate = null)
        {
            var set = CreateSet();

            return (predicate == null ? set.Count() : set.Count(predicate));
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            var set = CreateSet();
            return set.Any(predicate);
        }
    }
}
