using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Communication.DataAccess
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected IMongoDatabase _database;
        protected IMongoCollection<T> _collection;
        public RepositoryBase(IMongoDatabase database, IConfiguration configuration)
        {
            _database = database;
            _collection = _database.GetCollection<T>(typeof(T).Name);
        }
        public async Task Insert(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task<bool> Update(ObjectId id, UpdateDefinition<T> updateDefinition)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);

            var result = await _collection.UpdateOneAsync(filter, updateDefinition);

            return result.ModifiedCount != 0;
        }

        public async Task<bool> UpdateOneAsync(FilterDefinition<T> filterDefinitionBuilder, UpdateDefinition<T> updateDefinition)
        {
            var result = await _collection.UpdateOneAsync(filterDefinitionBuilder, updateDefinition);

            return result.ModifiedCount != 0;
        }

        public async Task<bool> DeleteById(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount != 0;
        }

        public async Task<bool> DeleteByFilter(FilterDefinition<T> filterDefinition)
        {
            var result = await _collection.DeleteManyAsync(filterDefinition);
            return result.DeletedCount != 0;
        }

        public async Task<long> DeleteAll()
        {
            var filter = new BsonDocument();
            var result = await _collection.DeleteManyAsync(filter);
            return result.DeletedCount;
        }

        public async Task<string> CreateIndexOnCollection(IMongoCollection<T> collection, string field)
        {
            IndexKeysDefinition<T> keys = Builders<T>.IndexKeys.Ascending(field);
            //Add an optional name- useful for admin
            var options = new CreateIndexOptions { Name = $"{typeof(T).Name}_Index_{field}" };
            var indexModel = new CreateIndexModel<T>(keys, options);
            return await collection.Indexes.CreateOneAsync(indexModel);
        }

        public async Task<List<T>> GetAllAsync(params Expression<Func<T, bool>>[] filter)
        {
            var query = _collection.AsQueryable();
            foreach (var item in filter)
            {
                query = query.Where(item);
            }
            return await query.ToListAsync();
        }

        public IMongoQueryable<T> GetCollectionQuery(params Expression<Func<T, bool>>[] filter)
        {
            return _collection.AsQueryable();
        }

        public async Task<long> CountAllAsync(params Expression<Func<T, bool>>[] filter)
        {
            var query = _collection.AsQueryable();
            foreach (var item in filter)
            {
                query = query.Where(item);
            }
            return await query.LongCountAsync();
        }

        public async Task<List<T>> GetAllAsyncPaging(int pageNo, int pageSize, Expression<Func<T, object>> orderBy, bool orderAsc = true, params Expression<Func<T, bool>>[] filter)
        {
            var query = _collection.AsQueryable();
            foreach (var item in filter)
            {
                query = query.Where(item);
            }
            if (orderBy != null)
            {
                if (orderAsc)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }
            if (pageNo > 0 && pageSize > 0)
            {
                query = query.Skip(pageSize * (pageNo - 1)).Take(pageSize);
            }
            return await query.ToListAsync();
        }

        public async Task InsertManyAsync(List<T> entities)
        {
            await _collection.InsertManyAsync(entities);
        }

        public async Task<UpdateResult> UpdateManyAsync(FilterDefinition<T> filterDefinitionBuilder, UpdateDefinition<T> updateDefinitionBuilder)
        {
            return await _collection.UpdateManyAsync(filterDefinitionBuilder, updateDefinitionBuilder, null);
        }

        public async Task InsertOneAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }
    }
}
