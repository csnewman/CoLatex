using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoLatex.Database
{
    public class UserDbModel
    {
        [BsonId] public ObjectId InternalId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}