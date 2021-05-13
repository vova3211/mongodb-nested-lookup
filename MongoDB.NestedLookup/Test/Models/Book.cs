using MongoDB.Bson.Serialization.Attributes;
using MongoDB.NestedLookup;
using MongoDB.NestedLookup.Entity;

namespace Test.Models
{
    [BsonIgnoreExtraElements]
    public class Book : BaseEntity
    {
        public Book()
        {

        }

        public Book(string name, int pages, string authorId)
        {
            Name = name;
            Pages = pages;
            AuthorId = authorId;
        }

        public string Name { get; set; }

        public int Pages { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string AuthorId { get; set; }

        [BsonIgnoreIfNull]
        [Relationship(RelationshipType.OneToOne, nameof(AuthorId))]
        public Author Author { get; set; }
    }
}
