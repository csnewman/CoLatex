using MongoDB.Driver;

namespace CoLatex.Database
{
    public class DatabaseContext
    {
        public IMongoDatabase Database { get; private set; }

        public DatabaseContext()
        {
            MongoClient client = new MongoClient("mongodb://127.0.0.1");
            Database = client.GetDatabase("guh");
        }
    }
}