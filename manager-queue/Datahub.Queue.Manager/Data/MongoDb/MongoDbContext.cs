using Datahub.Queue.Manager.Domains;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Datahub.Queue.Manager.Data.MongoDb
{
    public class MongoDbContext
    {
        private readonly string _mongoServerUrl;
        private readonly string _mongoDbName;
        private readonly MongoClient _client;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            _mongoServerUrl = settings.Value.ConnectionString;
            _mongoDbName = settings.Value.DatabaseName;
            var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(_mongoServerUrl));
            clientSettings.UseSsl = settings.Value.SslEnabled;
            if (clientSettings.UseSsl)
            {
                clientSettings.VerifySslCertificate = false;
            }

            _client = new MongoClient(clientSettings);
        }

        internal IMongoClient GetClient()
        {
            return _client;
        }

        public IMongoDatabase GetDatabase() { return _client.GetDatabase(_mongoDbName); }

        public bool IsClusterConnceted
        {
            get
            {
                return _client.Cluster.Description.State == ClusterState.Connected;
            }
        }

        public bool IsServerConnceted
        {
            get
            {
                return _client.Cluster.Description.Servers.All(p => p.State == ServerState.Connected);
            }
        }

        public void DropDatabase(string dbName)
        {
            _client.DropDatabase(dbName);
        }

        public void DropCollection<T>() where T : IEntity<ObjectId>
        {
            var database = GetDatabase();
            var collectionName = typeof(T).Name.ToLower();

            if (database.GetCollection<T>(collectionName) != null)
            {
                database.DropCollection(collectionName);
            }
        }
    }

    public class MongoDbSettings
    {
        public string ConnectionString;
        public string DatabaseName;
        public bool SslEnabled { get; set; }
    }
}
