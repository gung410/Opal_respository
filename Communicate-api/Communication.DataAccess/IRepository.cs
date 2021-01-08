using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Communication.DataAccess
{
    public interface IRepository<T> where T : class
    {
        Task Insert(T Content);
        Task<List<T>> GetAllAsync(params Expression<Func<T, bool>>[] filter);
        Task InsertManyAsync(List<T> entities);

        Task InsertOneAsync(T entity);
        Task<UpdateResult> UpdateManyAsync(FilterDefinition<T> filterDefinitionBuilder, UpdateDefinition<T> updateDefinitionBuilder);
        Task<bool> Update(ObjectId id, UpdateDefinition<T> updateDefinition);
        Task<bool> DeleteById(ObjectId id);
        Task<long> DeleteAll();
        Task<string> CreateIndexOnCollection(IMongoCollection<T> collection, string field);
        Task<List<T>> GetAllAsyncPaging(int pageNo, int pageSize, Expression<Func<T, object>> orderBy, bool orderAsc = true, params Expression<Func<T, bool>>[] filter);
        Task<long> CountAllAsync(params Expression<Func<T, bool>>[] filter);
        Task<bool> DeleteByFilter(FilterDefinition<T> filterDefinition);

        Task<bool> UpdateOneAsync(FilterDefinition<T> filterDefinitionBuilder, UpdateDefinition<T> updateDefinition);
        IMongoQueryable<T> GetCollectionQuery(params Expression<Func<T, bool>>[] filter);
    }
}
