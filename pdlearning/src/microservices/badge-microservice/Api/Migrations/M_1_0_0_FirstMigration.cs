using MongoDB.Driver;

namespace Microservice.Badge.Migrations
{
    public class M_1_0_0_FirstMigration : MongoDBMigrations.IMigration
    {
        public MongoDBMigrations.Version Version => new(1, 0, 0);

        public string Name => "First migration";

        public void Down(IMongoDatabase database)
        {
            // Nothing to do because this is the first migration
        }

        public void Up(IMongoDatabase database)
        {
            // Nothing to do because this is the first migration
        }
    }
}
