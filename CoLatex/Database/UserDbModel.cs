using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoLatex.Database
{
    public class UserDbModel
    {
        [BsonId] public ObjectId InternalId { get; set; }
        [BsonElement("username")] public string Username { get; set; }
        [BsonElement("password")] public string Password { get; set; }
        [BsonElement("name")] public string Name { get; set; }
        [BsonElement("email")] public string Email { get; set; }
    }
}