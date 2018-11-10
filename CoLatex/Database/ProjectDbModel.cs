using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CoLatex.Database
{
    public class ProjectDbModel
    {
        [BsonId] public ObjectId InternalId { get; set; }
        [BsonElement("id")] public string Id { get; set; }
        [BsonElement("name")] public string Name { get; set; }
        [BsonElement("owner")] public string Owner { get; set; }
        [BsonElement("collaborators")] public IList<string> Collaborators { get; set; }
        [BsonElement("last_edit")] public long LastEdit { get; set; }
    }
}