using MongoDB.Bson.Serialization.Attributes;
using MongoDB.NestedLookup;
using MongoDB.NestedLookup.Entity;
using System;
using System.Collections.Generic;

namespace Test.Models
{
    [BsonIgnoreExtraElements]
    public class Author : BaseEntity
    {
        public Author()
        {

        }

        public Author(string name, DateTime birth)
        {
            Name = name;
            Birth = birth;
        }

        public string Name { get; set; }

        public DateTime Birth { get; set; }

        [BsonIgnoreIfNull]
        [Relationship(RelationshipType.OneToMany, nameof(Book.AuthorId))]
        public IList<Book> Books { get; set; }
    }
}
