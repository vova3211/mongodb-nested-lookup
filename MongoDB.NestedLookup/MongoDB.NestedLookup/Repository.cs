using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.NestedLookup.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace MongoDB.NestedLookup
{
    public class Repository
    {
        protected IMongoClient _client;
        protected IMongoDatabase _database;

        public Repository()
        {
            var url = new MongoUrl("mongodb://localhost:27017/nested");
            _client = new MongoClient();
            _database = _client.GetDatabase(url.DatabaseName);

        }

        public IMongoCollection<T> GetCollection<T>() where T : class, new()
        {
            return _database.GetCollection<T>(typeof(T).Name);
        }

        public IMongoCollection<T> GetCollection<T>(string name) where T : class, new()
        {
            return _database.GetCollection<T>(name);
        }

        public IEnumerable Find<T>() where T : class, new()
        {
            return _database.GetCollection<T>(typeof(T).Name).Find(x => true).ToEnumerable();
        }

        public async Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            return await _database.GetCollection<T>(typeof(T).Name).Find(expression).FirstOrDefaultAsync();
        }

        public async Task<T> FirstOrDefaultWithRelationsAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            var type = typeof(T);
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = property.GetCustomAttribute<RelationshipAttribute>(false);

                if (attr != null)
                {
                    if (attr.Type == RelationshipType.OneToOne)
                    {
                        var propertyTypeImplementsIBaseEntity = property.PropertyType.GetInterfaces().Contains(typeof(IBaseEntity));

                        if (!propertyTypeImplementsIBaseEntity)
                        {
                            throw new Exception($"Property type of {property.Name} should implement {nameof(IBaseEntity)} interface.");
                        }

                        var s = await GetCollection<T>().Aggregate()
                            .Match(expression)
                            .Lookup(
                              property.PropertyType.Name,
                              attr.Field,
                              "_id",
                              property.Name
                            )
                            .Unwind(property.Name)
                            .FirstOrDefaultAsync();

                        return BsonSerializer.Deserialize<T>(s);
                    }

                    if (attr.Type == RelationshipType.OneToMany)
                    {
                        if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(IList<>))
                        {
                            throw new Exception($"Relation OneToMany should be used on IList<> type.");
                        }

                        var realType = property.PropertyType.GetGenericArguments()[0];
                        var propertyTypeImplementsIBaseEntity = realType.GetInterfaces().Contains(typeof(IBaseEntity));

                        if (!propertyTypeImplementsIBaseEntity)
                        {
                            throw new Exception($"Property type of {property.Name} should implement {nameof(IBaseEntity)} interface.");
                        }

                        var s = await GetCollection<T>().Aggregate()
                            .Match(expression)
                            .Lookup(
                              realType.Name,
                              "_id",
                              attr.Field,
                              property.Name
                            )
                            .FirstOrDefaultAsync();

                        return BsonSerializer.Deserialize<T>(s);
                    }
                }
            }

            return await FirstOrDefaultAsync(expression);
        }

        public async Task<IEnumerable<T>> FindWithRelationsAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            var type = typeof(T);
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = property.GetCustomAttribute<RelationshipAttribute>(false);

                if (attr != null)
                {
                    if (attr.Type == RelationshipType.OneToOne)
                    {
                        var propertyTypeImplementsIBaseEntity = property.PropertyType.GetInterfaces().Contains(typeof(IBaseEntity));

                        if (!propertyTypeImplementsIBaseEntity)
                        {
                            throw new Exception($"Property type of {property.Name} should implement {nameof(IBaseEntity)} interface.");
                        }

                        var s = await GetCollection<T>().Aggregate()
                            .Match(expression)
                            .Lookup(
                              property.PropertyType.Name,
                              attr.Field,
                              "_id",
                              property.Name
                            )
                            .Unwind(property.Name)
                            .ToListAsync();

                        return s.Select(x => BsonSerializer.Deserialize<T>(x));
                    }

                    if (attr.Type == RelationshipType.OneToMany)
                    {
                        if (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(IList<>))
                        {
                            throw new Exception($"Relation OneToMany should be used on IList<> type.");
                        }

                        var realType = property.PropertyType.GetGenericArguments()[0];
                        var propertyTypeImplementsIBaseEntity = realType.GetInterfaces().Contains(typeof(IBaseEntity));

                        if (!propertyTypeImplementsIBaseEntity)
                        {
                            throw new Exception($"Property type of {property.Name} should implement {nameof(IBaseEntity)} interface.");
                        }

                        var s = await GetCollection<T>().Aggregate()
                            .Match(expression)
                            .Lookup(
                              realType.Name,
                              "_id",
                              attr.Field,
                              property.Name
                            )
                            .ToListAsync();

                        return s.Select(x => BsonSerializer.Deserialize<T>(x));
                    }
                }
            }

            return null;
        }
    }

    public static class AttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }

            return default;
        }
    }
}
