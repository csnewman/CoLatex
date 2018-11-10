using MongoDB.Driver;

namespace CoLatex.Database
{
    public class DatabaseContext
    {
        private IMongoDatabase _database;

        public DatabaseContext()
        {
            MongoClient client = new MongoClient("mongodb://127.0.0.1");
            _database = client.GetDatabase("guh");
        }

        public IMongoCollection<UserDbModel> Users => _database.GetCollection<UserDbModel>("users");

        public IMongoCollection<ProjectDbModel> Projects => _database.GetCollection<ProjectDbModel>("projects");
    }
}