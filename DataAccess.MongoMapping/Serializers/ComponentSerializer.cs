using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Alma.DataAccess.MongoMapping.Serializers
{
    internal class ComponentSerializer<TClass, TMember> : IBsonSerializer<TClass>, IBsonSerializer where TClass : class
    {
        private Expression<Func<TClass, TMember>> member;
        private PropertyInfo prop;
        private IBsonSerializer serializer;

        public ComponentSerializer(Expression<Func<TClass, TMember>> memberLambda)
        {
            this.member = memberLambda;
            this.prop = ReflectionTools.GetPropertyInfo(memberLambda);


            var propType = this.member.ReturnType;
            if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                propType = propType.GetGenericArguments()[0];

            if (propType == typeof(short))
                this.serializer = new Int16Serializer();
            else if (propType == typeof(int))
                this.serializer = new Int32Serializer();
            else if (propType == typeof(long))
                this.serializer = new Int64Serializer();
            else if (propType == typeof(ushort))
                this.serializer = new UInt16Serializer();
            else if (propType == typeof(uint))
                this.serializer = new UInt32Serializer();
            else if (propType == typeof(ulong))
                this.serializer = new UInt64Serializer();
            else if (propType == typeof(float))
                this.serializer = new SingleSerializer();
            else if (propType == typeof(double))
                this.serializer = new DoubleSerializer();
            else if (propType == typeof(Guid))
                this.serializer = new GuidSerializer();
            else if (propType == typeof(string))
                this.serializer = new StringSerializer();
            else if (propType == typeof(DateTime))
                this.serializer = new DateTimeSerializer();
            else if (propType == typeof(TimeSpan))
                this.serializer = new TimeSpanSerializer();
            else
                throw new NotImplementedException($"The type {propType.FullName} is not implemented.");
        }

        Type IBsonSerializer.ValueType => typeof(TClass);

        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;

            if (bsonReader.GetCurrentBsonType() == BsonType.Null)
            {
                bsonReader.ReadNull();
                return null;
            }
            else
            {
                var value = (TMember)this.serializer.Deserialize(context, args);

                var obj = ReflectionTools.CreateInstance<TClass>();


                prop.SetValue(obj, value);
                return obj;
            }
        }

        TClass IBsonSerializer<TClass>.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return (TClass)((IBsonSerializer)this).Deserialize(context, args);
        }

        void IBsonSerializer.Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            var bsonWriter = context.Writer;

            if (value == null)
            {
                bsonWriter.WriteNull();
            }
            else
            {
                var itemValue = prop.GetValue(value);

                this.serializer.Serialize(context, itemValue);
            }
        }

        void IBsonSerializer<TClass>.Serialize(BsonSerializationContext context, BsonSerializationArgs args, TClass value)
        {
            ((IBsonSerializer)this).Serialize(context, value);
        }
    }
}
