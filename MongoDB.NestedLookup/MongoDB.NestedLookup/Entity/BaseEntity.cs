using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.NestedLookup.Entity
{
    public class BaseEntity : IBaseEntity
    {
        [BsonId]
        [BsonRepresentation(Bson.BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
