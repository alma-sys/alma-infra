using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Linq;

namespace Alma.DataAccess.MongoMapping.Serializers
{
    class DBRefSerializer<TClass> : IBsonSerializer<TClass>, IBsonSerializer where TClass : class
    {
        private Type itemType;

        private MongoDBRefSerializer dbRef;
        private BsonClassMap itemMap;
        private BsonMemberMap itemIdMap;

        public DBRefSerializer(BsonMemberMap map)
        {
            this.itemType = map.MemberType;

            if (!itemType.IsClass)
                throw new NotSupportedException("Please use a class to map");

            this.dbRef = new MongoDBRefSerializer();
        }


        Type IBsonSerializer.ValueType => itemType;

        private TClass Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            CheckItemMap();
            var bsonReader = context.Reader;
            if (bsonReader.GetCurrentBsonType() == BsonType.Null)
            {
                bsonReader.ReadNull();
                return null;
            }
            else
            {
                var idref = dbRef.Deserialize(context, args);

                if (idref == null || idref.Id == null)
                    return null;

                TClass obj = ReflectionTools.CreateInstance<TClass>();
                itemIdMap.Setter.Invoke(obj, BsonTypeMapper.MapToDotNetValue(idref.Id));

                //TODO: Buscar na collection correta a instancia completa.
                return obj;
            }

        }

        private void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TClass value)
        {
            CheckItemMap();

            if (value == null)
                this.dbRef.Serialize(context, args, null);
            else
            {
                var idref = new MongoDBRef(ClassMappingExtensions.GetCollectionForType(itemType), BsonValue.Create(itemIdMap.Getter.Invoke(value)));

                this.dbRef.Serialize(context, args, idref);

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
