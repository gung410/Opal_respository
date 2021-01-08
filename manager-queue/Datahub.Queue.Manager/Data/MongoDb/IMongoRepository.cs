using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Datahub.Queue.Manager.Domains;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Datahub.Queue.Manager.Data
{
    public interface IMongoRepository<T> where T : IEntity<ObjectId>
    {
        IMongoCollection<T> Collection { get; }
        int Count(Expression<Func<T, bool>> predicate = null);
        Task DeleteAsync(ObjectId id);
        bool Exists(Expression<Func<T, bool>> predicate);
        T GetById(ObjectId id);
        Task<T> InsertAsync(T instance);
        Task<IEnumerable<T>> InsertManyAsync(IEnumerable<T> instance);
        IReadOnlyList<T> List(Expression<Func<T, bool>> condition = null, Func<T, string> order = null);
        T Single(Expression<Func<T, bool>> predicate = null);
        Task UpdateAsync(T instance);
    }
}