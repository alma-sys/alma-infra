using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Alma.DataAccess.MongoMapping.Serializers
{
    class DBRefListSerializer<TClass> : IBsonSerializer<TClass>, IBsonSerializer where TClass : class
    {
        private Type listType;
        private Type itemType;

        private BsonClassMap itemMap;
        private BsonMemberMap itemIdMap;

        public DBRefListSerializer(BsonMemberMap map)
        {
            this.listType = map.MemberType;

            if (!listType.IsGenericType || (listType.GetGenericTypeDefinition() != typeof(IList<>) && listType.GetGenericTypeDefinition() != typeof(IReadOnlyList<>)))
                throw new NotSupportedException("Please use generic IList<class> or IReadOnlyList<class> to map");

            this.itemType = listType.GetGenericArguments()[0];

            if (!itemType.IsClass)
                throw new NotSupportedException("Please use generic IList<class> or IReadOnlyList<class> to map");

        }


        Type IBsonSerializer.ValueType => listType;

        private TClass Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            CheckItemMap();
            var reader = context.Reader;
            var bsonType = reader.CurrentBsonType;
            var elementType = typeof(MongoDBRef);
            switch (bsonType)
            {
                case BsonType.Null:
                    reader.ReadNull();
                    return null;
                case BsonType.Array:
                    reader.ReadStartArray();
                    var lista_objs = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));

                    //var serializer = BsonSerializer.LookupSerializer(elementType);
                    while (reader.ReadBsonType() != BsonType.EndOfDocument)
                    {
                        var mref = BsonSerializer.Deserialize(reader, elementType) as MongoDBRef;
                        if (mref == null)
                            lista_objs.Add(null);
                        else
                        {
                            var obj = ReflectionTools.CreateInstance(itemType);
                            itemIdMap.Setter.Invoke(obj, BsonTypeMapper.MapToDotNetValue(mref.Id));
                            lista_objs.Add(obj);
                        }
                    }
                    reader.ReadEndArray();
                    return (TClass)(object)lista_objs;
                default:

                    var message = string.Format("Can't deserialize a {0} from BsonType {1}.", elementType.FullName, bsonType);
                    throw new ApplicationException(message);

            }
        }

        private void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TClass value)
        {
            CheckItemMap();
            var writer = context.Writer;
            var list = value as IEnumerable;
            if (list == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartArray();
                foreach (var i in list)
                {

                    if (i == null)
                        BsonSerializer.Serialize(writer, (object)null);
                    else
                    {
                        var mref = new MongoDBRef(ClassMappingExtensions.GetCollectionForType(itemType), BsonValue.Create(itemIdMap.Getter.Invoke(i)));
                        BsonSerializer.Serialize(writer, mref);
                    }

                }
                writer.WriteEndArray();

            }
        }

        private void CheckItemMap()
        {
            if (itemMap == null)
                itemMap = BsonClassMap.IsClassMapRegistered(itemType) ? BsonClassMap.LookupClassMap(itemType) : null;
            if (itemMap == null)
                throw new NotSupportedException($"Please map {itemType.Name}");
            if (itemIdMap == null)
                itemIdMap = itemMap.IdMemberMap;
            if (itemIdMap == null)
                throw new NotSupportedException($"Please map {itemType.Name} id property.");
        }

        #region Explicit Interface
        TClass IBsonSerializer<TClass>.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        void IBsonSerializer<TClass>.Serialize(BsonSerializationContext context, BsonSerializationArgs args, TClass value)
        {
            Serialize(context, args, value);
        }

        void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            Serialize(context, args, (TClass)value);
        }
        #endregion
    }
}
