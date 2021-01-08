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
    public abstract class BaseMongoRepository<T> 
        where T : IEntity<ObjectId>
    {
        protected MongoDbContext _context;
        protected IMongoCollection<T> _collection;

        public IMongoCollection<T> Collection { get { return _collection; } }

        public BaseMongoRepository(IOptions<MongoDbSettings> settings)
        {
            _context = new MongoDbContext(settings);
            _collection = _context.GetDatabase().GetCollection<T>(typeof(T).Name.ToLower());
        }
    }
}
