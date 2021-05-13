using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.NestedLookup
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RelationshipAttribute : Attribute
    {
        public RelationshipType Type;
        public string Field;

        public RelationshipAttribute(RelationshipType type, string field)
        {
            Type = type;
            Field = field;
        }
    }

    public enum RelationshipType
    {
        OneToOne,
        OneToMany,
        ManyToMany
    }
}
